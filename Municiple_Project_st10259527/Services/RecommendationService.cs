using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Threading;

namespace Municiple_Project_st10259527.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _context;
        private const int MAX_RECOMMENDATIONS = 5;
        private const int MAX_SEARCH_HISTORY = 10;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventModel>> GetRecommendedEventsAsync(int userId)
        {
            var recommendedEvents = new SortedDictionary<DateTime, EventModel>();
            var eventIds = new HashSet<int>();
            var scores = new Dictionary<int, int>();
            var scoreBuckets = new SortedDictionary<int, SortedDictionary<DateTime, EventModel>>(Comparer<int>.Create((a,b) => b.CompareTo(a)));

            void AddToScoreBuckets(EventModel evt, int scoreIncrement)
            {
                if (evt == null) return;
                if (!scores.TryGetValue(evt.EventId, out var s)) s = 0;
                s += scoreIncrement;
                scores[evt.EventId] = s;

                if (!scoreBuckets.TryGetValue(s, out var bucket))
                {
                    bucket = new SortedDictionary<DateTime, EventModel>();
                    scoreBuckets[s] = bucket;
                }

                // ensure key uniqueness similar to recommendedEvents
                var key = evt.Date;
                while (bucket.ContainsKey(key)) key = key.AddTicks(1);
                bucket[key] = evt;
            }

            // Base query for upcoming events (no tracking for performance)
            var baseQuery = _context.Events
                .AsNoTracking()
                .Where(e => e.Date >= DateTime.Today);

            // Prepare frequency maps without using lists/arrays
            var termFreq = new Dictionary<string, (int Count, DateTime Last)>();
            var categoryFreq = new Dictionary<string, (int Count, DateTime Last)>(StringComparer.OrdinalIgnoreCase);
            UserSearchHistory recent1 = null, recent2 = null, recent3 = null;

            int historyCount = 0;
            await foreach (var s in _context.UserSearchHistory
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SearchDate)
                .Take(MAX_SEARCH_HISTORY)
                .AsAsyncEnumerable())
            {
                // track top 3 recent by streaming order
                if (recent1 == null) recent1 = s;
                else if (recent2 == null) recent2 = s;
                else if (recent3 == null) recent3 = s;

                var term = (s.SearchTerm ?? string.Empty).ToLower();
                if (!string.IsNullOrWhiteSpace(term))
                {
                    if (!termFreq.TryGetValue(term, out var v)) v = (0, DateTime.MinValue);
                    v.Count += 1;
                    if (s.SearchDate > v.Last) v.Last = s.SearchDate;
                    termFreq[term] = v;
                }

                var cat = (s.Category ?? "General").ToLower();
                if (!string.Equals(cat, "general", StringComparison.OrdinalIgnoreCase))
                {
                    if (!categoryFreq.TryGetValue(cat, out var c)) c = (0, DateTime.MinValue);
                    c.Count += 1;
                    if (s.SearchDate > c.Last) c.Last = s.SearchDate;
                    categoryFreq[cat] = c;
                }

                historyCount++;
            }

            // If no search history, return upcoming events
            if (historyCount == 0)
            {
                var result = new SortedDictionary<DateTime, EventModel>();
                await AddMatchingEventsToDictionary(
                    baseQuery.OrderBy(e => e.Date),
                    result,
                    new HashSet<int>(),
                    MAX_RECOMMENDATIONS
                );
                return result.Values;
            }

            // Build sorted views using SortedDictionary without lists/arrays
            var termsByCount = new SortedDictionary<int, HashSet<(string Term, DateTime Last)>>(Comparer<int>.Create((a,b) => b.CompareTo(a)));
            foreach (var kv in termFreq)
            {
                if (!termsByCount.TryGetValue(kv.Value.Count, out var set))
                {
                    set = new HashSet<(string, DateTime)>();
                    termsByCount[kv.Value.Count] = set;
                }
                set.Add((kv.Key, kv.Value.Last));
            }

            var catsByCount = new SortedDictionary<int, HashSet<(string Cat, DateTime Last)>>(Comparer<int>.Create((a,b) => b.CompareTo(a)));
            foreach (var kv in categoryFreq)
            {
                if (!catsByCount.TryGetValue(kv.Value.Count, out var set))
                {
                    set = new HashSet<(string, DateTime)>();
                    catsByCount[kv.Value.Count] = set;
                }
                set.Add((kv.Key, kv.Value.Last));
            }

            // Stop-terms to reduce overly-generic matches
            var stopTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "day", "event" };

            // 1. First priority: Exact matches for recent searches (process up to 3 most recent)
            async Task ProcessRecent(UserSearchHistory search)
            {
                if (search == null || recommendedEvents.Count >= MAX_RECOMMENDATIONS) return;

                var term = (search.SearchTerm ?? string.Empty).Trim().ToLower();
                if (string.IsNullOrWhiteSpace(term)) return;
                var category = search.Category?.Trim().ToLower();

                var query = baseQuery.AsQueryable();
                
                if (!string.IsNullOrEmpty(category) && category != "general")
                {
                    query = query.Where(e => 
                        (e.Category != null && e.Category.ToLower() == category) &&
                        ((e.Title != null && e.Title.ToLower().Contains(term)) || (e.Description != null && e.Description.ToLower().Contains(term)))
                    );
                }
                else
                {
                    query = query.Where(e => 
                        (e.Title != null && e.Title.ToLower().Contains(term)) || 
                        (e.Description != null && e.Description.ToLower().Contains(term))
                    );
                }

                await AddMatchingEventsToDictionary(
                    query.OrderBy(e => e.Date),
                    recommendedEvents,
                    eventIds,
                    MAX_RECOMMENDATIONS - recommendedEvents.Count,
                    onAdded: e => AddToScoreBuckets(e, string.IsNullOrEmpty(category) || category == "general" ? 3 : 5),
                    onExisting: e => AddToScoreBuckets(e, 1)
                );
            }

            await ProcessRecent(recent1);
            await ProcessRecent(recent2);
            await ProcessRecent(recent3);

            // 2. Second priority: Match any recent search terms (broader search)
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS && termsByCount.Count > 0)
            {
                int taken = 0;
                foreach (var bucket in termsByCount)
                {
                    // Order within bucket by last searched descending without arrays
                    var ordered = bucket.Value.OrderByDescending(t => t.Last);
                    foreach (var entry in ordered)
                    {
                        if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 5) break;
                        var term = entry.Term;
                        if (term.Length < 4 || stopTerms.Contains(term)) { taken++; continue; }
                        var query = baseQuery
                            .Where(e => !eventIds.Contains(e.EventId) &&
                                      ((e.Title != null && e.Title.ToLower().Contains(term)) ||
                                       (e.Description != null && e.Description.ToLower().Contains(term))))
                            .OrderBy(e => e.Date);

                        await AddMatchingEventsToDictionary(
                            query,
                            recommendedEvents,
                            eventIds,
                            MAX_RECOMMENDATIONS - recommendedEvents.Count,
                            onAdded: e => AddToScoreBuckets(e, 2),
                            onExisting: e => AddToScoreBuckets(e, 1)
                        );
                        taken++;
                    }
                    if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 5) break;
                }
            }

            // 3. Third priority: Match any recent categories
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS && catsByCount.Count > 0)
            {
                int taken = 0;
                foreach (var bucket in catsByCount)
                {
                    var ordered = bucket.Value.OrderByDescending(c => c.Last);
                    foreach (var entry in ordered)
                    {
                        if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 3) break;
                        var cat = entry.Cat.ToLower();
                        var query = baseQuery
                            .Where(e => !eventIds.Contains(e.EventId) &&
                                      (e.Category != null && e.Category.ToLower() == cat))
                            .OrderBy(e => e.Date);

                        await AddMatchingEventsToDictionary(
                            query,
                            recommendedEvents,
                            eventIds,
                            MAX_RECOMMENDATIONS - recommendedEvents.Count,
                            onAdded: e => AddToScoreBuckets(e, 1),
                            onExisting: e => AddToScoreBuckets(e, 1)
                        );
                        taken++;
                    }
                    if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 3) break;
                }
            }

            // 4. Fallback: Get upcoming events if we still need more
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS)
            {
                var countBefore = recommendedEvents.Count;
                await AddMatchingEventsToDictionary(
                    baseQuery.Where(e => !eventIds.Contains(e.EventId))
                            .OrderBy(e => e.Date),
                    recommendedEvents,
                    eventIds,
                    MAX_RECOMMENDATIONS - recommendedEvents.Count,
                    onAdded: e => AddToScoreBuckets(e, 0)
                );
            }

            // Return ordered by score desc, then date asc
            return scoreBuckets.SelectMany(b => b.Value.Values);
        }

        public async Task LogSearchAsync(int userId, string searchTerm, string category = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return;
            }

            try
            {
                if (!await _context.Database.CanConnectAsync())
                {
                    return;
                }

                // Ensure category is never null to satisfy non-nullable DB column
                var safeCategory = string.IsNullOrWhiteSpace(category) ? "General" : category;

                _context.UserSearchHistory.Add(new UserSearchHistory
                {
                    UserId = userId,
                    SearchTerm = searchTerm.Trim(),
                    Category = safeCategory,
                    SearchDate = DateTime.Now
                });
                
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Silently handle any errors
            }
        }

        private async Task AddMatchingEventsToDictionary(
            IQueryable<EventModel> query,
            SortedDictionary<DateTime, EventModel> dictionary,
            HashSet<int> eventIds,
            int maxToAdd,
            Action<EventModel> onAdded = null,
            Action<EventModel> onExisting = null)
        {
            if (maxToAdd <= 0) 
            {
                return;
            }

            int targetCount = dictionary.Count + maxToAdd;
            int scanned = 0;

            try
            {
                int addedCount = 0;
                await foreach (var evt in query.AsAsyncEnumerable())
                {
                    if (dictionary.Count >= targetCount) 
                    {
                        break;
                    }

                    scanned++;
                    if (dictionary.Count >= targetCount) 
                    {
                        break;
                    }

                    if (!eventIds.Contains(evt.EventId))
                    {
                        try
                        {
                            // ensure key uniqueness similar to recommendedEvents
                            var key = evt.Date;
                            while (dictionary.ContainsKey(key)) 
                                key = key.AddTicks(1);

                            dictionary[key] = evt;
                            eventIds.Add(evt.EventId);
                            addedCount++;
                            onAdded?.Invoke(evt);
                        }
                        catch
                        {
                            // Silently handle any errors
                        }
                    }
                    else
                    {
                        onExisting?.Invoke(evt);
                    }
                }
            }
            catch
            {
                // Silently handle any errors
            }
        }
    }
}