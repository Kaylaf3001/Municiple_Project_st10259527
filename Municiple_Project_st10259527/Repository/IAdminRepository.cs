using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repositories
{
    public interface IAdminRepository
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
    }
}
