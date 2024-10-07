using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using Microsoft.AspNetCore.Http;

namespace CorporateBankingApp.Services
{
    public interface IUserService
    {
        string LoginActivity(UserDTO userDTO);
        User GetUserByUsername(string username);
        bool EmailExists(string email);
        bool AccountNumberExists(string accountNumber);
        bool IFSCExists(string ifscCode);
        void CreateNewClient(ClientDTO clientDTO, IList<HttpPostedFileBase> files);
    }


}
