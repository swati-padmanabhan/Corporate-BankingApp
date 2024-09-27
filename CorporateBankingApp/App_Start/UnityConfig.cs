using CorporateBankingApp.Data;
using CorporateBankingApp.Repositories;
using CorporateBankingApp.Services;
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

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<ISession>(new InjectionFactory(c => NHibernateHelper.CreateSession()));

            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            

            container.RegisterType<IClientService, ClientService>();
            container.RegisterType<IClientRepository, ClientRepository>();

            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IAdminRepository, AdminRepository>();

            container.RegisterType<ICloudinaryService, CloudinaryService>();


            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}