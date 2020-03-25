using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BarberShop.DataStorages;
using BarberShop.Entities;
using BarberShop.DataStorages.Interfaces;
using BarberShop.Services.interfaces;
using BarberShop.Services;

namespace BarberShop.Utilities
{
    public static class ServicesExtensions
    {
        public static void AddDataStorages(this IServiceCollection services)
        {
            services.AddScoped<IBarberShopStorage, BarberShopStorage>();
        }

        private static void AddCrudServices(this IServiceCollection services)
        {
            services.AddScoped<ICrudService<Master>, MasterService>();
            services.AddScoped<ICrudService<Client>,ClientService>();
            services.AddScoped<IMasterServiceCrudService<MasterServices>,MasterServiceServices>();
            services.AddScoped<IServiceInVisitCrudService<ServiceInVisit>,ServiceInVisitService>();
            services.AddScoped<ICrudService<Service>,ServiceService>();
            services.AddScoped<ICrudService<Visit>,VisitService>();
            services.AddScoped<ICrudService<User>, UserService>();
        }



        private static void AddRepoServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Master>, EfRepository<Master>>();
            services.AddScoped<IRepository<Client>, EfRepository<Client>>();
            services.AddScoped<IRepository<MasterServices>, EfRepository<MasterServices>>();
            services.AddScoped<IRepository<ServiceInVisit>, EfRepository<ServiceInVisit>>();
            services.AddScoped<IRepository<Service>, EfRepository<Service>>();
            services.AddScoped<IRepository<Visit>, EfRepository<Visit>>();
            services.AddScoped<IRepository<User>, EfRepository<User>>();
        }

        public static void InjectDependencies(this IServiceCollection services)
        {
            services.AddCrudServices();
            services.AddRepoServices();
            services.AddDataStorages();
        }
    }
}

