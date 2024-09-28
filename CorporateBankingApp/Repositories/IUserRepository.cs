﻿using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System.Collections.Generic;

namespace CorporateBankingApp.Repositories
{
    public interface IUserRepository
    {
        User LoginActivity(User user);

        User GetUserByUsername(string username);

        void CreateNewClient(Client client);

        //void AdminRegistration(Admin admin, Role role);
    }

}
