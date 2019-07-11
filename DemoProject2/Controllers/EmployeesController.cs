using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoProject2.Models;
using DemoProject2.Repositories;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;

namespace DemoProject2.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        //private readonly MyDbContext _context;
        private IUnitOfWork uw;
        public EmployeesController(IUnitOfWork _uw)
        {
            uw = _uw;
            //_context = context;
        }
        
        // GET: Employees
        public IActionResult Index(string sortOrder,string search,int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            
            var emps = from a in uw.EmployeeRepository.GetAll() select a;
            
            ViewBag.Name = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.Role = sortOrder == "admin" ? "employee" : "admin";
            ViewBag.search = search;
            if (!string.IsNullOrEmpty(search))
            {
                emps = emps.Where(x => x.Nane.Contains(search) || x.Username.Contains(search));
            }
            switch (sortOrder)
            {
                    
                case "Name_desc":
                    emps = emps.OrderByDescending(x => x.Nane);
                    break;
                case "admin":
                    emps = emps.OrderBy(x => x.Role);
                    break;
                case "employee":
                    emps = emps.OrderByDescending(x => x.Role);
                    break;
                default:
                    emps = emps.OrderBy(x => x.Nane);
                    break;
            }
            return View(PagingList.Create(emps.ToList(),pageSize,pageNumber));
        }

        // GET: Employees/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = uw.EmployeeRepository.Get(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nane,Username,Password,Avatar,Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                uw.EmployeeRepository.Insert(employee);
                uw.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = uw.EmployeeRepository.Get(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nane,Username,Password,Avatar,Role")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    uw.EmployeeRepository.Update(employee);
                    uw.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        [Authorize(Policy = "Admin")]
        // GET: Employees/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = uw.EmployeeRepository.Get(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [Authorize(Policy = "Admin")]
        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = uw.EmployeeRepository.Get(id);
            uw.EmployeeRepository.Remove(employee);
            uw.Save();
            return RedirectToAction(nameof(Index));
        }

        
    }
}
