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
    public class LeaveTypeController : Controller
    {
        //public static string BaseURL = "http://192.168.2.48/LRS_API/";
        List<LeaveTypeInfo> listMember = new List<LeaveTypeInfo>();
        LeaveTypeInfo iLeaveTypeInfo = new LeaveTypeInfo();
        // GET: LeaveType

        [SessionTimeout]
        public ActionResult Index()
        {
            //using (var client = new HttpClient())
            //{
            //    iLeaveTypeInfo.iMode = 104;
            //    var listitems = Status.InvokeWebAPI(iLeaveTypeInfo, "GetLeaveTypeDetails");
            //    if (listitems.Any())
            //    {
            //        //ViewBag.LeaveTypeList = new SelectList(listitems, "LeaveTypeID", "LeaveTypeName");
            //        return View(listitems.ToList());
            //    }
            //    else
            //        return RedirectToAction("Create");
            //}
            //if (Convert.ToString(Session["Email"]) == "")
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            using (var client = new HttpClient())
            {
                iLeaveTypeInfo.iMode = 104;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetLeaveTypeDetails", iLeaveTypeInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            iLeaveTypeInfo = new LeaveTypeInfo
                            {
                                LeaveTypeID = Convert.ToInt32(dt.Rows[i]["LeaveTypeID"]),
                                LeaveTypeName = Convert.ToString(dt.Rows[i]["LeaveTypeName"]),
                                isActive = Convert.ToBoolean(dt.Rows[i]["isActive"])
                            };

                            listMember.Add(iLeaveTypeInfo);
                        }

                    }
                }
            }-
            return View(listMember);

        }

        // GET: LeaveType/Details/5


        // GET: LeaveType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveType/Create
        [HttpPost]
        [SessionTimeout]
        public ActionResult Create(LeaveTypeInfo iLeaveTypeInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(iLeaveTypeInfo.LeaveTypeName))
                    ModelState.AddModelError("LeaveTypeName", "Please enter leave type name");
                if (!ModelState.IsValid)
                {
                    return View();
                }

                using (var client = new HttpClient())
                {
                    iLeaveTypeInfo.iMode = 105;
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetLeaveTypeDetails", iLeaveTypeInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnSuccess();");
                        return View();
                    }
                    else
                        return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveType/Edit/5
        [SessionTimeout]
        public ActionResult Edit(int id)
        {
            using (var client = new HttpClient())
            {
                iLeaveTypeInfo.iMode = 106;
                iLeaveTypeInfo.LeaveTypeID = id;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetLeaveTypeDetails", iLeaveTypeInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            iLeaveTypeInfo = new LeaveTypeInfo
                            {
                                LeaveTypeID = Convert.ToInt32(dt.Rows[i]["LeaveTypeID"]),
                                LeaveTypeName = Convert.ToString(dt.Rows[i]["LeaveTypeName"]),
                                isActive = Convert.ToBoolean(dt.Rows[i]["isActive"])
                            };


                        }

                    }
                }
            }

            return View(iLeaveTypeInfo);
        }

        // POST: LeaveType/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        [HttpPost]
        [SessionTimeout]
        public ActionResult Edit(LeaveTypeInfo iLeaveTypeInfo, int id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(iLeaveTypeInfo.LeaveTypeName))
                    ModelState.AddModelError("LeaveTypeName", "Please enter leave type name");
                if (!ModelState.IsValid)
                {
                    return View();
                }

                using (var client = new HttpClient())
                {
                    iLeaveTypeInfo.iMode = 105;
                    iLeaveTypeInfo.LeaveTypeID = id;
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetLeaveTypeDetails", iLeaveTypeInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnSuccess();");
                        return View();
                        //return RedirectToAction("Index");
                    }
                    else
                        return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }


}
