using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Entities;

namespace BarberShop.Services
{
    public interface IMasterServiceCrudService<MasterServices>
    {
        MasterServices Get(Guid masterId, Guid serviceId);
        List<MasterServices> GetByMasterId(Guid masterId);
        List<MasterServices> GetAll();
        MasterServices Create(MasterServices resource);
        MasterServices Update(MasterServices resource);
        MasterServices UpdateNew(MasterServices resource);
        MasterServices Delete(Guid masterId, Guid serviceId);
    }
}
