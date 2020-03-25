using BarberShop.DataStorages.Interfaces;
using BarberShop.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using BarberShop.Exeptions.Throws;
using System.Collections.Generic;

namespace BarberShop.Services
{
    public class ClientService : ICrudService<Client>
    {
        private readonly IBarberShopStorage _storage;

        public ClientService(IBarberShopStorage storage)
        {
            _storage = storage;
        }

        public List<Client> GetAll()
        {
            return _storage.Clients
                .Queryable
                .AsNoTracking()
                .OrderBy(x => x.Name).ThenBy(x => x.Phone)
                .ToList();
        }

        public Client Get(Guid clientId)
        {
            var client = _storage.Clients.Queryable
                .AsNoTracking()
                .SingleOrDefault(x => x.Id == clientId);
            if (client == null) throw new ObjectMissingException("Клиент отсутствует в бд");
            return client;
        }

        public Client Create(Client clientToAdd)
        {
            if(ContainsInStorage(clientToAdd)) throw new ResourceAlreadyExistException($"Клиент с ФИО {clientToAdd.Name} и номером телефона {clientToAdd.Phone} уже существует");
            _storage.Clients.Add(clientToAdd);
            SaveAllChanges();
            return clientToAdd;
        }
        private bool ContainsInStorage(Client client)
        {
            return _storage.Clients
             .Get(x => (x.Id == client.Id || (x.Name.Equals(client.Name) && x.Phone.Equals(client.Phone))))
             .SingleOrDefault()!=null;
        }

        private void ContainsInStorageUpdate(Client client)
        {
               var count = _storage.Clients
                .Get(x => (x.Id == client.Id || (x.Name.Equals(client.Name) && x.Phone.Equals(client.Phone)))).Count();
            switch (count)
            {
                case 0: throw new ObjectMissingException($"Клиент с ФИО {client.Name} и номером телефона {client.Phone} отсутствует в бд");
                case 2: throw new ResourceAlreadyExistException($"Клиент с ФИО {client.Name} и номером телефона {client.Phone} уже существует");
                default:
                    break;
            }
        }

        public Client Update(Client clientToUpdate)
        {
            ContainsInStorageUpdate(clientToUpdate);
            _storage.Clients.Update(clientToUpdate);
            return clientToUpdate;
        }
        public Client Delete(Guid clientId)
        {
            var clientToDelete = _storage.Clients.FindById(clientId);
            if (clientToDelete == null) throw new ObjectMissingException("Клиент отсутствует в бд");
            _storage.Clients.Remove(clientToDelete);
            SaveAllChanges();
            return clientToDelete;
        }

        private void SaveAllChanges()
        {
            try
            {
                _storage.Save();
            }
            catch (Exception)
            {
                throw new ObsoleteDataException("Не удалось сохранить клиента в бд");
            }
        }
    }
}
