using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repository
{
    public interface IUserSearchHistoryRepository
    {
        Task AddSearchAsync(UserSearchHistory search);
        IAsyncEnumerable<UserSearchHistory> GetRecentSearchesAsync(int userId, int maxCount);
    }
}
