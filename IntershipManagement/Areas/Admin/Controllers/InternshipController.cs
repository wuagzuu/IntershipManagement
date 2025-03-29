using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class InternshipController : BaseController
    {
        // GET: Admin/Internship
        public ActionResult Index(int? page)
        {
            var model = from a in db.Internships
                        select new InternshipViews()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            StudentName = a.StudentName,
                            StudentCode = a.StudentCode,
                            StudentId = a.StudentId,
                            TeacherName = a.TeacherName,
                            TeacherId = a.TeacherId,
                            MajorId = a.MajorId,
                            MajorName = a.MajorName,
                            EnterpriseName = a.EnterpriseName,
                            EnterpriseId = a.EnterpriseId,
                            JobId = a.JobId,
                            JobName = a.JobName,
                            Status = a.Status,
                            IsConfirm = a.IsConfirm,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Editby = a.Editby,
                            Editdate = a.Editdate,
                        };
            if (User.IsInRole("Giáo viên"))
            {
                var teacherId = User.Identity.GetUserId();
                model = model.Where(a => a.TeacherId == teacherId);
            }
            if (User.IsInRole("Sinh viên"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(c => c.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var model = new InternshipViews();
            var isInternship = CheckInternship(userId);
            if (!isInternship)
            {
                return Json(new { success = false, message = "Bạn chưa được phân công thực tập hoặc phiếu thực tập đã được duyệt trước đó. Hãy kiểm tra lại." }, JsonRequestBehavior.AllowGet);
            }
            // Lấy thông tin sinh viên
            var student = db.AspNetUsers.Find(userId);
            if (student != null)
            {
                model.StudentId = student.Id;
                model.StudentName = student.FullName;
                model.StudentCode = student.Code;

                // Lấy thông tin major
                var major = db.Majors.Find(student.MajorId);
                if (major != null)
                {
                    model.MajorId = major.Id;
                    model.MajorName = major.Name;
                    model.MajorCode = major.Code;
                }

                // Lấy thông tin teacher
                var teacher = db.AspNetUsers.FirstOrDefault(c => c.UserName == student.Createby);
                if (teacher != null)
                {
                    model.TeacherId = teacher.Id;
                    model.TeacherName = teacher.FullName;
                    model.TeacherCode = teacher.Code;
                }
            }

            // Lấy danh sách enterprise
            model.ListEnterprise = db.Enterprises.ToList();
            return PartialView("~/Areas/Admin/Views/Internship/Create.cshtml", model);
        }
        [HttpPost]
        public ActionResult CreateConfirm(InternshipViews model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _model = new Internship()
                    {
                        Code = Guid.NewGuid().ToString("N").Substring(0, 8),
                        TeacherId = model.TeacherId,
                        TeacherName = model.TeacherName,
                        StudentId = model.StudentId,
                        StudentName = model.StudentName,
                        StudentCode = model.StudentCode,
                        MajorId = model.MajorId,
                        MajorName = model.MajorName,
                        EnterpriseId = model.EnterpriseId,
                        EnterpriseName = model.EnterpriseName,
                        JobId = model.JobId,
                        JobName = model.JobName,
                        IsConfirm = 0,//chờ duyệt
                        Status = 0,//mới tạo
                        Createby = User.Identity.Name,
                        Createdate = DateTime.Now,

                    };
                    db.Internships.Add(_model);
                    db.SaveChanges();
                    SetAlert("Đã đăng ký thông tin thành công.", "success");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }
            
            SetAlert("Đăng ký thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int Id)
        {
            var model = db.Internships.Find(Id);
            var _model = new InternshipViews()
            {
                Id = model.Id,
                Code = model.Code,
                StudentId = model.StudentId,
                StudentName = model.StudentName,
                StudentCode = model.StudentCode,
                TeacherId = model.TeacherId,
                TeacherName = model.TeacherName,
                MajorId = model.MajorId,
                MajorName = model.MajorName,
                Createdate = model.Createdate,
                Createby = model.Createby,
                Status = model.Status,
                IsConfirm = model.IsConfirm,
            };
            // Lấy danh sách enterprise
            _model.ListEnterprise = db.Enterprises.ToList();
            return PartialView("~/Areas/Admin/Views/Internship/Edit.cshtml", _model);
        }
        [HttpPost]
        public ActionResult EditConfirm(InternshipViews model)
        {
            if (ModelState.IsValid)
            {
                var _model = new Internship()
                {
                    Id = model.Id,
                    Code = model.Code,
                    TeacherId = model.TeacherId,
                    TeacherName = model.TeacherName,
                    StudentId = model.StudentId,
                    StudentName = model.StudentName,
                    StudentCode = model.StudentCode,
                    MajorId = model.MajorId,
                    MajorName = model.MajorName,
                    EnterpriseId = model.EnterpriseId,
                    EnterpriseName = model.EnterpriseName,
                    JobId = model.JobId,
                    JobName = model.JobName,
                    IsConfirm = 0,//chờ duyệt
                    Status = 0,//mới tạo
                    Createby = model.Createby,
                    Createdate = model.Createdate,
                    Editby = User.Identity.Name,
                    Editdate = DateTime.Now,

                };
                db.Entry(_model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Confirm(int Id)
        {
            try
            {
                var model = db.Internships.Find(Id);
                if (model == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu" });
                }

                model.IsConfirm = 1;
                model.Status = 1;
                model.Confirmby = User.Identity.Name;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, data = model });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetJobsByEnterprise(int enterpriseId, int majorId)
        {
            var jobs = db.Jobs
                .Where(j => j.EnterpriseId == enterpriseId && j.Slot > 0 && j.MajorId == majorId)
                .Select(j => new
                {
                    Id = j.Id,
                    Name = j.Name,
                    Slot = j.Slot,
                })
                .ToList();
            return Json(jobs, JsonRequestBehavior.AllowGet);
        }
    }
}