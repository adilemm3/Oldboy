using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using BarberShop.Exeptions.Throws;
using System.Collections.Generic;

namespace BarberShop.Services
{
    public class ServiceService : ICrudService<Service>
    {
        private readonly IBarberShopStorage _storage;

        public ServiceService(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public List<Service> GetAll()
        {
            return _storage.Services.Queryable
                .AsNoTracking()
                .OrderBy(x => x.NameOfService)
                .ToList();
        }

        public Service Get(Guid serviceId)
        {
            var service = _storage.Services.Queryable
                .AsNoTracking()
                .SingleOrDefault(x => x.Id == serviceId);
            if (service == null) throw new ObjectMissingException("Данная услуга отсутствует в бд");
            return service;
        }

        public Service Create(Service serviceToAdd)
        {
            if (ContainsInStorage(serviceToAdd)) throw new ResourceAlreadyExistException($"Услуга {serviceToAdd.NameOfService} уже существует");
            _storage.Services.Add(serviceToAdd);
            SaveAllChanges();
            return serviceToAdd;
        }
        
        private bool ContainsInStorage(Service resource)
        {
            return _storage.Services
                .Get(x => x.Id == resource.Id || x.NameOfService.Equals(resource.NameOfService))
                .SingleOrDefault() != null;
        }
        public Service Update(Service serviceToUpdate)
        {
            if(!ContainsInStorage(serviceToUpdate)) throw new ObjectMissingException($"Услуга {serviceToUpdate.NameOfService} отсутствует в бд");
            _storage.Services.Update(serviceToUpdate);
            return serviceToUpdate;
        }

        public Service Delete(Guid serviceId)
        {
            var serviceToDelete = _storage.Services.FindById(serviceId);
            if (serviceToDelete==null) throw new ObjectMissingException("Данная услуга отсутствует в бд");
            _storage.Services.Remove(serviceToDelete);
            SaveAllChanges();
            return serviceToDelete;
        }
        
        private void SaveAllChanges()
        {
            try
            {
                _storage.Save();
            }
            catch (Exception)
            {
                throw new ObsoleteDataException("Не удалось сохранить услугу в бд");
            }
        }
    }
}
