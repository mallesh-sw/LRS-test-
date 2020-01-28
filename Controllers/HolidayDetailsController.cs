using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LRS.Controllers
{
    public class HolidayDetailsController : Controller
    {
       // public static string BaseURL = "http://192.168.2.48/LRS_API/";
        // GET: HolidayDetails
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HolidayInfo iHolidayInfo)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(iHolidayInfo.HolidayName)))
                ModelState.AddModelError("HolidayName", "Please enter holiday name");

            if (string.IsNullOrWhiteSpace(Convert.ToString(iHolidayInfo.HolidayDate)))
                ModelState.AddModelError("HolidayDate", "Please enter date");

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                using (var client = new HttpClient())
                {

                    iHolidayInfo.iMode = 115;
                    iHolidayInfo.PreApproved = 1;
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("HolidayDetails", iHolidayInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                        return View("Index");
                }
            }
            catch
            {
                return View("Index");
            }

        }
    }
}