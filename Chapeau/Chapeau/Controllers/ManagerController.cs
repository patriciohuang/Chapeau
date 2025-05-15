using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Repositories;
using System.Collections.Generic;

namespace Chapeau.Controllers
{
    public class ManagerController : BaseController
    {
        private readonly IEmployeesRepository _employeesRepository;

        public ManagerController(IEmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }

        public IActionResult Index()
        {
            // Check if user has access to this controller
            var accessResult = CheckAccess("Manager");
            if (accessResult != null)
            {
                return accessResult;
            }

            // Get all employees from repository
            IEnumerable<Employee> employees = _employeesRepository.GetAll();

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Check if user has access to this controller
            var accessResult = CheckAccess("Manager");
            if (accessResult != null)
            {
                return accessResult;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            // Check if user has access to this controller
            var accessResult = CheckAccess("Manager");
            if (accessResult != null)
            {
                return accessResult;
            }

            if (ModelState.IsValid) // Check if the model is valid
            {
                try
                {
                    _employeesRepository.Add(employee);
                    TempData["SuccessMessage"] = "Employee added successfully.";
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "Error adding employee: " + ex.Message);
                }
            }

            return View(employee);
        }

        [HttpPost]
        public IActionResult Delete(int employeeNr)
        {
            // Check if user has access to this controller
            var accessResult = CheckAccess("Manager");
            if (accessResult != null)
            {
                return accessResult;
            }

            try
            {
                // We need to add this method to the repository
                _employeesRepository.Delete(employeeNr);
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting employee: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}