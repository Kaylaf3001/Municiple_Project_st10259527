using System;
using System.Threading.Tasks;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;

namespace Municiple_Project_st10259527.Services
{
    public class DefaultAdminService
    {
        private readonly IUserRepository _userRepository;
        private const string DefaultAdminEmail = "admin@municipal.gov";
        private const string DefaultAdminPassword = "Admin@123"; // In production, use secure password management

        public DefaultAdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateDefaultAdminIfNotExists()
        {
            var adminExists = _userRepository.UserExists(DefaultAdminEmail);
            if (!adminExists)
            {
                var adminUser = new UserModel
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = DefaultAdminEmail,
                    Password = DefaultAdminPassword, // In production, hash the password
                    IsAdmin = true
                };

                _userRepository.AddUser(adminUser);
                await _userRepository.SaveChangesAsync();
            }
        }
    }
}
