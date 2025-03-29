using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        static IEnumerable<UserView> listexc = null;
        static List<AspNetUser> listerror = new List<AspNetUser>();
        ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin/User
        public ActionResult Index(int? page, string textsearch, string chanel, string statuss, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where a.IsDelete != 1
                        select new UserView()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            UserName = a.UserName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Editdate = a.Editdate,
                            Editby = a.Editby,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Role = b.Name,
                            FullName = a.FullName,
                            IDCARD = a.IDCARD,
                            StatusConfirm = a.StatusConfirm,
                            MajorName = db.Majors.FirstOrDefault(c => c.Id == a.MajorId).Name,
                        };
            ViewBag.countUser = model.Count();
            //hiển thị theo quyền
            if (User.IsInRole("Giáo viên"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name || a.UserName == User.Identity.Name);
                ViewBag.countStudent = model.Count();
            }
            if (User.IsInRole("Sinh viên"))
            {
                model = model.Where(a => a.UserName == User.Identity.Name);
            }
            //lọc theo thông tin 
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.UserName.Contains(textsearch)
                || a.PhoneNumber.Contains(textsearch)
                || a.Email.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.FullName.Contains(textsearch)
                || a.Role.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Role.Contains(chanel));
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(statuss))
            {
                model = model.Where(a => a.StatusConfirm == 1);
                ViewBag.status = statuss;
            }
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            ViewBag.role = db.AspNetRoles.ToList();
            listexc = model as IEnumerable<UserView>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(listexc.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin - Quản trị toàn hệ thống"))
            {
                ViewBag.role = context.Roles.Where(a => a.Id == "Teacher").ToList();
            }
            else if (User.IsInRole("Giáo viên"))
            {
                ViewBag.role = context.Roles.Where(a => a.Id == "Student").ToList();
            }
            return PartialView("~/Areas/Admin/Views/User/Create.cshtml", null);
        }
        [HttpPost]
        public ActionResult Edit(string Id)
        {
            ViewBag.role = context.Roles.ToList();
            var model = db.AspNetUsers.Find(Id);
            return PartialView("~/Areas/Admin/Views/User/Edit.cshtml", model);
        }
        [HttpPost]
        public ActionResult EditConfirm([Bind(Include = "")] AspNetUser _user, string Roler)
        {
            if (ModelState.IsValid)
            {
                var user = db.AspNetUsers.Find(_user.Id);
                if (User.IsInRole("Admin - Quản trị toàn hệ thống"))
                {
                    string r = user.AspNetRoles.FirstOrDefault().Name;
                    if (!user.AspNetRoles.FirstOrDefault().Name.Equals(Roler))
                    {
                        ApplicationUser u = context.Users.Find(user.Id);
                        //xoa di roi add lai la duoc
                        u.Roles.Remove(u.Roles.FirstOrDefault());
                        u.Roles.Add(new IdentityUserRole() { UserId = user.Id, RoleId = Roler });
                        context.SaveChanges();
                    }
                }
                user.PhoneNumber = _user.PhoneNumber;
                user.Address = _user.Address;
                int idProvince = 0, idDistrict = 0, idWard = 0;
                string _province = "", _district = "", _ward = "";
                if (_user.Province != null && _user.Province != user.Province)
                {
                    idProvince = int.Parse(_user.Province);
                    _province = db.Provinces.FirstOrDefault(c => c.Id == idProvince).Name;
                    user.Province = _province;
                }
                else
                {
                    user.Province = _user.Province;
                }
                if (_user.District != null && _user.District != user.District)
                {
                    idDistrict = int.Parse(_user.District);
                    _district = db.Districts.FirstOrDefault(c => c.Id == idDistrict).Name;
                    user.District = _district;
                }
                else
                {
                    user.District = _user.District;
                }
                if (_user.Ward != null && _user.Ward != user.Ward)
                {
                    idWard = int.Parse(_user.Ward);
                    _ward = db.Wards.FirstOrDefault(c => c.Id == idWard).Name;
                    user.Ward = _ward;
                }
                else
                {
                    user.Ward = _user.Ward;
                }
                user.Code = _user.Code;
                user.Email = _user.Email;
                user.FullName = _user.FullName;
                user.Editdate = DateTime.Now;
                user.Editby = User.Identity.Name;
                db.Entry(user).State = EntityState.Modified;
                var log_edit = new Edit_Log()
                {
                    Id_User = user.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " đã chỉnh sửa thông tin tài khoản."
                };
                db.Edit_Log.Add(log_edit);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                ViewBag.role = context.Roles.ToList();
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            ViewBag.role = context.Roles.ToList();
            return RedirectToAction("Index");
        }
        public ActionResult UploadFile()
        {
            List<AspNetUser> list_users = new List<AspNetUser>();
            return View(list_users);
        }
        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            listerror.Clear();
            List<AspNetUser> list_users = new List<AspNetUser>();
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            string user;
                            string password;
                            string fullname;
                            string phone;
                            string address;
                            string ward;
                            string district;
                            string province;
                            string email;
                            string role;
                            string code;
                            string majorCode;


                            try { user = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { user = ""; }
                            try { password = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { password = ""; }
                            try { fullname = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { fullname = ""; }
                            try { phone = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { phone = ""; }
                            try { address = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { address = ""; }
                            try { ward = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { ward = ""; }
                            try { district = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { district = ""; }
                            try { province = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { province = ""; }
                            try { email = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { email = ""; }
                            try { role = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { role = ""; }
                            try { code = workSheet.Cells[rowIterator, 11].Value.ToString(); } catch (Exception) { code = ""; }
                            try { majorCode = workSheet.Cells[rowIterator, 12].Value.ToString(); } catch (Exception) { majorCode = ""; }


                            //add thong tin rows
                            var users = new AspNetUser()
                            {
                                UserName = user,
                                Address = address + " " + ward + " " + district + " " + province,
                                PhoneNumber = phone,
                                Email = email,
                                FullName = fullname,
                            };
                            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(code))
                            {
                                var drole = db.AspNetRoles.Where(a => a.Id == role);
                                if (drole.Count() == 0)
                                {
                                    users.FullName = "phân quyền k đúng";
                                    listerror.Add(users);
                                    continue;
                                }
                                var checkCode = db.AspNetUsers.FirstOrDefault(c => c.Code == code);
                                if (checkCode != null)
                                {
                                    users.FullName = "mã định danh đã tồn tại";
                                    listerror.Add(users);
                                    continue;
                                }
                                else
                                {
                                    //tao tai khoan
                                    try
                                    {
                                        var majorId = -1;
                                        if (!string.IsNullOrEmpty(majorCode))
                                        {
                                            var major = db.Majors.FirstOrDefault(c => c.Code == majorCode && c.IsUse != 1);
                                            if (major == null)
                                            {
                                                users.FullName = "mã CN k tồn tại/ đã được phân công";
                                                listerror.Add(users);
                                                continue;
                                            }
                                            else
                                            {
                                                major.IsUse = 1;
                                                db.Entry(major).State = EntityState.Modified;
                                                majorId = major.Id;
                                            }
                                        }
                                        var asp = new ApplicationUser
                                        {
                                            UserName = user,
                                            PhoneNumber = phone,
                                            Email = email
                                        };

                                        var result = UserManager.Create(asp, password);
                                        if (result.Succeeded)
                                        {
                                            //add quyền cho tài khoản luôn
                                            ApplicationUser u = context.Users.Find(asp.Id);
                                            u.Roles.Add(new IdentityUserRole() { UserId = asp.Id, RoleId = role });
                                            context.SaveChanges();

                                            //bổ sung thong tin tài khoản
                                            var _user = db.AspNetUsers.Find(asp.Id);
                                            _user.Address = address;
                                            _user.Ward = ward;
                                            _user.District = district;
                                            _user.Province = province;
                                            _user.Createby = User.Identity.Name;
                                            _user.Createdate = DateTime.Now;
                                            _user.FullName = fullname;
                                            _user.MajorId = majorId;
                                            if(role == "Teacher")
                                            {
                                                _user.IsTeacher = true;
                                            }
                                            if (role == "Student")
                                            {
                                                _user.IsStudent = true;
                                            }
                                            db.Entry(_user).State = EntityState.Modified;
                                            users.Id = asp.Id;
                                        }
                                        else
                                        {
                                            users.FullName = "không tạo được tài khoản";
                                            listerror.Add(users);
                                            continue;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        users.FullName = ex.Message;
                                        listerror.Add(users);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                users.FullName = "nhập thiếu thông tin đầu vào";
                                listerror.Add(users);
                            }
                            list_users.Add(users);
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(list_users);
        }
        public void Export_User()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Tài khoản";
            Sheet.Cells["C1"].Value = "Tên tài khoản";
            Sheet.Cells["D1"].Value = "Quyền";
            Sheet.Cells["E1"].Value = "Số điện thoại";
            Sheet.Cells["F1"].Value = "Email";
            Sheet.Cells["G1"].Value = "Địa chỉ";
            Sheet.Cells["H1"].Value = "Ngày tạo";

            int index = 1;
            int row = 2;
            foreach (var item in listexc)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FullName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Role;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PhoneNumber;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void UploadFail()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Tài khoản lỗi";
            Sheet.Cells["C1"].Value = "Mã lỗi";
            int index = 1;
            int row = 2;
            foreach (var item in listerror)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FullName;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportUploadFail.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        [HttpPost]
        public ActionResult ViewLog(string userId)
        {
            var log = db.Edit_Log.Where(a => a.Id_User == userId);
            ViewBag.UserName = db.AspNetUsers.Find(userId).UserName;
            return PartialView("~/Areas/Admin/Views/User/Viewlog.cshtml", log.ToList());
        }
        [HttpPost]
        public ActionResult GetTeacher()
        {
            var result = from a in db.AspNetUsers
                         from b in a.AspNetRoles
                         where b.Id == "Giáo viên"
                         select a;
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetMajor()
        {
            try
            {
                var result = db.Majors.Select(m => new { m.Id, m.Name }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Có lỗi xảy ra: " + ex.Message);
            }
        }
        [HttpPost]
        public JsonResult GetProvince()
        {
            var province = (from a in db.Provinces
                            orderby a.Name
                            select new { a.Name, a.Id });
            return Json(province, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDistrict(int text)
        {
            var district = (from a in db.Districts
                            orderby a.Name
                            //where a.Province.Name.Equals(text)
                            where a.Province.Id == text
                            select new { a.Name, a.Id });
            return Json(district, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetWard(int text)
        {
            var ward = (from a in db.Wards
                        orderby a.Name
                        //where a.District.Name.Equals(text)
                        where a.District.Id == text
                        select new { a.Name, a.Id });
            return Json(ward, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string Id)
        {
            try
            {
                var model = db.AspNetUsers.Find(Id);
                db.AspNetUsers.Remove(model);
                db.SaveChanges();
                SetAlert("Xóa tài khoản thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }
            ViewBag.role = db.AspNetRoles.ToList();
            return RedirectToAction("Index");
        }
    }
}