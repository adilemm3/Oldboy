using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.DataStorages.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.DataStorages
{
    public class EfRepository<TEntity> :
        IRepository<TEntity>
        where TEntity : class
    {
        private readonly BarberShopContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public IQueryable<TEntity> Queryable => _dbSet;

        public EfRepository(BarberShopContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Add(TEntity item)
        {
            _dbSet.Add(item);
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            _dbSet.AddRange(items);
        }

        public TEntity FindById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> Get()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _dbSet.AsNoTracking().AsEnumerable().Where(predicate).ToList();
        }

        public void Update(TEntity item)
        {
            try
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            catch (InvalidOperationException)
            {
                var type = item.GetType();
                var originalItem = _context.Find(type, ((IEntity)item).Id);
                _context.Entry(originalItem).CurrentValues.SetValues(item);
            }
            finally
            {
                _context.SaveChanges();
            }
        }
        public void UpdateNew(TEntity item)
        {
            try
            {
                _context.Entry(item).State = EntityState.Added;
            }
            catch (InvalidOperationException)
            {
                var type = item.GetType();
                var originalItem = _context.Find(type, ((IEntity)item).Id);
                _context.Entry(originalItem).CurrentValues.SetValues(item);
            }
            finally
            {
                _context.SaveChanges();
            }
        }

        public void Remove(TEntity item)
        {
            _dbSet.Remove(item);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
