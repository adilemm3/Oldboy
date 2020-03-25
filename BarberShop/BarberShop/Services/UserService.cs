using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using BarberShop.Exeptions.Throws;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShop.Services
{
    public class UserService : ICrudService<User>
    {
        private readonly IBarberShopStorage _storage;

        public UserService(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public List<User> GetAll()
        {
            return _storage.Users.Queryable.OrderBy(x => x.FullName).ToList();
        }

        public User Get(Guid userId)
        {
            var user = _storage.Users.Queryable
               
                .SingleOrDefault(x => x.Id == userId);
            if (user == null) throw new ObjectMissingException($"Парикмахер отсутствует в бд");
            return user;
        }

        public User Create(User userToAdd)
        {
            if (ContainsInStorage(userToAdd)) throw new ResourceAlreadyExistException($"Парикмахер с ФИО {userToAdd.FullName} уже существует");
            userToAdd.Id = Guid.NewGuid();
            _storage.Users.Add(userToAdd);
            SaveAllChanges();
            return userToAdd;
        }

        private bool ContainsInStorage(User userToAdd)
        {
            return _storage.Masters
                .Get(x => x.Id == userToAdd.Id || x.FullName.Equals(userToAdd.FullName))
                .SingleOrDefault() != null;
        }

        public User Update(User userToUpdate)
        {
            if (!ContainsInStorage(userToUpdate)) throw new ObjectMissingException($"Парикмахер с ФИО {userToUpdate.FullName} отсутствует в бд");
            _storage.Users.Update(userToUpdate);
            return userToUpdate;
        }

        public User Delete(Guid userId)
        {
            var userToDelete = _storage.Users.FindById(userId);
            if (userToDelete == null) throw new ObjectMissingException($"Парикмахер отсутствует в бд");
            _storage.Users.Remove(userToDelete);
            SaveAllChanges();
            return userToDelete;
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
