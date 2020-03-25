using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using BarberShop.Exeptions.Throws;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BarberShop.Services
{
    public class VisitService : ICrudService<Visit>
    {
        private readonly IBarberShopStorage _storage;

        public VisitService(IBarberShopStorage storage)
        {
            _storage = storage;
        }
        public List<Visit> GetAll()
        {
            return _storage.Visits.Queryable
                .AsNoTracking()
                .Include(x => x.ServicesInVisit)
                .Include(c=>c.Client)
                .OrderByDescending(x => x.DateOfVisit)
                .ToList();
        }

        public Visit Get(Guid visitId)
        {
            var visit = _storage.Visits.Queryable
                .AsNoTracking()
                .Include(c=>c.Client)
                .Include(s=>s.ServicesInVisit)
                .SingleOrDefault(x => x.Id == visitId);
            if (visit == null) throw new ObjectMissingException("Данная запись клиента отсутствует в бд");
            return visit;
        }

        public Visit Create(Visit visitToAdd)
        {
            if (ContainsInStorage(visitToAdd)) throw new ResourceAlreadyExistException($"Запись клиента {visitToAdd.Client.Name} в {visitToAdd.DateOfVisit} уже существует");
            _storage.Visits.Add(visitToAdd);
            SaveAllChanges();
            return visitToAdd;
        }
        private bool ContainsInStorage(Visit visit)
        {
            return _storage.Visits
                .Get(x => x.Id == visit.Id || (x.DateOfVisit.Equals(visit.DateOfVisit) && x.ClientId.Equals(visit.ClientId)))
                .SingleOrDefault() != null;
        }

        public Visit Update(Visit visitToUpdate)
        {
            if (!ContainsInStorage(visitToUpdate)) throw new ObjectMissingException($"Запись клиента {visitToUpdate.Client.Name} в {visitToUpdate.DateOfVisit} отсутствует в бд");
            _storage.Visits.Update(visitToUpdate);
            return visitToUpdate;
        }

        public Visit Delete(Guid visitId)
        {
            var visitToDelete = _storage.Visits.FindById(visitId);
            if(visitToDelete == null) throw new ObjectMissingException("Данная запись клиента отсутствует в бд");
            _storage.Visits.Remove(visitToDelete);
            SaveAllChanges();
            return visitToDelete;
        }

        private void SaveAllChanges()
        {
            try
            {
                _storage.Save();
            }
            catch (Exception)
            {
                throw new ObsoleteDataException("Не удалось сохранить запись клиента в бд");
            }
        }
    }
}
