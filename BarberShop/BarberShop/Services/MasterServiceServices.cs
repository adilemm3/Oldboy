using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using BarberShop.Exeptions.Throws;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShop.Services
{
    public class MasterServiceServices : IMasterServiceCrudService<MasterServices>
    {
        private readonly IBarberShopStorage _storage;

        public MasterServiceServices(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public List<MasterServices> GetAll()
        {
            return _storage.MasterServices.Queryable
                .AsNoTracking()
                .Include(s => s.Service)
                .Include(m => m.Master)
                .ToList();
        }
        private bool ContainsInStorage(MasterServices resource)
        {
            return _storage.MasterServices
                .Get(x => (x.MasterId == resource.MasterId && x.ServiceId==resource.ServiceId))
                .SingleOrDefault() != null;
        }

        public MasterServices Create(MasterServices resource)
        {
            if (ContainsInStorage(resource)) throw new ResourceAlreadyExistException($"Данная услуга мастера уже существует");
            _storage.MasterServices.Add(resource);
            SaveAllChanges();
            return resource;

        }

        public MasterServices Update(MasterServices resource)
        {
            if (ContainsInStorage(resource)) throw new ObjectMissingException("Данная услуга мастера уже существует");
            _storage.MasterServices.Update(resource);
            return resource;
        }
        public MasterServices UpdateNew(MasterServices resource)
        {
            if (ContainsInStorage(resource)) throw new ObjectMissingException("Данная услуга мастера уже существует");
            _storage.MasterServices.UpdateNew(resource);
            return resource;
        }

        public MasterServices Delete(Guid masterId, Guid serviceId)
        {
            var masterToDelete = _storage.MasterServices.Get(x=>(x.MasterId==masterId && x.ServiceId==serviceId)).First();
            if (masterToDelete == null) throw new ObjectMissingException("Данная услуга мастера отсутствует в бд");
            _storage.MasterServices.Remove(masterToDelete);
            SaveAllChanges();
            return masterToDelete;
        }

        private void SaveAllChanges()
        {
            try
            {
                _storage.Save();
            }
            catch (Exception)
            {
                throw new ObsoleteDataException("Не удалось сохранить услугу мастера в бд");
            }
        }

        public MasterServices Get(Guid masterId, Guid serviceId)
        {
            var masterService = _storage.MasterServices.Queryable
                .AsNoTracking().Include(s=>s.Service).Include(m=>m.Master)
                .SingleOrDefault(x=>(x.MasterId==masterId &&x.ServiceId==serviceId));
            return masterService;
        }

        public List<MasterServices> GetByMasterId(Guid masterId)
        {
            var masterService = _storage.MasterServices.Queryable
                .AsNoTracking().Include(s => s.Service).Include(m => m.Master)
                .Where(x => x.MasterId == masterId)
                .ToList();
            if (masterService == null) throw new ObjectMissingException("Данная услуга мастера отсутствует в бд");
            return masterService;
        }
    }
}
