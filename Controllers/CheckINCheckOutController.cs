using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using LRS;
using Model;
using System.Linq;

namespace LRS.Controllers
{
    public class CheckINCheckOutController : Controller
    {
        List<MemberInfo> listMember = new List<MemberInfo>();
        MemberInfo iMemberInfo = new MemberInfo();
        // GET: ChechINCheckOut
        public ActionResult Index()
        {
            var listMember = GetMemberInfo();            
            ViewData["Norecords"] = "No records found";
            return View(listMember);
            //return View();
        }

        private List<MemberInfo> GetMemberInfo()
        {
            using (var client = new HttpClient())
            {
                //Session["UserID"] = 6;
                iMemberInfo.iMode = 104;
                // iLeaveRequestInfo.Id = Convert.ToInt32(Session["UserID"]);
                iMemberInfo.EmpID = Convert.ToString(Session["UserId"]);
                iMemberInfo.ID = Convert.ToInt32(Convert.ToString(Session["UserId"]));
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetCheckINDetails", iMemberInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;

                if (status.Errors == null)
                {
                    var memberInfo = JsonConvert.DeserializeObject<List<MemberInfo>>(status.Data.ToString());
                    return memberInfo;
                }
                return listMember;
            }
        }

        public ActionResult Create()
        {
            using (var client = new HttpClient())
            {
                iMemberInfo.iMode = 103;
                iMemberInfo.EmpID = Session["UserId"].ToString(); 
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetCheckINDetails", iMemberInfo).Result;
                /*var result = response.Content.ReadAsAsync<MemberInfo>().Result;
                if (Convert.ToString(result.CheckOutTime) != "")
                {
                    TempData["Button"] = "CheckIn";
                }
                else
                {
                    TempData["Button"] = "CheckOut";
                }*/
                TempData["Button"] = "Check In";
                TempData.Keep("Button");
               

                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        DataColumnCollection columns = dt.Columns;
                        if (columns.Contains("CheckOutTime"))
                        {

                            if (Convert.ToString(dt.Rows[0]["CheckOutTime"]) != "")
                            {
                                TempData["Button"] = "Check In";
                                ViewBag.Checkout = "You Checked Out At" + " " + dt.Rows[0]["CheckOutTime"];
                            }
                            else
                            {
                                TempData["Button"] = "Check Out";
                            }
                        }
                        else
                        {
                            TempData["Button"] = "Check Out";
                            //ViewBag.Checkout = false;
                        }
                        TempData.Keep("Button");
                    }
                    else
                    {
                        TempData["Button"] = "Check In";
                        
                        ViewBag.Checkout = "You Checked Out at" + " " + dt.Rows[0]["CheckOutTime"];
                    }
                    //TempData.Keep("Button");//11/1/19
                   
                    ViewBag.Checkin = "You Checked In at" + " " + dt.Rows[0]["CheckInTime"];
                    //TempData["JavaScriptFunction"] = string.Format("fnSuccess('0');");

                    //ViewBag.Checkout = dt.Rows[0]["CheckOutTime"];
                }
            }
            //TempData.Keep("Button");

            var listMember = GetMemberInfo();
            //var hours = listMember.Select(p => p.TotalHours).Sum();

            //ViewBag.Hours = hours;
            return View(listMember);

           

            //return View();
        }

        [HttpPost]
        public ActionResult Create(MemberInfo iMemberInfo)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    iMemberInfo.iMode = 102;
                    iMemberInfo.EmpID = Session["UserId"].ToString();
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetCheckINDetails", iMemberInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        //TempData["JavaScriptFunction"] = string.Format("fnEditSuccess('0');");
                        if (Convert.ToString(TempData["Button"]) == "Check In")
                        {
                            TempData["JavaScriptFunction"] = string.Format("fnSuccess('0','0');");
                        }
                        else
                        {

                            TempData["JavaScriptFunction"] = string.Format("fnSuccess('0','1');");
                        }
                        //return View();
                    }
                    else
                    {
                        //TempData["JavaScriptFunction"] = string.Format("fnEditSuccess('1');");

                        TempData["JavaScriptFunction"] = string.Format("fnSuccess('1','1');");
                        //return View();
                    }

                    return RedirectToAction("Create");

                }


            }
            catch (Exception ex)
            {
                return View();
            }
            
        }
    }
}
