using System.Collections.Generic;
using System.Linq;
using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Utils;
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
            //var currentUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            var currentUser = _session.Query<User>().FirstOrDefault(u => u.UserName == user.UserName);
            if (currentUser != null && PasswordHashing.VerifyPassword(user.Password, currentUser.Password))
            {
                return currentUser;
            }
            return null;
        }

        public User GetUserByUsername(string username)
        {
            return _session.Query<User>().FirstOrDefault(u =>u.UserName == username);
        }

        public void CreateNewClient(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                client.Password = PasswordHashing.HashPassword(client.Password);
                var role = new Role()
                {
                    RoleName = "Client",
                    User = client
                };
                _session.Save(client);
                _session.Save(role);
                transaction.Commit();
            }
        }


        public bool EmailExists(string email)
        {
            return _session.Query<Client>().Any(c => c.Email == email);
        }

        public bool AccountNumberExists(string accountNumber)
        {
            return _session.Query<Client>().Any(c => c.AccountNumber == accountNumber);
        }

        // Check if IFSC code exists
        public bool IFSCExists(string ifscCode)
        {
            return _session.Query<Client>().Any(c => c.ClientIFSC == ifscCode);
        }

    }

}