using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LRS.Controllers
{
    public class LoginController : Controller
    {
        //public static string BaseURL = "http://192.168.2.48/LRS_API/";
        LeaveTypeInfo iLeaveTypeInfo = new LeaveTypeInfo();
        LeaveRequestInfo iLeaveRequestInfo = new LeaveRequestInfo();
        MemberInfo iMemberInfo = new MemberInfo();
        List<LeaveRequestInfo> listItems = new List<LeaveRequestInfo>();
        List<MemberInfo> liMemberInfo = new List<MemberInfo>();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(MemberInfo iMemberInfo)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Utility.BaseURL);
                iMemberInfo.iMode = 122;
                //var response = client.PostAsJsonAsync<MemberInfo>("CheckUser", iMemberInfo).Result;
                //Status status = response.Content.ReadAsAsync<Status>().Result;

                var response = client.PostAsJsonAsync("GetMemberDetails", iMemberInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Data != null)
                {
                    var userdetails = JsonConvert.DeserializeObject<List<MemberInfo>>(status.Data.ToString());
                    if (userdetails != null && userdetails.Count > 0)
                    {
                        Session["UserId"] = userdetails[0].ID;
                        Session["FirstName"] = userdetails[0].FirstName;
                        Session["Email"] = userdetails[0].EmailAddress;
                        Session["IsAdmin"] = userdetails[0].IsAdmin; 
                        if (Convert.ToBoolean(Session["IsAdmin"]) == true)
                            Session["Password"] = "Admin123";
                        else
                            Session["Password"] = "Maple@123";


                        if ((Convert.ToBoolean(Session["IsAdmin"]) == true) && (Convert.ToString(Session["Password"]) == iMemberInfo.Password) && (Convert.ToString(Session["Email"]).ToLower() == iMemberInfo.EmailAddress.ToLower()))
                        {
                            return RedirectToAction("Index", "MemberDetails");
                        }
                        else if ((Convert.ToString(Session["Email"]).ToLower() == iMemberInfo.EmailAddress.ToLower()) && (Convert.ToString(Session["Password"]) == iMemberInfo.Password))
                        {
                            return RedirectToAction("Create", "CheckINCheckOut");
                        }
                        else
                        {
                            TempData["JavaScriptFunction"] = string.Format("fnSuccess('1');");
                        }
                    }
                    else
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnSuccess('0');");
                    }
                }
                else
                {
                    TempData["JavaScriptFunction"] = string.Format("fnSuccess('1');");
                }
                return View();

            }
        }

        public ActionResult SessionLogOff()
        {
            Session.Clear();
            return RedirectToAction("Index");            
        }


    }
    //    if (data.Code == HttpStatusCode.OK)
    //    {
    //        var userdetails = JsonConvert.DeserializeObject<List<EmployeesDto>>(data.Data.ToString());

    //        if (userdetails != null && userdetails.Count > 0)
    //        {
    //            Session["UserId"] = userdetails[0].UserID;
    //            Session["EmployeeId"] = userdetails[0].EmployeeID;
    //            Session["Name"] = userdetails[0].FirstName.ToString() + ", " + userdetails[0].LastName[0].ToString();
    //            Session["IsAdmin"] = userdetails[0].IsAdmin.ToString().ToLower();
    //            Session["Designation"] = userdetails[0].Designation;
    //            Session["IsProfileUpdated"] = userdetails[0].IsProfileUpdated.ToString().ToLower();
    //            Session["ProfilePic"] = Utility.GetImagePath(userdetails[0].EmployeeID.ToString(), Convert.ToInt32(userdetails[0].Gender));
    //        }

    //        Status status = new Status("OK", null, userdetails);
    //        return Json(status, JsonRequestBehavior.AllowGet);
    //    }
    //    else
    //    {
    //        return Json(data, JsonRequestBehavior.AllowGet);
    //    }
    //}

}
