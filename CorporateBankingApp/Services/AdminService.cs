using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;  

namespace CorporateBankingApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public List<ClientDTO> GetRegisteredClientsPendingApproval()
        {
            var clients = _adminRepository.GetAllClients()
                .Where(c => c.OnBoardingStatus == Status.PENDING && c.IsActive).ToList();
            return clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                CompanyName = c.CompanyName,
                Location = c.Location,
                ContactInformation = c.ContactInformation
            }).ToList();
        }

        public ClientDTO GetClientById(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            return new ClientDTO
            {
                Id = client.Id,
                UserName = client.UserName,
                Email = client.Email,
                CompanyName = client.CompanyName,
                Location = client.Location,
                ContactInformation = client.ContactInformation
            };
        }

        public void ApproveClient(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            client.OnBoardingStatus = Status.APPROVED;
            _adminRepository.UpdateClientDetails(client);
        }

        public void RejectClient(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            client.IsActive = false; // Set inactive if rejected
            _adminRepository.UpdateClientDetails(client);
        }

        public void DeleteClientDetails(Guid id)
        {
            _adminRepository.DeleteClientDetails(id);
        }

        public List<Client> GetAllClients()
        {
            var clients = _adminRepository.GetAllClients();
            return clients;
        }

        public void UpdateClientDetails(ClientDTO clientDTO, Guid id)
        {
            var client = new Client
            {
                Id = id,
                UserName = clientDTO.UserName,
                Email = clientDTO.Email,
                CompanyName = clientDTO.CompanyName,
                Location = clientDTO.Location,
                ContactInformation = clientDTO.ContactInformation,
            };
            _adminRepository.UpdateClientDetails(client);
        }
    }
}
