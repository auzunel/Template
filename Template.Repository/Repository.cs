using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Template.Repository
{
    public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : class
    {

        private readonly DbContext _context;

        public Repository()
        {
            _context = new TemplateEntities();
        }

        public IList<TResult> GetAll<TResult>(Func<TEntity, TResult> select = null) where TResult : new()
        {
            IQueryable<TEntity> dbQuery = _context.Set<TEntity>();

            return select == null ? dbQuery.AsNoTracking().Cast<TResult>().ToList() : dbQuery.AsNoTracking().Select(select).ToList();
        }

        public TResult GetSingle<TResult>(Expression<Func<TEntity, bool>> where, Func<TEntity, TResult> select = null) where TResult : new()
        {
            IQueryable<TEntity> dbQuery = _context.Set<TEntity>();

            return select == null ? dbQuery.AsNoTracking().Where(where).Cast<TResult>().SingleOrDefault() : dbQuery.AsNoTracking().Where(where).Select(select).SingleOrDefault();
        }

        public IList<TResult> GetList<TResult>(Expression<Func<TEntity, bool>> where, Func<TEntity, TResult> select = null) where TResult : new()
        {
            IQueryable<TEntity> dbQuery = _context.Set<TEntity>();

            return select == null ? dbQuery.AsNoTracking().Where(where).Cast<TResult>().ToList() : dbQuery.AsNoTracking().Where(where).Select(select).ToList();
        }

        public int Count(Expression<Func<TEntity, bool>> where = null)
        {
            IQueryable<TEntity> dbQuery = _context.Set<TEntity>();

            return where == null ? dbQuery.AsNoTracking().Count() : dbQuery.AsNoTracking().Where(where).Count();
        }

        public void Add(params TEntity[] entities)
        {
            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Added;
            }

            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            } 
        }

        public void Update(params TEntity[] entities)
        {
            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Remove(params TEntity[] entities)
        {
            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }

            _context.SaveChanges();
        }

        public void Remove(Expression<Func<TEntity, bool>> where)
        {
            IQueryable<TEntity> dbQuery = _context.Set<TEntity>();

            var entities = dbQuery.Where(where);

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }

            _context.SaveChanges();
        }

        #region Implementation of IDisposable
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
        #endregion
    }
}
