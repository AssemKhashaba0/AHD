using AHD.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }


        public List<T> GetAll(string? include = null)
        {
            return include == null ? dbSet.ToList() : dbSet.Include(include).ToList();
        }

        public IEnumerable<T> Get(Expression<Func<T, object>>[]? includeProp = null, Expression<Func<T, bool>>? expression = null)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProp != null)
            {
                foreach (var prop in includeProp)
                {
                    query = query.Include(prop);
                }
            }

            return query.ToList();
        }

        public T? GetOne(Expression<Func<T, bool>> expression)
        {
            return dbSet.FirstOrDefault(expression);
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includeProp = null)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProp != null)
            {
                foreach (var prop in includeProp)
                {
                    query = query.Include(prop);
                }
            }

            return query.FirstOrDefault();
        }

        public T? GetById(int entityId)
        {
            return dbSet.Find(entityId);
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public int Count(Expression<Func<T, bool>>? expression = null)
        {
            return expression != null ? dbSet.Count(expression) : dbSet.Count();
        }
    }
}
