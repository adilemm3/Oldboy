using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Entities;

namespace BarberShop.DataStorages.Interfaces
{
    public interface IBarberShopStorage
    {
        IRepository<Master> Masters { get; }
        IRepository<Client> Clients { get; }
        IRepository<Visit> Visits { get; }
        IRepository<Service> Services { get; }
        IRepository<MasterServices> MasterServices { get; }
        IRepository<ServiceInVisit> ServicesInVisit { get; }
        IRepository<User> Users { get; }
        int Save();

    }
}
