using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.DataStorages;
using BarberShop.Entities;
using BarberShop.DataStorages.Interfaces;

namespace BarberShop.DataStorages
{
    public class BarberShopStorage : IDisposable, IBarberShopStorage
    {
        private readonly BarberShopContext _context;

        public BarberShopStorage(BarberShopContext context)
        {
            _context = context;

            Masters = new EfRepository<Master>(_context);
            Clients = new EfRepository<Client>(_context);
            Visits = new EfRepository<Visit>(_context);
            Services = new EfRepository<Service>(_context);
            MasterServices = new EfRepository<MasterServices>(_context);
            ServicesInVisit = new EfRepository<ServiceInVisit>(_context);
            Users = new EfRepository<User>(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public IRepository<Master> Masters { get; }

        public IRepository<Client> Clients { get; }

        public IRepository<Visit> Visits { get; }

        public IRepository<Service> Services { get; }

        public IRepository<MasterServices> MasterServices { get; }

        public IRepository<ServiceInVisit> ServicesInVisit { get; }
        public IRepository<User> Users { get; }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
