﻿using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ISession _session;

        public PaymentRepository(ISession session)
        {
            _session = session;
        }



        public void Save(Payment payment)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(payment);
                transaction.Commit();
            }
        }

        public Payment GetByRazorpayOrderId(string orderId)
        {
            return _session.QueryOver<Payment>()
                .Where(p => p.RazorpayPaymentId == orderId)
                .SingleOrDefault();
        }

        public void Update(Payment payment)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(payment);
                transaction.Commit();
            }
        }
    }
}