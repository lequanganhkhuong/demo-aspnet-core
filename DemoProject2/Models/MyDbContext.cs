using Microsoft.EntityFrameworkCore;

namespace DemoProject2.Models
{
    public class MyDbContext :DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options):base(options)
        {
            //Database.Migrate();
        }
        public DbSet<Employee> Employees { get; set; }
    }
}