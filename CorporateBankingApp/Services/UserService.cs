using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using System.Web.Mvc;
using System.Web;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using Microsoft.AspNetCore.Http;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Utils;

namespace CorporateBankingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
       

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
           
        }

        public string LoginActivity(UserDTO userDTO)
        {
            string role;
            var user = new User { UserName = userDTO.UserName, Password = userDTO.Password };
            var currentUser = _userRepository.LoginActivity(user);
            if (currentUser != null)
            {
                if (currentUser.Role.RoleName == "Admin")
                {
                    return role = "Admin";
                }
                else if (currentUser.Role.RoleName == "Client")
                {
                    return role = "Client";
                }
            }
            return role = null;
        }

        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public bool EmailExists(string email)
        {
            return _userRepository.EmailExists(email);
        }

        public bool AccountNumberExists(string accountNumber)
        {
            return _userRepository.AccountNumberExists(accountNumber);
        }

        public bool IFSCExists(string ifscCode)
        {
            return _userRepository.IFSCExists(ifscCode);
        }

        public void CreateNewClient(ClientDTO clientDTO, IList<HttpPostedFileBase> files)
            {
                var client = new Client()
                {
                    UserName = clientDTO.UserName,
                    Password = clientDTO.Password,
                    Email = clientDTO.Email,
                    CompanyName = clientDTO.CompanyName,
                    ContactInformation = clientDTO.ContactInformation,
                    Location = clientDTO.Location,
                    AccountNumber = clientDTO.AccountNumber,
                    ClientIFSC = clientDTO.ClientIFSC,
                    Balance = clientDTO.Balance,
                    OnBoardingStatus = CompanyStatus.PENDING,
                    IsActive = true
                };

                string[] documentTypes = { "Company Id Proof", "Address Proof" };
                string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/ClientRegistration/") + client.UserName;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                for (int i = 0; i < files.Count; i++)
                {

                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(folderPath, fileName);
                        file.SaveAs(filePath);
                        // Save relative file path (relative to the Content folder)
                        string relativeFilePath = $"~/Content/Documents/ClientRegistration/{client.UserName}/{fileName}";
                        var document = new Document
                        {
                            DocumentType = documentTypes[i], // Get document type based on index
                            FilePath = relativeFilePath,
                            UploadDate = DateTime.Now,
                            Client = client
                        };

                        client.Documents.Add(document);
                    }
                }

                _userRepository.CreateNewClient(client);
            }



    }
}