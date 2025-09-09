using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Repository
{
    public interface IUserRepository
    {
        UserModel GetUserByEmailAndPassword(string email, string password);
        bool UserExists(string email);
        void AddUser(UserModel user);
        UserModel GetUserById(int userId);
        IEnumerable<UserModel> GetAllUsers();
    }
}
