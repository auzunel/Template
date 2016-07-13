using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Template.Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {

        IList<TResult> GetAll<TResult>(Func<T, TResult> select = null)
            where TResult : new();

        TResult GetSingle<TResult>(Expression<Func<T, bool>> where, Func<T, TResult> select = null)
            where TResult : new();

        IList<TResult> GetList<TResult>(Expression<Func<T, bool>> where, Func<T, TResult> select = null)
            where TResult : new();

        int Count(Expression<Func<T, bool>> where = null);

        void Add(params T[] entities);

        void Update(params T[] entities);

        void Remove(params T[] entities);

        void Remove(Expression<Func<T, bool>> where);
    }
}
