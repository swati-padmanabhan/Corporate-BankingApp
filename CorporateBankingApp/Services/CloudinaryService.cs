using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Services
{
    public class CloudinaryService:ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var cloudName = System.Configuration.ConfigurationManager.AppSettings["CloudinaryCloudName"];
            var apiKey = System.Configuration.ConfigurationManager.AppSettings["CloudinaryApiKey"];
            var apiSecret = System.Configuration.ConfigurationManager.AppSettings["CloudinaryApiSecret"];

            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public string UploadDocument(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return null;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.InputStream),
                PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                Overwrite = true,
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            return uploadResult.SecureUrl.ToString(); // Returns the URL of the uploaded file
        }

    }
}