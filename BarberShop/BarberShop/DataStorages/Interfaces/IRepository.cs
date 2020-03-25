using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShop.DataStorages.Interfaces
{
    public interface IRepository<TEntity>:IDisposable
    {
        IQueryable<TEntity> Queryable { get; }
        void Add(TEntity item);
        TEntity FindById(Guid id);
        IEnumerable<TEntity> Get();
        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
        void Remove(TEntity item);
        void Update(TEntity item);
        void UpdateNew(TEntity irem);
         
    }
}
