using System;
using DemoProject2.Models;

namespace DemoProject2.Repositories
{
    public class UnitOfWork :IDisposable,IUnitOfWork
    {
        private readonly MyDbContext _db;

        public UnitOfWork(MyDbContext db)
        {
            _db = db;
        }
        private Repository<Employee> ER;

        public IRepository<Employee> EmployeeRepository => ER ?? (ER = new Repository<Employee>(_db));

        public void Save()
        {
            _db.SaveChanges();
        }
        private bool disposed = false;
 
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}