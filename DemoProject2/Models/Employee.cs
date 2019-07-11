using System.ComponentModel.DataAnnotations;

namespace DemoProject2.Models
{
    public class Employee
    {
        [Key] 
        public int Id { get; set; }

        public string Nane { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Role { get; set; }
    }
}