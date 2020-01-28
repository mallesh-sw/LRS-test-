using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LRS.Controllers
{
    public class LeaveReportController : Controller
    {
        //public static string BaseURL = "http://192.168.2.48/LRS_API/";
        LeaveTypeInfo iLeaveTypeInfo = new LeaveTypeInfo();
        LeaveRequestInfo iLeaveRequestInfo = new LeaveRequestInfo();
        LeaveReportInfo iLReport = new LeaveReportInfo();
        MemberInfo iMemberInfo = new MemberInfo();
        List<LeaveReportInfo> listItems = new List<LeaveReportInfo>();
        // GET: LeaveReport

        [SessionTimeout]
        public ActionResult Index()
        {
            //if (Convert.ToString(Session["Email"]) == "")
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            return View();
        }

        //[HttpPost]
        //public ActionResult GetReportDetails()
        //{
        //    TempData["rowid"] = Convert.ToString(Request.Params["HdfVal"]);
        //    //TempData["monthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(TempData["rowid"]));
        //    using (var client = new HttpClient())
        //    {
        //        iLeaveRequestInfo.iMode = 111;
        //        iLeaveRequestInfo.Id = Convert.ToInt32(Request.Params["HdfVal"]);
        //        iLeaveRequestInfo.From = DateTime.Now;
        //        iLeaveRequestInfo.To = DateTime.Now;
        //        client.BaseAddress = new Uri(BaseURL);
        //        var response = client.PostAsJsonAsync("GetLeaveRequestDetails", iLeaveRequestInfo).Result;
        //        Status status = response.Content.ReadAsAsync<Status>().Result;
        //        if (status.Errors == null)
        //        {
        //            DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
        //            if (dt.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    //string sVal = string.Empty;
        //                    //if (Convert.ToString(dt.Rows[i]["Status"]) == "Approved/Rejected")
        //                    //    sVal=
        //                    //        else

        //                    iLeaveRequestInfo = new LeaveRequestInfo
        //                    {
        //                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
        //                        Name = Convert.ToString(dt.Rows[i]["Name"]),
        //                        From = Convert.ToDateTime(dt.Rows[i]["From"]),
        //                        To = Convert.ToDateTime(dt.Rows[i]["To"]),
        //                        Type = Convert.ToString(dt.Rows[i]["Type"]),
        //                        Days = Convert.ToDecimal(dt.Rows[i]["Days"]),
        //                        //  Status = Convert.ToString(dt.Rows[i]["Status"]),
        //                        // Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
        //                    };

        //                    listItems.Add(iLeaveRequestInfo);
        //                }

        //            }
        //        }
        //    }
        //    ViewData["Norecords"] = "No records found";
        //    return View(listItems);
        //}


        [HttpPost]
        [SessionTimeout]
        public ActionResult GetReportDetails()
        {

            using (var client = new HttpClient())
            {
                if (Convert.ToBoolean(Session["IsAdmin"]) == true)
                    //iLeaveRequestInfo.iMode = 121;
                    iLeaveRequestInfo.iMode = 127;
                else
                {
                    //iLeaveRequestInfo.iMode = 125;
                    iLeaveRequestInfo.iMode = 128;
                    iLeaveRequestInfo.Id = Convert.ToInt32(Session["UserID"]);
                }
                iLeaveRequestInfo.RecordID = Convert.ToInt32(Request.Params["HdfVal"]);
                TempData["rowid"] = Convert.ToInt32(Request.Params["HdfVal"]);
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

                            iLReport = new LeaveReportInfo
                            {
                                SNo = Convert.ToInt32(dt.Rows[i]["SNo"]),
                                EmpID = Convert.ToString(dt.Rows[i]["EmpID"]),
                                EmployeeName = Convert.ToString(dt.Rows[i]["EmployeeName"]),
                                Location = Convert.ToString(dt.Rows[i]["Location"]),
                                Trig_PL = Convert.ToDecimal(dt.Rows[i]["Trig_PL"]),
                                Trig_SL = Convert.ToDecimal(dt.Rows[i]["Trig_SL"]),
                                //  Status = Convert.ToString(dt.Rows[i]["Status"]),
                                // Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
                                PLs = Convert.ToDecimal(dt.Rows[i]["PLs"]),
                                SLs = Convert.ToDecimal(dt.Rows[i]["SLs"]),
                                Coff = Convert.ToDecimal(dt.Rows[i]["Coffs"]),
                                MLs = Convert.ToDecimal(dt.Rows[i]["MLs"]),
                                Coff_gd = Convert.ToDecimal(dt.Rows[i]["Coff_gd"]),
                                PL_use = Convert.ToDecimal(dt.Rows[i]["PL_use"]),


                                SL_use = Convert.ToDecimal(dt.Rows[i]["SL_use"]),
                                Coff_use = Convert.ToDecimal(dt.Rows[i]["Coff_use"]),
                                MLs_use = Convert.ToDecimal(dt.Rows[i]["MLs_use"]),
                                PLs_bal = Convert.ToDecimal(dt.Rows[i]["PLs_bal"]),
                                SLs_bal = Convert.ToDecimal(dt.Rows[i]["SLs_bal"]),
                                Coff_bal = Convert.ToDecimal(dt.Rows[i]["Coff_bal"]),
                                MLs_bal = Convert.ToDecimal(dt.Rows[i]["MLs_bal"]),
                                LOP = Convert.ToDecimal(dt.Rows[i]["LOP"]),
                            };

                            listItems.Add(iLReport);
                        }

                    }
                }
            }
            ViewData["Norecords"] = "No records found";
            return View(listItems);
        }

        [HttpPost]
        [SessionTimeout]
        public ActionResult DownloadExcel()
        {
            var gv = new GridView();
            gv.DataSource = this.GetReportList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=export.html");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);
            string headerTable2 = @"<Table style=""width:100%;text-align:center""><tr><td ><b style=""font-size:14px;font-family:monospace;"">Maple Software Pvt. Ltd. - Attendance for the month of " + TempData["monthName"] + "-" + DateTime.Parse(DateTime.Now.ToString()).Year.ToString() + " </b></td> <tr><td>&nbsp; </td></tr> </Table>";
            Response.Write(headerTable2);

            string headerTable = @"<Table style=""width:100%;text-align:center""><tr><td ><b style=""font-size:18px;font-family:monospace;text-align:center"">iCheckup Team Leave record for month of " + TempData["monthName"] + "-" + DateTime.Parse(DateTime.Now.ToString()).Year.ToString() + " </b></td> <tr><td>&nbsp; </td></tr> </Table>";
            Response.Write(headerTable);

            //string headerTable3 = @"<table style=""width:100%"" class=""table table-responsive table-bordered table-condensed table-striped""><tr><td colspan=""6"" style=""background-color:red;"">&nbsp;</td><td colspan=""4"" style=""background-color:lightgreen;"">Apr-2019(starting)-Avl</td><td colspan=""5"" style=""background-color:lightgoldenrodyellow;"">Apr-2019(Used) </td><td colspan=""5"" style=""background-color:aliceblue;"">Apr-2019(Ending)-Bal</td></tr></table>";
            //Response.Write(headerTable3);



            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index", "LeaveReport");
        }

        public DataTable GetReportList()
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                iLeaveRequestInfo.iMode = 121;
                TempData["monthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(TempData["rowid"]));
                iLeaveRequestInfo.RecordID = Convert.ToInt32(TempData["rowid"]); // Convert.ToInt32(Request.Params["HdfVal"]);
                iLeaveRequestInfo.From = DateTime.Now;
                iLeaveRequestInfo.To = DateTime.Now;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetLeaveRequestDetails", iLeaveRequestInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    //if (dt.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        //string sVal = string.Empty;
                    //        //if (Convert.ToString(dt.Rows[i]["Status"]) == "Approved/Rejected")
                    //        //    sVal=
                    //        //        else

                    //        iLeaveRequestInfo = new LeaveRequestInfo
                    //        {
                    //            // Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                    //            Name = Convert.ToString(dt.Rows[i]["Name"]),
                    //            From = Convert.ToDateTime(dt.Rows[i]["From"]),
                    //            To = Convert.ToDateTime(dt.Rows[i]["To"]),
                    //            Type = Convert.ToString(dt.Rows[i]["Type"]),
                    //            Days = Convert.ToDecimal(dt.Rows[i]["Days"]),
                    //            //Status = Convert.ToString(dt.Rows[i]["Status"]),
                    //            //Planned = Convert.ToBoolean(dt.Rows[i]["Planned"]),
                    //        };

                    //        listItems.Add(iLeaveRequestInfo);
                    //    }

                    //}
                }
            }
            return dt;
        }
    }
}