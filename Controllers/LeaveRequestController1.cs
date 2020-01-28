using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LRS.Controllers
{
    public class LeaveRequestController1 : Controller
    {
        //public static string BaseURL = "http://192.168.2.48/LRS_API/";
        LeaveTypeInfo iLeaveTypeInfo = new LeaveTypeInfo();
        LeaveRequestInfo iLeaveRequestInfo = new LeaveRequestInfo();
        MemberInfo iMemberInfo = new MemberInfo();
        List<LeaveRequestInfo> listItems = new List<LeaveRequestInfo>();
        // GET: LeaveRequest
        public ActionResult Index()
        {
            using (var client = new HttpClient())
            {
                //Session["UserID"] = 6;
                iLeaveRequestInfo.iMode = 110;
               // iLeaveRequestInfo.Id = Convert.ToInt32(Session["UserID"]);
                iLeaveRequestInfo.From = DateTime.Now;
                iLeaveRequestInfo.To = DateTime.Now;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetLeaveRequestDetails", iLeaveRequestInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            iLeaveRequestInfo = new LeaveRequestInfo
                            {
                                Name = Convert.ToString(dt.Rows[i]["Name"]),
                                UserID = Convert.ToString(dt.Rows[i]["UserID"]),
                                From = Convert.ToDateTime(dt.Rows[i]["From"]),
                                To = Convert.ToDateTime(dt.Rows[i]["To"]),
                                Type = Convert.ToString(dt.Rows[i]["Type"]),
                                Days = Convert.ToDecimal(dt.Rows[i]["Days"]),
                                Status = Convert.ToString(dt.Rows[i]["Status"]),
                                Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
                        };
                            listItems.Add(iLeaveRequestInfo);
                        }

                    }
                }
            }
            ViewData["Norecords"] = "No records found";
            return View(listItems);
        }

        // GET: LeaveRequest/Create
        public ActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> itemName = new List<SelectListItem>();
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Utility.BaseURL);
                iLeaveTypeInfo.iMode = 113;
                var response = client.PostAsJsonAsync<LeaveTypeInfo>("GetLeavetypes", iLeaveTypeInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                List<LeaveTypeInfo> model = (status.Text.ToLower() == "ok") ? JsonConvert.DeserializeObject<List<LeaveTypeInfo>>(status.Data.ToString()) : new List<LeaveTypeInfo>();
                foreach (var item in model) // Loop through List with foreach
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.LeaveTypeName,
                        Value = Convert.ToString(item.LeaveTypeID)
                    });
                }
                ViewData["LeaveTypes"] = items;
                // iLeaveRequestInfo.Type = model;

                iMemberInfo.iMode = 112;
                var response1 = client.PostAsJsonAsync<MemberInfo>("GetDetails", iMemberInfo).Result;
                Status status1 = response1.Content.ReadAsAsync<Status>().Result;
                if (status1.Data != null)
                {
                    List<MemberInfo> model1 = (status1.Text.ToLower() == "ok") ? JsonConvert.DeserializeObject<List<MemberInfo>>(status1.Data.ToString()) : new List<MemberInfo>();
                    // iLeaveRequestInfo.Name = model1;

                    foreach (var item1 in model1) // Loop through List with foreach
                    {

                        itemName.Add(new SelectListItem
                        {
                            Text = item1.FirstName + " " + item1.LastName,
                            Value = Convert.ToString(item1.ID)
                        });
                    }
                }
                ViewData["MemberName"] = itemName;
                return View(iLeaveRequestInfo);
            }
        }

        // POST: LeaveRequest/Create
        // [ActionName("InsertLeaveInfo")]
        [HttpPost]
        public ActionResult Create(LeaveRequestInfo iLeaveRequestInfo)
        {
            //if (string.IsNullOrWhiteSpace(iLeaveRequestInfo.Name))
            //    ModelState.AddModelError("Name", "select name");
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Create");
            //}

            try
            {
                using (var client = new HttpClient())
                {
                    //Session["UserID"] = 6;
                    iLeaveRequestInfo.iMode = 107;
                    //int recordid = Convert.ToInt32(Session["recordid"]) + 1;
                    //iLeaveRequestInfo.RecordID = recordid;
                    //iLeaveRequestInfo.UserID = Convert.ToString(Session["UserID"]);
                    // int Days = Convert.ToInt32((Convert.ToDateTime(iLeaveRequestInfo.To) - Convert.ToDateTime(iLeaveRequestInfo.From)).TotalDays) + 1;
                    //iLeaveRequestInfo.Days = Days;
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetLeaveRequestDetails", iLeaveRequestInfo).Result;
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
