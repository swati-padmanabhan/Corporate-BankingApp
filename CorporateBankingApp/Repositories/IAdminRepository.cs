using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IAdminRepository
    {
        Client GetClientById(Guid id);
        List<Client> GetAllClients();
        void UpdateClientDetails(Client client);
        void DeleteClientDetails(Guid id);
    }
}
