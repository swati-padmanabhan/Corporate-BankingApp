using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Services
{
    public interface IEmailService
    {
        void SendOnboardingAcceptanceEmail(string clientEmail);
        void SendOnboardingRejectionEmail(string clientEmail, string reason);
    }
}
