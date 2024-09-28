using CorporateBankingApp.Models;
using CorporateBankingApp.Utils;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ISession _session;

        public AdminRepository(ISession session)
        {
            _session = session;
        }

        public void CreateAdmin(Admin admin)
        {
            using (var transaction = _session.BeginTransaction())
            {
                admin.Password = PasswordHashing.HashPassword(admin.Password);
                var role = new Role
                {
                    RoleName = "Admin",
                    User = admin
                };
                _session.Save(admin);
                _session.Save(role);
                transaction.Commit();
            }
        }
    

    public Client GetClientById(Guid id)
        {
            return _session.Get<Client>(id);
        }

        public void DeleteClientDetails(Guid id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var currentClient = _session.Get<Client>(id);

                if (currentClient != null)
                {
                    currentClient.IsActive = false;
                }
                _session.Update(currentClient);
                transaction.Commit();
            }
        }

        public List<Client> GetAllClients()
        {
            var clients = _session.Query<Client>().Where(c => c.IsActive == true).ToList();
            return clients;
        }

        public void UpdateClientDetails(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var existingClient = _session.Get<Client>(client.Id);

                if (existingClient != null)
                {
                    existingClient.UserName = client.UserName;
                    existingClient.Email = client.Email;
                    existingClient.CompanyName = client.CompanyName;
                    existingClient.Location = client.Location;
                    existingClient.ContactInformation = client.ContactInformation;
                }
                _session.Update(existingClient);
                transaction.Commit();
            }
        }
    }
}