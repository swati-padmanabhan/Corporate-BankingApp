using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using CorporateBankingApp.DTOs;
using Microsoft.AspNetCore.Http;

namespace CorporateBankingApp.Services
{
    public interface IUserService
    {
       string LoginActivity(UserDTO userDTO);
       User GetUserByUsername(string username);
    }

}
