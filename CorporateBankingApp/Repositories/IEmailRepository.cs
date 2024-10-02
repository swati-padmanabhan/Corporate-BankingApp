using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IEmailRepository
    {
        void SendEmailNotification(string to, string subject, string body);
    }
}
