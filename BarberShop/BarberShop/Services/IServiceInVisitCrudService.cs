using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Entities;

namespace BarberShop.Services.interfaces
{
    public interface IServiceInVisitCrudService<ServiceInVisit>
    {
        ServiceInVisit Get(Guid masterId, Guid serviceId,Guid visitId);
        List<ServiceInVisit> GetAll();
        ServiceInVisit Create(ServiceInVisit resource);
        ServiceInVisit Update(ServiceInVisit resource);
        ServiceInVisit Delete(Guid serviceInVisitId);
        List<ServiceInVisit> GetInVisit(Guid visitId);
        ServiceInVisit GetById(Guid serviceInVisitId);
    }
}
