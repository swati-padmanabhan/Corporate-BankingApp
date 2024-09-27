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


        //public void CreateNewClient(ClientDTO clientDto, IList<HttpPostedFileBase> uploadedFiles)
        //{
        //    var client = new Client()
        //    {
        //        UserName = clientDto.UserName,
        //        Password = clientDto.Password,
        //        Email = clientDto.Email,
        //        CompanyName = clientDto.CompanyName,
        //        ContactInformation = clientDto.ContactInformation,
        //        Location = clientDto.Location,
        //        OnBoardingStatus = Status.PENDING,
        //        IsActive = true,
        //        Documents = new List<Document>()

        //    };

        //    string[] documentTypes = { "Company Id Proof", "Address Proof" };

        //    for (int i = 0; i < uploadedFiles.Count; i++)
        //    {

        //        var file = uploadedFiles[i];
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            string folderPath = HttpContext.Current.Server.MapPath("~/Documents/ClientRegistration/") + client.UserName;
        //            if (!Directory.Exists(folderPath))
        //            {
        //                Directory.CreateDirectory(folderPath);
        //            }
        //            string filePath = Path.Combine(folderPath, file.FileName);
        //            file.SaveAs(filePath);

        //            var document = new Document
        //            {
        //                DocumentType = documentTypes[i], // Get document type based on index
        //                FilePath = filePath,
        //                UploadDate = DateTime.Now,
        //                Client = client
        //            };

        //            client.Documents.Add(document);
        //        }
        //    }

        //    _userRepository.AddingNewClient(client);
        //}


        //public void CreateNewClient(ClientDTO clientDto, IList<HttpPostedFileBase> uploadedFiles)
        //{
        //    var client = new Client()
        //    {
        //        UserName = clientDto.UserName,
        //        Password = clientDto.Password,
        //        Email = clientDto.Email,
        //        CompanyName = clientDto.CompanyName,
        //        ContactInformation = clientDto.ContactInformation,
        //        Location = clientDto.Location,
        //        OnBoardingStatus = Status.PENDING,
        //        IsActive = true,
        //        Documents = new List<Document>() // Ensure initialization
        //    };

        //    string[] documentTypes = { "Company Id Proof", "Address Proof" };

        //    for (int i = 0; i < uploadedFiles.Count; i++)
        //    {
        //        var file = uploadedFiles[i];
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            string folderPath = HttpContext.Current.Server.MapPath("~/Documents/ClientRegistration/") + client.UserName;
        //            if (!Directory.Exists(folderPath))
        //            {
        //                Directory.CreateDirectory(folderPath);
        //            }
        //            string filePath = Path.Combine(folderPath, file.FileName);
        //            file.SaveAs(filePath);

        //            var document = new Document
        //            {
        //                DocumentType = documentTypes[i],
        //                FilePath = filePath,
        //                UploadDate = DateTime.Now,
        //                Client = client
        //            };

        //            // Add the document to the client's documents collection
        //            client.Documents.Add(document);
        //        }
        //    }

        //    _userRepository.AddingNewClient(client);
        //}







    }
}