using CorporateBankingApp.Data;
using CorporateBankingApp.Repositories;
using CorporateBankingApp.Services;
using Microsoft.Extensions.Configuration;
using NHibernate;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace CorporateBankingApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Register NHibernate session
            container.RegisterType<ISession>(new InjectionFactory(c => NHibernateHelper.CreateSession()));

            // Register services and repositories
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserRepository, UserRepository>();

            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IEmailRepository, EmailRepository>();

            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IAdminRepository, AdminRepository>();

            container.RegisterType<IClientService, ClientService>();
            container.RegisterType<IClientRepository, ClientRepository>();

            container.RegisterType<IBeneficiaryService, BeneficiaryService>();
            container.RegisterType<IBeneficiaryRepository, BeneficiaryRepository>();

            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IPaymentRepository, PaymentRepository>();


            // Set the dependency resolver for MVC
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
