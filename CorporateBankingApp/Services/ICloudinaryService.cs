using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CorporateBankingApp.Services
{
    public  interface ICloudinaryService
    {
        string UploadDocument(HttpPostedFileBase file);


    }
}
