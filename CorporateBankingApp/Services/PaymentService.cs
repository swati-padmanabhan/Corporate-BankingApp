using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void CreatePayment(Guid clientId, Beneficiary beneficiary, double amount, string razorpayPaymentId)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                Beneficiary = beneficiary,
                Amount = amount,
                PaymentRequestDate = DateTime.Now,
                RazorpayPaymentId = razorpayPaymentId,
                PaymentStatus = CompanyStatus.PENDING
            };

            _paymentRepository.Save(payment);
        }

        public void UpdatePaymentStatus(string razorpayOrderId, CompanyStatus status)
        {
            var payment = _paymentRepository.GetByRazorpayOrderId(razorpayOrderId);
            if (payment != null)
            {
                payment.PaymentStatus = status;
                payment.PaymentApprovalDate = DateTime.Now;
                _paymentRepository.Update(payment);
            }
        }
        public void ApprovePayment(Payment payment)
        {
            payment.PaymentApprovalDate = DateTime.Now; // Set the approval date to the current date and time
            _paymentRepository.Save(payment); // Save the payment
        }
    }
}