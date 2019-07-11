using DemoProject2.Models;

namespace DemoProject2.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Employee> EmployeeRepository { get; }
        void Save();
    }
}