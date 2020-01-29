using Model;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LRS.Controllers
{
    public class LeaveDetailsController : Controller
    {
        // GET: LeaveDetails
       // public static string BaseURL = "http://192.168.2.48/LRS_API/";
        LeaveTypeInfo iLeaveTypeInfo = new LeaveTypeInfo();
        LeaveRequestInfo iLeaveRequestInfo = new LeaveRequestInfo();
        MemberInfo iMemberInfo = new MemberInfo();
        List<LeaveRequestInfo> listItems = new List<LeaveRequestInfo>();
       
        [SessionTimeout]
        public ActionResult Index()
        {
            //int pageSize = 5;
            //int pageIndex = 1;
            //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //IPagedList<LeaveRequestInfo> MI = null;

            using (var client = new HttpClient())
            {
                // Session["UserID"] = 6;
                iLeaveRequestInfo.iMode = 110;
                // iLeaveRequestInfo.Id = Convert.ToInt32(Session["UserID"]); //need to change with session userid
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
                            //string sVal = string.Empty;
                            //if (Convert.ToString(dt.Rows[i]["Status"]) == "Approved/Rejected")
                            //    sVal=
                            //        else

                            iLeaveRequestInfo = new LeaveRequestInfo
                            {
                                Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                                Name = Convert.ToString(dt.Rows[i]["Name"]),
                                UserID = Convert.ToString(dt.Rows[i]["UserID"]),
                                From = Convert.ToDateTime(dt.Rows[i]["From"]),
                                To = Convert.ToDateTime(dt.Rows[i]["To"]),
                                Type = Convert.ToString(dt.Rows[i]["Type"]),
                                Days = Convert.ToDecimal(dt.Rows[i]["Days"]),
                                Status = Convert.ToString(dt.Rows[i]["Status"]),
                                Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
                                ArchivedLeaves = Convert.ToDecimal(dt.Rows[i]["ArchivedLeaves"]),
                                LOP = Convert.ToDecimal(dt.Rows[i]["LOP"]),
                            };

                            //iLeaveRequestInfo.LRI = listItems;
                            //MI = listItems.ToPagedList(pageIndex, pageSize);
                            listItems.Add(iLeaveRequestInfo);
                        }

                    }
                }
            }
            ViewData["Norecords"] = "No records found";
            return View(listItems);
        }

        [HttpPost]
        [SessionTimeout]
        // FormCollection form
        public ActionResult ValidateLeaveStatus(LeaveRequestInfo iLeaveRequestInfo)
        {
            //both method methods are working
            string val = Convert.ToString(Request.Params["hdfID"]);
            string[] values = val.Split(',');
            val = values[0];
            var LeaveStatus = values[1];
            var UserID = values[2];
            var Type = values[4];
            var from  = values[5];
            var to = values[6];
            Decimal Days = Convert.ToDecimal(values[3]);
            try
            {
                using (var client = new HttpClient())
                {
                    iLeaveRequestInfo.iMode = 107;
                    iLeaveRequestInfo.Id = Convert.ToInt32(val);
                    iLeaveRequestInfo.From =Convert.ToDateTime(from);
                    iLeaveRequestInfo.To = Convert.ToDateTime(to);
                    iLeaveRequestInfo.Status = LeaveStatus;
                    iLeaveRequestInfo.UserID = UserID;
                    iLeaveRequestInfo.Days = Days;
                    iLeaveRequestInfo.Type = Type;
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetLeaveRequestDetails", iLeaveRequestInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        //ViewBag.Message = "Success";
                        //return RedirectToAction("Index");

                        string strFrom = System.Configuration.ConfigurationManager.AppSettings["RatingMDAdmin"].ToString().Trim();
                        string strTo = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"].ToString().Trim();
                        string strVerifiedMailBody = string.Empty;
                        string strSubject = string.Empty;
                        if (iLeaveRequestInfo.Status == "1")
                        {
                            strVerifiedMailBody = "Your leave request has been approved from "+ iLeaveRequestInfo.From+" to " + iLeaveRequestInfo.To;
                            strSubject = "Your Requested Leave is Approved";
                            ViewBag.Message = "Success";
                            TempData["JavaScriptFunction"] = string.Format("fnSuccess('0');");
                            // return RedirectToAction("Index");
                        }
                        else
                        {
                            strVerifiedMailBody = "Your leave request has been rejected from " + iLeaveRequestInfo.From + " to  " + iLeaveRequestInfo.To;
                            strSubject = "Your Requested Leave is Rejected";
                            ViewBag.Message = "Fail";
                            TempData["JavaScriptFunction"] = string.Format("fnSuccess('1');");
                            //return RedirectToAction("Index");
                        }
                        Status.SendActionEMail(strFrom, strTo, strSubject, strVerifiedMailBody,"LRS");
                        
                        return RedirectToAction("Index");

                    }
                    else
                        return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }
        

        private void GetLeaveStatus()
        {
            DataTable dtLeaveStatus = new DataTable();
            dtLeaveStatus.Columns.Add("LeaveStatusID", typeof(Int32));
            dtLeaveStatus.Columns.Add("LeaveDesc", typeof(string));

            DataRow dr = dtLeaveStatus.NewRow();
            dr["LeaveStatusID"] = 1;
            dr["LeaveDesc"] = "Approve";
            dtLeaveStatus.Rows.Add(dr);

            dr = dtLeaveStatus.NewRow();
            dr["LeaveStatusID"] = 2;
            dr["LeaveDesc"] = "Reject";
            dtLeaveStatus.Rows.Add(dr);

            dtLeaveStatus.AcceptChanges();

            ViewBag.LeaveStatus = dtLeaveStatus;
        }

        [SessionTimeout]
        public ActionResult Edit(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Utility.BaseURL);
                List<SelectListItem> items = new List<SelectListItem>();
                List<SelectListItem> itemName = new List<SelectListItem>();
                iMemberInfo.iMode = 112;
                var response1 = client.PostAsJsonAsync<MemberInfo>("GetDetails", iMemberInfo).Result;
                Status status1 = response1.Content.ReadAsAsync<Status>().Result;
                List<MemberInfo> model1 = (status1.Text.ToLower() == "ok") ? JsonConvert.DeserializeObject<List<MemberInfo>>(status1.Data.ToString()) : new List<MemberInfo>();
                // iLeaveRequestInfo.Name = model1;

                foreach (var item1 in model1) // Loop through List with foreach
                {

                    itemName.Add(new SelectListItem
                    {
                        Text = item1.FirstName + " " + item1.LastName,
                        Value = Convert.ToString(item1.ID)
                        // Value = Convert.ToString(item1.Id) + "^" + item1.EmailAddress
                    });
                }
                ViewData["MemberName"] = itemName;


                iLeaveTypeInfo.iMode = 113;
                var response2 = client.PostAsJsonAsync<LeaveTypeInfo>("GetLeavetypes", iLeaveTypeInfo).Result;
                Status status2 = response2.Content.ReadAsAsync<Status>().Result;
                List<LeaveTypeInfo> model = (status2.Text.ToLower() == "ok") ? JsonConvert.DeserializeObject<List<LeaveTypeInfo>>(status2.Data.ToString()) : new List<LeaveTypeInfo>();
                foreach (var item in model) // Loop through List with foreach
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.LeaveTypeName,
                        Value = Convert.ToString(item.LeaveTypeID)
                    });
                }
                var selectList = new SelectList(items, "Id", "Name", 0);
                ViewData["LeaveTypes"] = items;
                BindUsers();

                iLeaveRequestInfo.iMode = 116; 
                iLeaveRequestInfo.Id = id; //need to change with session userid
                iLeaveRequestInfo.From = DateTime.Now;
                iLeaveRequestInfo.To = DateTime.Now;
                //iLeaveRequestInfo.UserID = Convert.ToString("72");

                var response = client.PostAsJsonAsync("GetLeaveRequestDetails", iLeaveRequestInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //string sVal = string.Empty;
                            //if (Convert.ToString(dt.Rows[i]["Status"]) == "Approved/Rejected")
                            //    sVal=
                            //        else

                            iLeaveRequestInfo = new LeaveRequestInfo
                            {
                                Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                                Name = Convert.ToString(dt.Rows[i]["Name"]),
                                UserID = Convert.ToString(dt.Rows[i]["UserID"]),
                                //From = Convert.ToDateTime(dt.Rows[i]["From"]),
                                //To = Convert.ToDateTime(dt.Rows[i]["To"]),
                                DateRange = Convert.ToString(dt.Rows[i]["DateRange"]),
                                Type = Convert.ToString(dt.Rows[i]["TypeID"]),
                                Days = Convert.ToDecimal(dt.Rows[i]["Days"]),
                                Status = Convert.ToString(dt.Rows[i]["Status"]),
                                Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
                                ArchivedLeaves = Convert.ToDecimal(dt.Rows[i]["ArchivedLeaves"]),
                                LOP = Convert.ToDecimal(dt.Rows[i]["LOP"]),
                            };

                            listItems.Add(iLeaveRequestInfo);
                        }

                    }
                }
            }

            return View(iLeaveRequestInfo);
        }

        [HttpPost]
        [SessionTimeout]
        public ActionResult Edit(LeaveRequestInfo iLeaveRequestInfo, int id)
        {
           

            try
            {
                if (string.IsNullOrWhiteSpace(Convert.ToString(iLeaveRequestInfo.ArchivedLeaves)))
                    ModelState.AddModelError("ArchivedLeaves", "Please enter avail leaves");
                if (!ModelState.IsValid)
                {
                    BindUsers();
                    return View();
                }

                using (var client = new HttpClient())
                {
                    iLeaveRequestInfo.iMode = 117;
                    iLeaveRequestInfo.Id = id;
                    //iLeaveRequestInfo.UserID = Convert.ToString("72");

                    
                    //string val = Convert.ToString(Request.Params["hdfID"]);
                    //string[] values = val.Split(',');
                    //var UserID = values[2];

                    //iLeaveRequestInfo.UserID = UserID;

                    //DateTime dt = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
                    System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-us");

                    string from = Convert.ToString(iLeaveRequestInfo.DateRange.Split('-')[0].Trim());
                    DateTime dt = DateTime.Parse(from, cultureinfo);
                    string To = Convert.ToString(iLeaveRequestInfo.DateRange.Split('-')[1].Trim());
                    DateTime dtt = DateTime.Parse(To, cultureinfo);

                    //string from = Convert.ToString(iLeaveRequestInfo.DateRange.Split('-')[0]);
                    //string To = Convert.ToString(iLeaveRequestInfo.DateRange.Split('-')[1]);
                    iLeaveRequestInfo.From = Convert.ToDateTime(dt);
                    iLeaveRequestInfo.To = Convert.ToDateTime(dtt);
                    //iLeaveRequestInfo.UserID = "UserID";
                    //iLeaveRequestInfo.From = DateTime.Now;
                    //iLeaveRequestInfo.To = DateTime.Now;

                    iLeaveRequestInfo.Days = iLeaveRequestInfo.ArchivedLeaves; 

                    //int Days = Convert.ToDateTime(dt - dtt).Day;
                    //iLeaveRequestInfo.Days = Days;

                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetLeaveRequestDetails", iLeaveRequestInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        TempData["JavaScriptFunction"] = string.Format("fn_Ld_Edit('1');");
                        return RedirectToAction("Index");
                    }
                    else
                        return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Edit", new { id = id });
            }
        }

        private void BindUsers()
        {
            List<SelectListItem> itemName = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Utility.BaseURL);
                iMemberInfo.iMode = 112;
                var response1 = client.PostAsJsonAsync<MemberInfo>("GetDetails", iMemberInfo).Result;
                Status status1 = response1.Content.ReadAsAsync<Status>().Result;

                //if (string.IsNullOrWhiteSpace(iLeaveRequestInfo.Name))
                //    ModelState.AddModelError("Message", "Please enter Name");

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
                        //Value = Convert.ToString(item1.Id) + "^" + item1.EmailAddress
                    }
                }
                ViewData["MemberName"] = itemName;
            }
        }

        public string SetDate(string date)
        {
            string setdate = null;
            if (date != null && date != "")
            {
                DateTime dt = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
                setdate = dt.ToString("dd/MM/yyyy");
            }
            return setdate;
        }
    }
}
