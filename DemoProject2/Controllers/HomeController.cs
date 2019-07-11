using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemoProject2.Models;
using DemoProject2.Models.RegisterVM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace DemoProject2.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext db;
        private readonly IHostingEnvironment he;

        public HomeController(MyDbContext _db,IHostingEnvironment _he)
        {
            he = _he;
            db = _db;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var src = db.Employees.SingleOrDefault(x => x.Nane.Equals(User.Identity.Name))?.Avatar;
                ViewBag.src = src;
            }

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return RedirectToAction(nameof(Index));
            }

            var emp = db.Employees
                .FirstOrDefault(x => x.Username.Equals(username) && x.Password.Equals(password));
            if (emp != null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, emp.Nane),
                    new Claim(ClaimTypes.Role,emp.Role), 
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction(nameof(Index),"Employees");
            }
            return BadRequest();
            

        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult NotLogin() => View();
        public IActionResult ErrorForbbiden() => View();
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterVm reg, IFormFile pic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!reg.Password.Equals(reg.RePassword))
            {
                ViewBag.Message = "Password dont match";
                return View();
            }
            string src = "/Uploads/defaultAvatar.img";
            Employee emp = new Employee()
            {
                Nane = reg.Nane,
                Username = reg.Username,
                Password = reg.Password,
                Role = "employee",
                Avatar = src
            };
            db.Employees.Add(emp);
            db.SaveChanges();
            
            if (pic != null)
            {
                src = "/Uploads/Employees/"+ emp.Id.ToString();
                var path = he.ContentRootPath + "/wwwroot" + src;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file  = Path.Combine(path,pic.FileName);
                pic.CopyTo(new FileStream(file,FileMode.Create));
                emp.Avatar = src + "/" + pic.FileName;
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));

        }
        [Authorize(Policy =  "Admin")]
        public IActionResult Manage() => View();
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}