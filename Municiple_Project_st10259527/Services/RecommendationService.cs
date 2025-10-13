using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Municiple_Project_st10259527.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RecommendationService> _logger;
        private const int MAX_RECOMMENDATIONS = 5;
        private const int MAX_SEARCH_HISTORY = 10;

        public RecommendationService(AppDbContext context, ILogger<RecommendationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private void LogRecommendationStep(string step, string message, object data = null)
        {
            _logger.LogInformation($"[Recommendation] {step}: {message}");
            if (data != null)
            {
                _logger.LogInformation($"[Recommendation] {step} Data: {System.Text.Json.JsonSerializer.Serialize(data)}");
            }
        }

        public async Task<IEnumerable<EventModel>> GetRecommendedEventsAsync(int userId)
        {
            LogRecommendationStep("Start", $"Getting recommendations for user {userId}", new { UserId = userId });
            
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

            // Base query for upcoming events
            var baseQuery = _context.Events
                .Where(e => e.Date >= DateTime.Today);

            LogRecommendationStep("SearchHistory", "Fetching user's search history");
            
            // Prepare frequency maps without using lists/arrays
            var termFreq = new Dictionary<string, (int Count, DateTime Last)>();
            var categoryFreq = new Dictionary<string, (int Count, DateTime Last)>(StringComparer.OrdinalIgnoreCase);
            UserSearchHistory recent1 = null, recent2 = null, recent3 = null;

            int historyCount = 0;
            await foreach (var s in _context.UserSearchHistory
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

            LogRecommendationStep("SearchHistory", $"Found {historyCount} search history items",
                termFreq.Select(kv => new { Term = kv.Key, kv.Value.Count, kv.Value.Last }));

            // If no search history, return upcoming events
            if (historyCount == 0)
            {
                LogRecommendationStep("NoHistory", "No search history found, returning upcoming events");
                var result = new SortedDictionary<DateTime, EventModel>();
                await AddMatchingEventsToDictionary(
                    baseQuery.OrderBy(e => e.Date),
                    result,
                    new HashSet<int>(),
                    MAX_RECOMMENDATIONS
                );
                LogRecommendationStep("UpcomingEvents", $"Returning {result.Count} upcoming events");
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

            LogRecommendationStep("Analysis", "Search terms analysis",
                termsByCount.Select(kv => new { Count = kv.Key, Terms = kv.Value.Select(t => new { t.Term, t.Last }) }));
            LogRecommendationStep("Analysis", "Categories analysis",
                catsByCount.Select(kv => new { Count = kv.Key, Categories = kv.Value.Select(c => new { c.Cat, c.Last }) }));

            // Stop-terms to reduce overly-generic matches
            var stopTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "day", "event" };

            // 1. First priority: Exact matches for recent searches (process up to 3 most recent)
            async Task ProcessRecent(UserSearchHistory search)
            {
                if (search == null || recommendedEvents.Count >= MAX_RECOMMENDATIONS) return;

                var term = search.SearchTerm.ToLower();
                var category = search.Category?.ToLower();

                LogRecommendationStep("Matching", $"Trying to match recent search - Term: {term}, Category: {category}");

                var query = baseQuery.AsQueryable();
                
                if (!string.IsNullOrEmpty(category) && category != "general")
                {
                    query = query.Where(e => 
                        e.Category.ToLower() == category &&
                        (e.Title.ToLower().Contains(term) || e.Description.ToLower().Contains(term))
                    );
                    LogRecommendationStep("Query", "Matching both term and category");
                }
                else
                {
                    query = query.Where(e => 
                        e.Title.ToLower().Contains(term) || 
                        e.Description.ToLower().Contains(term)
                    );
                    LogRecommendationStep("Query", "Matching term only");
                }

                var countBefore = recommendedEvents.Count;
                await AddMatchingEventsToDictionary(
                    query.OrderBy(e => e.Date),
                    recommendedEvents,
                    eventIds,
                    MAX_RECOMMENDATIONS - recommendedEvents.Count,
                    onAdded: e => AddToScoreBuckets(e, string.IsNullOrEmpty(category) || category == "general" ? 3 : 5),
                    onExisting: e => AddToScoreBuckets(e, 1)
                );
                LogRecommendationStep("MatchResult", $"Found {recommendedEvents.Count - countBefore} matches for '{term}'", 
                    new { Term = term, Category = category, Found = recommendedEvents.Count - countBefore });
            }

            await ProcessRecent(recent1);
            await ProcessRecent(recent2);
            await ProcessRecent(recent3);

            // 2. Second priority: Match any recent search terms (broader search)
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS && termsByCount.Count > 0)
            {
                int taken = 0;
                LogRecommendationStep("TermMatching", "Trying to match top unique search terms");
                foreach (var bucket in termsByCount)
                {
                    // Order within bucket by last searched descending without arrays
                    var ordered = bucket.Value.OrderByDescending(t => t.Last);
                    foreach (var entry in ordered)
                    {
                        if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 5) break;
                        var term = entry.Term;
                        if (term.Length < 4 || stopTerms.Contains(term)) { taken++; continue; }
                        var countBefore = recommendedEvents.Count;
                        var query = baseQuery
                            .Where(e => !eventIds.Contains(e.EventId) &&
                                      (e.Title.ToLower().Contains(term) ||
                                       e.Description.ToLower().Contains(term)))
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
                        LogRecommendationStep("TermMatchResult", $"Found {recommendedEvents.Count - countBefore} additional matches for '{term}'",
                            new { Term = term, Found = recommendedEvents.Count - countBefore });
                    }
                    if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 5) break;
                }
            }

            // 3. Third priority: Match any recent categories
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS && catsByCount.Count > 0)
            {
                LogRecommendationStep("CategoryMatching", "Trying to match top categories");
                int taken = 0;
                foreach (var bucket in catsByCount)
                {
                    var ordered = bucket.Value.OrderByDescending(c => c.Last);
                    foreach (var entry in ordered)
                    {
                        if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 3) break;
                        var cat = entry.Cat.ToLower();
                        var countBefore = recommendedEvents.Count;
                        var query = baseQuery
                            .Where(e => !eventIds.Contains(e.EventId) &&
                                      e.Category.ToLower() == cat)
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
                        LogRecommendationStep("CategoryMatchResult", $"Found {recommendedEvents.Count - countBefore} matches for category '{cat}'",
                            new { Category = cat, Found = recommendedEvents.Count - countBefore });
                    }
                    if (recommendedEvents.Count >= MAX_RECOMMENDATIONS || taken >= 3) break;
                }
            }

            // 4. Fallback: Get upcoming events if we still need more
            if (recommendedEvents.Count < MAX_RECOMMENDATIONS)
            {
                var countBefore = recommendedEvents.Count;
                LogRecommendationStep("Fallback", $"Adding {MAX_RECOMMENDATIONS - countBefore} upcoming events as fallback");
                
                await AddMatchingEventsToDictionary(
                    baseQuery.Where(e => !eventIds.Contains(e.EventId))
                            .OrderBy(e => e.Date),
                    recommendedEvents,
                    eventIds,
                    MAX_RECOMMENDATIONS - recommendedEvents.Count,
                    onAdded: e => AddToScoreBuckets(e, 0)
                );
                
                LogRecommendationStep("FallbackResult", $"Added {recommendedEvents.Count - countBefore} upcoming events");
            }

            LogRecommendationStep("Complete", $"Returning {recommendedEvents.Count} recommended events", 
                recommendedEvents.Values.Select(e => new { e.EventId, e.Title, e.Category }));

            // Return ordered by score desc, then date asc
            return scoreBuckets.SelectMany(b => b.Value.Values);
        }

        public async Task LogSearchAsync(int userId, string searchTerm, string category = "General")
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                category = "General";
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogWarning("Attempted to log empty search term for user {UserId}", userId);
                return;
            }

            // Log the attempt with all relevant information
            _logger.LogInformation("=== Starting LogSearchAsync ===");
            _logger.LogInformation("UserId: {UserId}, SearchTerm: {SearchTerm}, Category: {Category}", 
                userId, searchTerm, category);

            try
            {
                // Verify database connection
                _logger.LogInformation("Checking database connection...");
                bool canConnect = await _context.Database.CanConnectAsync();
                _logger.LogInformation("Database connection status: {Status}", canConnect ? "Connected" : "Not Connected");

                // Log the actual DB connection string (path for sqlite)
                try
                {
                    var conn = _context.Database.GetDbConnection();
                    _logger.LogInformation("EF Core ConnectionString: {Conn}", conn?.ConnectionString);
                }
                catch { /* ignore */ }

                if (!canConnect)
                {
                    _logger.LogError("Cannot connect to the database");
                    return;
                }

                var searchHistory = new UserSearchHistory
                {
                    UserId = userId,
                    SearchTerm = searchTerm.Trim().ToLower(),
                    Category = category.Trim().ToLower(),
                    SearchDate = DateTime.Now
                };

                _logger.LogInformation("Created search history entry: {@SearchHistory}", searchHistory);

                _context.UserSearchHistory.Add(searchHistory);
                _logger.LogInformation("Added search history to context. Starting SaveChanges...");
                
                int recordsAffected = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChanges completed. Records affected: {RecordsAffected}", recordsAffected);

                if (recordsAffected > 0)
                {
                    _logger.LogInformation("✅ Successfully saved search history. SearchId: {SearchId}", searchHistory.SearchId);

                    // Verify by reading count for this user (diagnostics only)
                    try
                    {
                        var countForUser = await _context.UserSearchHistory
                            .AsNoTracking()
                            .Where(x => x.UserId == userId)
                            .CountAsync();
                        _logger.LogInformation("Post-save verification: User {UserId} now has {Count} search history rows.", userId, countForUser);
                    }
                    catch (Exception exVerify)
                    {
                        _logger.LogWarning(exVerify, "Post-save verification failed");
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ No records were affected when saving search history");
                    _logger.LogWarning("SearchHistory state: {@SearchHistory}", searchHistory);
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "❌ Database error saving search history. Inner Exception: {InnerException}", 
                    dbEx.InnerException?.Message ?? "No inner exception");
                _logger.LogError("SQL Error: {SqlError}", dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx ? 
                    sqlEx.Message : "N/A");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error in LogSearchAsync");
                _logger.LogError("Error details: {Message}\n{StackTrace}", 
                    ex.Message, ex.StackTrace);
            }
            finally
            {
                _logger.LogInformation("=== Completed LogSearchAsync ===\n");
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
                LogRecommendationStep("AddEvents", "Skipping - maxToAdd is 0 or negative");
                return;
            }

            try
            {
                // Log the query being executed
                var queryString = query.ToQueryString();
                LogRecommendationStep("QueryExecution", $"Executing query for {maxToAdd} events", 
                    new { Query = queryString });

                int addedCount = 0;
                int scanned = 0;
                int targetCount = Math.Min(dictionary.Count + maxToAdd, MAX_RECOMMENDATIONS);

                await foreach (var evt in query.AsAsyncEnumerable())
                {
                    if (dictionary.Count >= targetCount) 
                    {
                        LogRecommendationStep("AddEvents", $"Reached target count of {targetCount}, stopping");
                        break;
                    }

                    scanned++;
                    if (dictionary.Count >= targetCount) 
                    {
                        LogRecommendationStep("AddEvents", $"Reached target count of {targetCount}, stopping");
                        break;
                    }

                    if (eventIds.Add(evt.EventId))
                    {
                        try
                        {
                            // Create a unique key using date + event ID to handle events at the same time
                            // Add a small random tick to ensure unique keys even for same-date events
                            var random = new Random().Next(1, 100);
                            var key = evt.Date.AddTicks(evt.EventId + random);
                            
                            // Ensure we don't have a key collision (should be extremely rare)
                            while (dictionary.ContainsKey(key))
                            {
                                key = key.AddTicks(1);
                            }
                            
                            dictionary[key] = evt;
                            addedCount++;
                            onAdded?.Invoke(evt);
                            
                            LogRecommendationStep("EventAdded", $"Added event to recommendations: {evt.Title}", 
                                new { 
                                    EventId = evt.EventId, 
                                    Title = evt.Title,
                                    Date = evt.Date,
                                    Category = evt.Category,
                                    CurrentTotal = dictionary.Count,
                                    TargetCount = targetCount,
                                    MaxRecommendations = MAX_RECOMMENDATIONS
                                });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding event to dictionary", 
                                new { EventId = evt?.EventId, Title = evt?.Title });
                        }
                    }
                    else
                    {
                        LogRecommendationStep("EventSkipped", "Event already in recommendations", 
                            new { EventId = evt.EventId, Title = evt.Title });
                        onExisting?.Invoke(evt);
                    }
                }

                LogRecommendationStep("AddEventsComplete", $"Added {addedCount} new events to recommendations", 
                    new { Added = addedCount, Total = dictionary.Count, Scanned = scanned });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddMatchingEventsToDictionary");
                LogRecommendationStep("AddEventsError", $"Error: {ex.Message}", 
                    new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}