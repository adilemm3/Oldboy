using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using BarberShop.Exeptions.Throws;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using BarberShop.Services.interfaces;
using System.Collections.Generic;

namespace BarberShop.Services
{
    public class ServiceInVisitService : IServiceInVisitCrudService<ServiceInVisit>
    {
        private readonly IBarberShopStorage _storage;

        public ServiceInVisitService(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public ServiceInVisit Create(ServiceInVisit resource)
        {
            if (ContainsInStorage(resource)) throw new ResourceAlreadyExistException($"Данная выбранная услуга уже существует");
            resource.Id = Guid.NewGuid();
            _storage.ServicesInVisit.Add(resource);

            SaveAllChanges();
            return resource;
        }

        private bool ContainsInStorage(ServiceInVisit resource)
        {
            return _storage.ServicesInVisit
                .Get(x => (x.MasterId == resource.MasterId 
                    && x.ServiceId == resource.ServiceId 
                    && x.VisitId==resource.VisitId))
                .SingleOrDefault() != null;
        }

        public List<ServiceInVisit> GetAll()
        {
            return _storage.ServicesInVisit.Queryable
                .AsNoTracking()
                .Include(x => x.Visit)
                .Include(x => x.MasterServices)
                .ToList();
        }

        public ServiceInVisit Update(ServiceInVisit resource)
        {
            if (ContainsInStorage(resource)) throw new ObjectMissingException("Данная выбранная услуга уже существует");
            _storage.ServicesInVisit.Update(resource);
            return resource;
        }
        

        private void SaveAllChanges()
        {
            try
            {
                _storage.Save();
            }
            catch (Exception)
            {
                throw new ObsoleteDataException("Не удалось сохранить выбранную услугу в бд");
            }
        }

        public ServiceInVisit Get(Guid masterId, Guid serviceId, Guid visitId)
        {
            var serviceInVisit = _storage.ServicesInVisit.Queryable
                .AsNoTracking().Include(m => m.MasterServices)
                .SingleOrDefault(x => (x.MasterId == masterId && x.ServiceId == serviceId && x.VisitId==visitId));
            return serviceInVisit;
        }


        public ServiceInVisit Delete(Guid serviceInVisit)
        {
            var serviceInVisitToDelete = _storage.ServicesInVisit.Get(x => x.Id == serviceInVisit).First();
            if (serviceInVisitToDelete == null) throw new ObjectMissingException("Данная выбранная услуга отсутствует в бд");
            _storage.ServicesInVisit.Remove(serviceInVisitToDelete);
            SaveAllChanges();
            return serviceInVisitToDelete;
        }
        public ServiceInVisit GetById(Guid serviseInVisitId)
        {
            var serviceInVisit = _storage.ServicesInVisit.Queryable
                .AsNoTracking().Include(m => m.MasterServices.Service).Include(s => s.MasterServices.Master).Include(x => x.Visit)
                .SingleOrDefault(x => x.Id == serviseInVisitId);
            if (serviceInVisit == null) throw new ObjectMissingException("Услуги мастера отсутствует в бд");
            return serviceInVisit;
        }

        public List<ServiceInVisit> GetInVisit(Guid visitId)
        {
            var serviceInVisit = _storage.ServicesInVisit.Queryable
                .AsNoTracking().Include(m => m.MasterServices.Service).Include(s=>s.MasterServices.Master).Include(x => x.Visit)
                .Where(x => x.VisitId == visitId)
                .ToList();
            if (serviceInVisit == null) throw new ObjectMissingException("Услуги мастера отсутствует в бд");
            return serviceInVisit;
        }
    }
}
