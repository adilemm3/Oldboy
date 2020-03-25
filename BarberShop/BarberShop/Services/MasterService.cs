using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using BarberShop.Exeptions.Throws;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShop.Services
{
    public class MasterService : ICrudService<Master>
    {
        private readonly IBarberShopStorage _storage;

        public MasterService(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public List<Master> GetAll()
        {
            return _storage.Masters.Queryable
                .AsNoTracking()
                .Include(s => s.MasterServices)
                .OrderBy(x => x.FullName)
                .ToList();
        }

        public Master Get(Guid masterId)
        {
            var master = _storage.Masters.Queryable
                .AsNoTracking()
                .Include(s=>s.MasterServices)
                .SingleOrDefault(x => x.Id == masterId);
            if (master == null) throw new ObjectMissingException($"Парикмахер отсутствует в бд");
            return master;
        }

        public Master Create(Master masterToAdd)
        {
            if (ContainsInStorage(masterToAdd)) throw new ResourceAlreadyExistException($"Парикмахер с ФИО {masterToAdd.FullName} уже существует");
            masterToAdd.Id = Guid.NewGuid();
            _storage.Masters.Add(masterToAdd);
            SaveAllChanges();
            return masterToAdd;
        }

        private bool ContainsInStorage(Master masterToAdd)
        {
            return _storage.Masters
                .Get(x => x.Id == masterToAdd.Id || x.FullName.Equals(masterToAdd.FullName))
                .SingleOrDefault() != null;
        }

        public Master Update(Master masterToUpdate)
        {
            if (!ContainsInStorage(masterToUpdate)) throw new ObjectMissingException($"Парикмахер с ФИО {masterToUpdate.FullName} отсутствует в бд");
            _storage.Masters.Update(masterToUpdate);
            return masterToUpdate;
        }

        public Master Delete(Guid masterId)
        {
            var masterToDelete = _storage.Masters.FindById(masterId);
            if (masterToDelete==null) throw new ObjectMissingException($"Парикмахер отсутствует в бд");
            _storage.Masters.Remove(masterToDelete);
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
                throw new ObsoleteDataException("Не удалось сохранить парикмахера в бд");
            }
        }
    }
}
