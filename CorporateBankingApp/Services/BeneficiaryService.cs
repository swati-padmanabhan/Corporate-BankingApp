using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
        }

        public Beneficiary GetBeneficiaryById(Guid id)
        {
            return _beneficiaryRepository.GetBeneficiaryById(id);
        }

        //public void AddOutboudDetails(BeneficiaryDTO beneficiaryDTO, Client client)
        //{
        //    var employee = new BeneficiaryDTO
        //    {
        //        Id = Guid.NewGuid(),
        //        BeneficiaryName = beneficiaryDTO.BeneficiaryName,
        //        AccountNumber = beneficiaryDTO.AccountNumber,

        //        BankIFSC = beneficiaryDTO.BankIFSC,
        //        BeneficiaryType = BeneficiaryType.OUTBOUND,
        //        OnBoardingStatus = Status.PENDING,
        //        IsActive = true,
        //        Client = client
        //    };
        //    _beneficiaryRepository.AddBeneficiaryDetails(employee);
        //}

        public List<BeneficiaryDTO> GetAllBeneficiaries(Guid clientId, UrlHelper urlHelper)
        {
            var beneficiaries = _beneficiaryRepository.GetAllBeneficiaries(clientId);
            var beneficiariesDto = beneficiaries.Select(b => new BeneficiaryDTO
            {
                Id = b.Id,
                BeneficiaryName = b.BeneficiaryName,
                AccountNumber = b.AccountNumber,
                BankIFSC = b.BankIFSC,
                BeneficiaryStatus = b.BeneficiaryStatus.ToString().ToUpper(),
                BeneficiaryType = b.BeneficiaryType.ToString().ToUpper(),
                IsActive = b.IsActive,
                DocumentUrls = b.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return beneficiariesDto;
        }


        public void AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles)
        {
            var beneficiary = new Beneficiary()
            {
                Id = Guid.NewGuid(),
                BeneficiaryName = beneficiaryDTO.BeneficiaryName,
                AccountNumber = beneficiaryDTO.AccountNumber,
                BankIFSC = beneficiaryDTO.BankIFSC,
                BeneficiaryType = BeneficiaryType.OUTBOUND,
                BeneficiaryStatus = CompanyStatus.PENDING,
                IsActive = true,
                Client = client
            };
            string[] documentTypes = { "Beneficiary Id Proof", "Beneficiary Address Proof" };
            string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/Beneficiary/") + beneficiary.BeneficiaryName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            for (int i = 0; i < uploadedFiles.Count; i++)
            {

                var file = uploadedFiles[i];
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(filePath);
                    // Save relative file path (relative to the Content folder)
                    string relativeFilePath = $"~/Content/Documents/Beneficiary/{beneficiary.BeneficiaryName}/{fileName}";
                    var document = new Document
                    {
                        DocumentType = documentTypes[i], // Get document type based on index
                        FilePath = relativeFilePath,
                        UploadDate = DateTime.Now,
                        Beneficiary = beneficiary
                    };
                    beneficiary.Documents.Add(document);
                }
            }
            _clientRepository.AddNewBeneficiary(beneficiary);
        }

        public void UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            _beneficiaryRepository.UpdateBeneficiaryStatus(id, isActive);
        }


    }
}