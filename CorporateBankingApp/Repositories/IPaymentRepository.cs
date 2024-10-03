using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IPaymentRepository
    {

        void Save(Payment payment);

        Payment GetByRazorpayOrderId(string orderId);

        void Update(Payment payment);
    }
}
