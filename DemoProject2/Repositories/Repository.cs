using System.Collections.Generic;
using DemoProject2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;  
using System;
using System.Linq.Expressions;

namespace DemoProject2.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MyDbContext _db;
        private readonly DbSet<T> _entity;

        public Repository(MyDbContext db)
        {
            _db = db;
            _entity = _db.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _entity;
        }

        public T Get(int? id)
        {
            return _entity.Find(id);
        }

        public void Insert(T entity)
        {
            _entity.Add(entity);
        }

        public void Update(T entity)
        {
            _entity.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        

        public void Remove(T entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _entity.Attach(entity);
            }

            _entity.Remove(entity);
        }
        public void Delete(object id)
        {
            T entity = _entity.Find(id);
            Remove(entity);
        }
    }
}