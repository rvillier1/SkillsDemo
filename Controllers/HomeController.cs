using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkillsDemo.Models;
using SkillsDemo.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace SkillsDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;
        private string connString;

        public HomeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            Configuration = _configuration;
            connString = Configuration.GetConnectionString("AzureTestDB");
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult DatabaseIO(string SortType, string SortOrder)
        {
            List<Employee> emp = new List<Employee>();
            try
            {
                dbContect context = new dbContect(connString);
                string sql = $"EXECUTE dbo.GetEmployeeForDisplay '" + SortOrder + "', '" + SortType + "'";
                emp = context.GetEmployeeForDisplay.FromSqlRaw(sql).ToList();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return View(emp);
        }
                
        public IActionResult EditDBIO(int EmployeeID)
        {
            List<Employee> emp = new List<Employee>();
            try
            {
                string region = "";
                if (EmployeeID != 0)
                {
                    dbContect context = new dbContect(connString);
                    string sql = $"EXECUTE dbo.GetEmployeeByID " + EmployeeID;
                    emp = context.GetEmployeeForDisplay.FromSqlRaw(sql).ToList();
                    region = emp[0].Region;
                }
                else
                {
                    Employee newEmp = new Employee();
                    emp.Add(newEmp);
                }
                var StateList = StateArray.States();
                ViewBag.StateList = new SelectList(StateList, "Abbreviations", "Name", region);

            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return View(emp);            
        }

        public IActionResult APISample()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EmployeeUpdates(Employee EmpResults)
        {
            string success = "OK";
            string ErrorMessage = "";
            List<Employee> emp = new List<Employee>();
            try
            {
                dbContect context = new dbContect(connString);
                string sql = "";
                if(EmpResults.EmployeeId == 0)
                    sql = "EXECUTE dbo.InsertEmployee '" + EmpResults.LastName + "', '" + EmpResults.FirstName + "', '" + EmpResults.Title + "', '" + EmpResults.BirthDate + "', '" + EmpResults.HireDate + "', '" + EmpResults.Address + "', '" + EmpResults.City + "', '" + EmpResults.Region + "', '" + EmpResults.PostalCode + "', '" + EmpResults.HomePhone + "', '" + EmpResults.Extension + "', '" + EmpResults.Notes + "'";
                else
                    sql = "EXECUTE dbo.UpdateEmployeeByID '" + EmpResults.EmployeeId + "', '" + EmpResults.LastName + "', '" + EmpResults.FirstName + "', '" + EmpResults.Title + "', '" + EmpResults.BirthDate + "', '" + EmpResults.HireDate + "', '" + EmpResults.Address + "', '" + EmpResults.City + "', '" + EmpResults.Region + "', '" + EmpResults.PostalCode + "', '" + EmpResults.HomePhone + "', '" + EmpResults.Extension + "', '" + EmpResults.Notes + "'";
                emp = context.GetEmployeeForDisplay.FromSqlRaw(sql).ToList();
            }
            catch (Exception ex)
            {
                success = "FAIL";
                ErrorMessage = ex.Message;
            }
            return Json(new { success = success, ErrorMessage = ErrorMessage });
        }

        [HttpPost]
        public JsonResult EmployeeDelete(int EmployeeID)
        {
            string success = "OK";
            string ErrorMessage = "";
            List<Employee> emp = new List<Employee>();
            try
            {
                dbContect context = new dbContect(connString);
                string sql =  "EXECUTE dbo.DeleteEmployeeByID " + EmployeeID;                
                emp = context.DeleteEmployeeByID.FromSqlRaw(sql).ToList();
            }
            catch (Exception ex)
            {
                success = "FAIL";
                ErrorMessage = ex.Message;
            }
            return Json(new { success = success, ErrorMessage = ErrorMessage });
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
