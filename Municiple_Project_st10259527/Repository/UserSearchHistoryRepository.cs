using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using Microsoft.EntityFrameworkCore;

namespace Municiple_Project_st10259527.Repository
{
    public class UserSearchHistoryRepository : IUserSearchHistoryRepository
    {
        private readonly AppDbContext _context;

        public UserSearchHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSearchAsync(UserSearchHistory search)
        {
            if (search == null) throw new ArgumentNullException(nameof(search));

            await _context.UserSearchHistory.AddAsync(search);
            await _context.SaveChangesAsync();
        }

        // Return searches as IAsyncEnumerable (no list/array needed)
        public async IAsyncEnumerable<UserSearchHistory> GetRecentSearchesAsync(int userId, int maxCount)
        {
            await foreach (var s in _context.UserSearchHistory
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SearchDate)
                .Take(maxCount)
                .AsAsyncEnumerable())
            {
                yield return s;
            }
        }
    }
}
