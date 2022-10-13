using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SkillsDemo.Data;
using SkillsDemo.Model;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkillsDemo.Models;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SkillsDemo.Controllers
{
    public class APIController : Controller
    {
        //Hosted web API REST Service base url
        string Baseurl = "https://localhost:7010/";

        public async Task<ActionResult> APIIO(string SortType, string SortOrder, string EmployeeID)
        {
            List<EmployeeForDisplay> EmpInfo = new List<EmployeeForDisplay>();
            try
            {
                using (var client = new HttpClient())
                {                    
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();                    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                    
                    HttpResponseMessage Res = await client.GetAsync("EmployeeData?SortOrder=" + SortOrder + "&SortType=" + SortType + "&EmployeeID=0");                    
                    if (Res.IsSuccessStatusCode)
                    {  
                        string EmpResponse = Res.Content.ReadAsStringAsync().Result;                        
                        EmpInfo = JsonSerializer.Deserialize<List<EmployeeForDisplay>>(EmpResponse);
                    }
                }
            }
            catch(Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return View(EmpInfo);
        }

        public async Task<ActionResult> EditAPIIO(int EmployeeID)
        {
            List<EmployeeForDisplay> EmpInfo = new List<EmployeeForDisplay>();
            string region = "";
            try
            {
                if (EmployeeID == 0)
                {
                    EmployeeForDisplay newEmp = new EmployeeForDisplay();
                    EmpInfo.Add(newEmp);
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage Res = await client.GetAsync("EmployeeData?SortOrder=FirstName&SortType=Asc&EmployeeID=" + EmployeeID);
                        if (Res.IsSuccessStatusCode)
                        {
                            string EmpResponse = Res.Content.ReadAsStringAsync().Result;
                            EmpInfo = JsonSerializer.Deserialize<List<EmployeeForDisplay>>(EmpResponse);
                            region = EmpInfo[0].region;
                        }
                    }
                }
                var StateList = StateArray.States();
                ViewBag.StateList = new SelectList(StateList, "Abbreviations", "Name", region);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return View(EmpInfo);
        }

        public async Task<JsonResult> EmployeeUpdates(EmployeeForDisplay EmpInfo)
        {

            string success = "OK";
            string ErrorMessage = "";
            try
            {               
                using (var client = new HttpClient())
                {
                    var json = JsonSerializer.Serialize(EmpInfo);
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();                 
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                    
                    HttpResponseMessage Res = await client.PostAsync("EmployeeData", requestContent);
                    if (Res.IsSuccessStatusCode)
                    {
                        string EmpResponse = Res.Content.ReadAsStringAsync().Result;
                        EmpInfo = JsonSerializer.Deserialize<EmployeeForDisplay>(EmpResponse);                        
                    }
                }
            }
            catch (Exception e)
            {
                success = "FAIL";
                ErrorMessage = e.Message;
            }
            return Json(new { success = success, ErrorMessage = ErrorMessage });
        }

        public async Task<JsonResult> EmployeeDelete(string EmployeeID)
        {
            List<EmployeeForDisplay> EmpInfo = new List<EmployeeForDisplay>();
            string success = "OK";
            string ErrorMessage = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    HttpResponseMessage Res = await client.DeleteAsync("EmployeeData?EmployeeID=" + EmployeeID);
                    if (Res.IsSuccessStatusCode)
                    {
                        string EmpResponse = Res.Content.ReadAsStringAsync().Result;
                        EmpInfo = JsonSerializer.Deserialize<List<EmployeeForDisplay>>(EmpResponse);
                    }
                }
            }
            catch (Exception e)
            {
                success = "FAIL";
                ErrorMessage = e.Message;
            }
            return Json(new { success = success, ErrorMessage = ErrorMessage });
        }

        
    }
}
