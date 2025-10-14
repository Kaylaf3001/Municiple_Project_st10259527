using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Services
{
    public class RecommendationService
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IUserSearchHistoryRepository _searchRepository;
        private const int MaxRecommendations = 5;
        private const int MaxSearchHistory = 10;

        public RecommendationService(
            IEventsRepository eventsRepository,
            IUserSearchHistoryRepository searchRepository)
        {
            _eventsRepository = eventsRepository;
            _searchRepository = searchRepository;
        }

        public async Task<Stack<EventModel>> GetRecommendedEventsAsync(int userId)
        {
            var searches = new Stack<UserSearchHistory>();

            await foreach (var s in _searchRepository.GetRecentSearchesAsync(userId, MaxSearchHistory))
                searches.Push(s);

            var upcomingQueue = await _eventsRepository.GetUpcomingEventsQueueAsync();

            if (!searches.Any())
            {
                var fallbackStack = new Stack<EventModel>();
                foreach (var evt in upcomingQueue)
                    fallbackStack.Push(evt);
                return fallbackStack;
            }

            var eventScores = new Dictionary<int, (EventModel Event, int Score)>();

            foreach (var search in searches)
                await ApplySearchToScoresAsync(search, eventScores, upcomingQueue);

            var sortedStack = new Stack<EventModel>(
                eventScores
                    .OrderByDescending(x => x.Value.Score)
                    .ThenBy(x => x.Value.Event.Date)
                    .Select(x => x.Value.Event)
                    .Take(MaxRecommendations)
                    .Reverse() // so highest score is on top of stack
            );

            return sortedStack;
        }


        private async Task ApplySearchToScoresAsync(UserSearchHistory search, Dictionary<int, (EventModel Event, int Score)> eventScores, Queue<EventModel> upcomingQueue)
        {
            string term = (search.SearchTerm ?? string.Empty).Trim().ToLowerInvariant();
            string category = (search.Category ?? "General").Trim().ToLowerInvariant();

            foreach (var evt in upcomingQueue)
            {
                if (((evt.Title ?? "").ToLower().Contains(term) ||
                     (evt.Description ?? "").ToLower().Contains(term)) &&
                    (category == "general" || (evt.Category ?? "").ToLower() == category))
                {
                    int baseScore = category == "general" ? 3 : 5;

                    if (eventScores.TryGetValue(evt.EventId, out var existing))
                        eventScores[evt.EventId] = (evt, existing.Score + baseScore);
                    else
                        eventScores[evt.EventId] = (evt, baseScore);
                }
            }
        }

    }
}