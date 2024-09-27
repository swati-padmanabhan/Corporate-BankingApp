using System.Collections.Generic;
using System.Linq;
using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using NHibernate;

namespace CorporateBankingApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session;
        }

        public User LoginActivity(User user)
        {
            var currentUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            //if (currentUser != null && PasswordHashing.VerifyPassword(user.Password, currentUser.Password))
            //{
            //    return currentUser;
            //}
            //return null;
            return currentUser;
        }

        public User GetUserByUsername(string username)
        {
            return _session.Query<User>().FirstOrDefault(u =>u.UserName == username);
        }
    }

}