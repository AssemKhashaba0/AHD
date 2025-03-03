using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll(string? include = null);

        IEnumerable<T> Get(Expression<Func<T, object>>[]? includeProp = null, Expression<Func<T, bool>>? expression = null);

        T? GetOne(Expression<Func<T, bool>> expression);

        T? GetById(int entityId);
        T? GetFirstOrDefault(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includeProp = null);


        void Create(T entity);
        void Edit(T entity);
        void Delete(T entity);
        void Commit();

        int Count(Expression<Func<T, bool>>? expression = null);
    }
}
