using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CorporateBankingApp.Services
{
    public interface IAdminService
    {
        void RegisterAdmin(AdminDTO adminDTO);

        List<ClientDTO> GetRegisteredClientsPendingApproval();
        ClientDTO GetClientById(Guid id);
        void ApproveClient(Guid id);
        void RejectClient(Guid id);
        List<Client> GetAllClients();
        void UpdateClientDetails(ClientDTO clientDTO, Guid id);
        void DeleteClientDetails(Guid id);
    }
}
