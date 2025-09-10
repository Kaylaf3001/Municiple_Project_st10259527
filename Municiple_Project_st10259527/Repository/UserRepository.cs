using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public UserModel GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public HashSet<string> GetAllUserEmails()
        {
            return _context.Users.Select(u => u.Email).ToHashSet();
        }

        public void AddUser(UserModel user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public UserModel GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
