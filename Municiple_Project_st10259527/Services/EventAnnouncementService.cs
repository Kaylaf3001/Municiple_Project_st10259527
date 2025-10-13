using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Services
{
    public class EventAnnouncementService
    {
        // Indexes for fast lookups
        private readonly Dictionary<int, EventModel> _eventsById = new();
        private readonly Dictionary<int, AnnouncementModel> _announcementsById = new();

        // Indexes for filtering
        private readonly Dictionary<DateTime, HashSet<int>> _eventsByDate = new();
        private readonly Dictionary<string, HashSet<int>> _eventsByCategory = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, HashSet<int>> _eventsBySearchTerm = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, HashSet<int>> _announcementsByDate = new();

        public void Initialize(IEnumerable<EventModel> events, IEnumerable<AnnouncementModel> announcements)
        {
            // Clear existing data
            _eventsById.Clear();
            _announcementsById.Clear();
            _eventsByDate.Clear();
            _eventsByCategory.Clear();
            _eventsBySearchTerm.Clear();
            _announcementsByDate.Clear();

            // Index events
            foreach (var evt in events)
            {
                _eventsById[evt.EventId] = evt;

                // Index by date
                var eventDate = evt.Date.Date;
                if (!_eventsByDate.ContainsKey(eventDate))
                    _eventsByDate[eventDate] = new HashSet<int>();
                _eventsByDate[eventDate].Add(evt.EventId);

                // Index by category
                if (!string.IsNullOrEmpty(evt.Category))
                {
                    if (!_eventsByCategory.ContainsKey(evt.Category))
                        _eventsByCategory[evt.Category] = new HashSet<int>();
                    _eventsByCategory[evt.Category].Add(evt.EventId);
                }

                // Index by search terms (title and description)
                IndexSearchTerms(evt.EventId, evt.Title, evt.Description);
            }

            // Index announcements
            foreach (var announcement in announcements)
            {
                _announcementsById[announcement.AnnouncementId] = announcement;

                // Index by date
                var announcementDate = announcement.Date.Date;
                if (!_announcementsByDate.ContainsKey(announcementDate))
                    _announcementsByDate[announcementDate] = new HashSet<int>();
                _announcementsByDate[announcementDate].Add(announcement.AnnouncementId);
            }
        }

        private void IndexSearchTerms(int eventId, params string[] texts)
        {
            foreach (var text in texts.Where(t => !string.IsNullOrEmpty(t)))
            {
                var words = text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    var normalizedWord = word.ToLowerInvariant();
                    if (!_eventsBySearchTerm.ContainsKey(normalizedWord))
                        _eventsBySearchTerm[normalizedWord] = new HashSet<int>();
                    _eventsBySearchTerm[normalizedWord].Add(eventId);
                }
            }
        }

        public (IEnumerable<EventModel> Events, IEnumerable<AnnouncementModel> Announcements)
            Filter(EventAnnouncementFilter filter)
        {
            var eventIds = new HashSet<int>(_eventsById.Keys);
            var announcementIds = new HashSet<int>(_announcementsById.Keys);

            // Apply event filters
            var filteredEventIds = FilterEvents(eventIds, filter);
            var filteredAnnouncementIds = FilterAnnouncements(announcementIds, filter);

            return (
                filteredEventIds.Select(id => _eventsById[id]).OrderBy(e => e.Date),
                filteredAnnouncementIds.Select(id => _announcementsById[id]).OrderByDescending(a => a.Date)
            );
        }

        private IEnumerable<int> FilterEvents(HashSet<int> eventIds, EventAnnouncementFilter filter)
        {
            var result = new HashSet<int>(eventIds);

            // Apply date filter
            if (filter.StartDate.HasValue || filter.EndDate.HasValue)
            {
                var startDate = filter.StartDate?.Date ?? DateTime.MinValue;
                var endDate = filter.EndDate?.Date ?? DateTime.MaxValue;

                var dateFilteredIds = _eventsByDate
                    .Where(kvp => kvp.Key >= startDate && kvp.Key <= endDate)
                    .SelectMany(kvp => kvp.Value)
                    .ToHashSet();

                result.IntersectWith(dateFilteredIds);
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(filter.Category) &&
                _eventsByCategory.TryGetValue(filter.Category, out var categoryIds))
            {
                result.IntersectWith(categoryIds);
            }

            // Apply search term filter
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerms = filter.SearchTerm.ToLowerInvariant()
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var searchResults = searchTerms
                    .Where(term => _eventsBySearchTerm.ContainsKey(term))
                    .SelectMany(term => _eventsBySearchTerm[term])
                    .ToHashSet();

                // If we have search terms but no results, return empty set
                if (searchTerms.Any() && searchResults.Count == 0)
                    return Enumerable.Empty<int>();

                result.IntersectWith(searchResults);
            }

            return result;
        }

        private IEnumerable<int> FilterAnnouncements(HashSet<int> announcementIds, EventAnnouncementFilter filter)
        {
            var result = new HashSet<int>(announcementIds);

            // Apply date filter
            if (filter.StartDate.HasValue || filter.EndDate.HasValue)
            {
                var startDate = filter.StartDate?.Date ?? DateTime.MinValue;
                var endDate = filter.EndDate?.Date ?? DateTime.MaxValue;

                var dateFilteredIds = _announcementsByDate
                    .Where(kvp => kvp.Key >= startDate && kvp.Key <= endDate)
                    .SelectMany(kvp => kvp.Value)
                    .ToHashSet();

                result.IntersectWith(dateFilteredIds);
            }

            // For announcements, we can only filter by date in this implementation
            return result;
        }

        public IEnumerable<string> GetCategories() =>
            _eventsByCategory.Keys.OrderBy(k => k);
    }

    public class EventAnnouncementFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Category { get; set; }
        public string SearchTerm { get; set; }
    }
}