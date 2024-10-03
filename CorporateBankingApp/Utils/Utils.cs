using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Utils
{
    public class Utils
    {
        public static void verifyPaymentSignature(Dictionary<string, string> attributes)
        {
            RazorpayClient client = new RazorpayClient(
                System.Configuration.ConfigurationManager.AppSettings["RazorpayKey"],
                System.Configuration.ConfigurationManager.AppSettings["RazorpaySecret"]
            );

            Utils.verifyPaymentSignature(attributes);
        }
    }
}