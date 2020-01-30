using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using LRS;
using Model;
using PagedList;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace LRS.Controllers
{
    public class MemberDetailsController : Controller
    {
        // public static string BaseURL = "http://192.168.2.48/LRS_API/";
        List<MemberInfo> listMember = new List<MemberInfo>();
        MemberInfo iMemberInfo = new MemberInfo();
        // GET: MemberDetails
        [SessionTimeout]
        public ActionResult Index(int? page)
        {
            //if (Convert.ToString(Session["Email"]) =="")
            //{
            //    return RedirectToAction("Index", "Login"); 
            //}

            //int pageSize = 5;
            //int pageIndex = 1;
            //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //IPagedList<MemberInfo> MI = null;

            using (var client = new HttpClient())
            {
                iMemberInfo.iMode = 102;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetMemberDetails", iMemberInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        //foreach(DataRow dr in dt.Rows)
                        {
                            iMemberInfo = new MemberInfo
                            {
                                ID = Convert.ToInt32(dt.Rows[i]["Id"]),
                                EmpID = Convert.ToString(dt.Rows[i]["EmpID"]),
                                FirstName = Convert.ToString(dt.Rows[i]["FirstName"]),
                                LastName = Convert.ToString(dt.Rows[i]["LastName"]),
                                PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]),
                                EmailAddress = Convert.ToString(dt.Rows[i]["EmailAddress"]),
                                Address = Convert.ToString(dt.Rows[i]["Address"]),
                                isActive = Convert.ToBoolean(dt.Rows[i]["isActive"]),
                            };

                            //iMemberInfo = new MemberInfo
                            //{
                            //    ID = Convert.ToInt32(dr["ID"]),
                            //    EmpID = Convert.ToString(dr["EmpID"]),
                            //    FirstName = Convert.ToString(dr["FirstName"]),
                            //    LastName = Convert.ToString(dr["LastName"]),
                            //    PhoneNumber = Convert.ToString(dr["PhoneNumber"]),
                            //    EmailAddress = Convert.ToString(dr["EmailAddress"]),
                            //    Address = Convert.ToString(dr["Address"]),
                            //    isActive = Convert.ToBoolean(dr["isActive"]),
                            //};

                            //iMemberInfo.std = listMember;
                            //MI = listMember.ToPagedList(pageIndex, pageSize);
                            listMember.Add(iMemberInfo);
                        }
                    }
                }
            }
            ViewData["Norecords"] = "No records found";
            //return View(MI);
            return View(listMember);
        }

        [SessionTimeout]
        public ActionResult Create(string displaymessage = "")
        {
            ViewData["SuccessMessage"] = displaymessage;
            return View();
        }

        [HttpPost]
        [SessionTimeout]
        public ActionResult Create(MemberInfo iMemberInfo)
        {

            if (string.IsNullOrWhiteSpace(iMemberInfo.FirstName))
                ModelState.AddModelError("FirstName", "Please enter first name");

            if (string.IsNullOrWhiteSpace(iMemberInfo.LastName))
                ModelState.AddModelError("LastName", "Please enter last name");

            //if (string.IsNullOrWhiteSpace(iMemberInfo.Password))
            //    ModelState.AddModelError("Password", "Please enter Password");

            if (string.IsNullOrWhiteSpace(iMemberInfo.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Please enter Phone Number");
            else
            {
                //iMemberInfo.PhoneNumber = new string(iMemberInfo.PhoneNumber.Where(char.IsDigit).ToArray());

                //if (!Regex.IsMatch(iMemberInfo.PhoneNumber, @"^[0-9]*$"))
                //    ModelState.AddModelError("PhoneNumber", "Enter a valid phone number");
                string PhoneRegex = @"^([0-9]{10})$";
                Regex phonere = new Regex(PhoneRegex);
                if (!phonere.IsMatch(iMemberInfo.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Enter a valid phone number");
                }
                else if (iMemberInfo.PhoneNumber.Length != 10)
                    ModelState.AddModelError("PhoneNumber", "Phone number should be 10 digits");
            }
            if (!string.IsNullOrEmpty(iMemberInfo.EmailAddress))
            {
                string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(iMemberInfo.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "Please Enter Correct Email Address");
                }
            }
            else if (string.IsNullOrWhiteSpace(iMemberInfo.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "Please enter EmailAddress");
            }

            if (string.IsNullOrWhiteSpace(iMemberInfo.Address))
                ModelState.AddModelError("Address", "Please enter address");

            if (!(iMemberInfo.isActive))
                ModelState.AddModelError("isActive", "Please check Active");
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                using (var client = new HttpClient())
                {
                    iMemberInfo.iMode = 101;
                    //iMemberInfo.Id = iMemberInfo.Id;
                    // iMemberInfo.iMode = 122;
                   client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetMemberDetails", iMemberInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    if (status.Errors == null)
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnSuccess('0');");
                        //return RedirectToAction("Index");
                        return View();
                        //  return RedirectToAction("Create", new { displaymessage = "Success" });
                    }
                    else
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnSuccess('1');");
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Create", new { message = "Failed" });
            }
        }

        [SessionTimeout]
        public ActionResult Edit(int id, string displaymessage = "")
        {
            using (var client = new HttpClient())
            {
                iMemberInfo.iMode = 103;
                iMemberInfo.ID = id;
                client.BaseAddress = new Uri(Utility.BaseURL);
                var response = client.PostAsJsonAsync("GetMemberDetails", iMemberInfo).Result;
                Status status = response.Content.ReadAsAsync<Status>().Result;
                if (status.Errors == null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(status.Data.ToString(), (typeof(DataTable)));
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            iMemberInfo = new MemberInfo
                            {
                                ID = Convert.ToInt32(dt.Rows[i]["ID"]),
                                FirstName = Convert.ToString(dt.Rows[i]["FirstName"]),
                                LastName = Convert.ToString(dt.Rows[i]["LastName"]),
                                PhoneNumber = Convert.ToString(dt.Rows[i]["PhoneNumber"]),
                                EmailAddress = Convert.ToString(dt.Rows[i]["EmailAddress"]),
                                Address = Convert.ToString(dt.Rows[i]["Address"]),
                                EmpID = Convert.ToString(dt.Rows[i]["EmpID"]),
                                //Password = Convert.ToString(dt.Rows[i]["Password"]),
                                isActive = Convert.ToBoolean(dt.Rows[i]["isActive"]),
                                //isAdmin = Convert.ToBoolean(dt.Rows[i]["isAdmin"]),
                            };
                        }
                    }
                }
            }
            //ViewData["SuccessMessage"] = displaymessage;
            return View(iMemberInfo);
        }

        [HttpPost]
        [SessionTimeout]
        public ActionResult Edit(MemberInfo iMemberInfo, int id)
        {
            if (string.IsNullOrWhiteSpace(iMemberInfo.FirstName))
                ModelState.AddModelError("FirstName", "Please enter first name");

            if (string.IsNullOrWhiteSpace(iMemberInfo.LastName))
                ModelState.AddModelError("LastName", "Please enter last name");

            if (string.IsNullOrWhiteSpace(iMemberInfo.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Please enter Phone Number");
            else
            {
                //iMemberInfo.PhoneNumber = new string(iMemberInfo.PhoneNumber.Where(char.IsDigit).ToArray());
                //if (!Regex.IsMatch(iMemberInfo.PhoneNumber, @"^[0-9]*$"))
                //    ModelState.AddModelError("PhoneNumber", "Enter a valid phone number");
                string PhoneRegex = @"^([0-9]{10})$";
                Regex phonere = new Regex(PhoneRegex);
                if (!phonere.IsMatch(iMemberInfo.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Enter a valid phone number");
                }
                else if (iMemberInfo.PhoneNumber.Length != 10)
                    ModelState.AddModelError("PhoneNumber", "Phone number should be 10 digits");
            }
            if (!string.IsNullOrEmpty(iMemberInfo.EmailAddress))
            {
                string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(iMemberInfo.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "Please Enter Correct Email Address");
                }
            }
            else if (string.IsNullOrWhiteSpace(iMemberInfo.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "Please enter EmailAddress");
            }

            if (string.IsNullOrWhiteSpace(iMemberInfo.Address))
                ModelState.AddModelError("Address", "Please enter address");

            if (!(iMemberInfo.isActive))
                ModelState.AddModelError("isActive", "Please check Active");
            if (!ModelState.IsValid)
            {

                //write code to update student 

                return View();
            }

            try
            {
                using (var client = new HttpClient())
                {
                    iMemberInfo.iMode = 101;
                    iMemberInfo.ID = id;
                    iMemberInfo.EmpID = iMemberInfo.EmpID; 
                    client.BaseAddress = new Uri(Utility.BaseURL);
                    var response = client.PostAsJsonAsync("SetMemberDetails", iMemberInfo).Result;
                    Status status = response.Content.ReadAsAsync<Status>().Result;
                    //if (status.Errors == null)
                    //{
                    //    return RedirectToAction("Index");
                    //}
                    //else
                    //    return View("Index");

                    if (status.Errors == null)
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnEditSuccess('0');");
                        return RedirectToAction("Index");
                        //  return RedirectToAction("Create", new { displaymessage = "Success" });
                    }
                    else
                    {
                        TempData["JavaScriptFunction"] = string.Format("fnEditSuccess('1');");
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Edit");
            }

        }

        //[AllowAnonymous]
        //public async Task<JsonResult> UserAlreadyExistsAsync(string email)
        //{
        //    var result =
        //        await userManager.FindByNameAsync(email) ??
        //        await userManager.FindByEmailAsync(email);
        //    return Json(result == null, JsonRequestBehavior.AllowGet);
        //}
        //30/1/20
    }
}
