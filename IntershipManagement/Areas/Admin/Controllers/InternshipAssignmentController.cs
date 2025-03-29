using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class InternshipAssignmentController : BaseController
    {
        // GET: Admin/InternshipAssignment
        public ActionResult Index()
        {
            var model = from a in db.InternshipApplications
                         .Include(i => i.CourseClass)
                        select new InternshipApplicationViewModel()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Name = a.Name,
                            ApplicationDate = a.ApplicationDate,
                            CourseClass = a.CourseClass,
                            CourseClassId = a.CourseClassId,
                            Remarks = a.Remarks,
                            Status = a.Status,
                            StudentCount = a.CourseClass.StudentClasses.Count(),
                            TeacherName = db.AspNetUsers.FirstOrDefault(c => c.Id == a.TeacherId).FullName
                        };
            return View(model);
        }
        // GET: Admin/InternshipAssignment/Create
        public ActionResult Create()
        {
            var viewModel = new InternshipApplicationViewModel
            {
                CourseClasses = db.CourseClasses
                        .Where(cc => !db.InternshipApplications
                                        .Select(ia => ia.CourseClassId)
                                        .Contains(cc.Id)) // Lọc các CourseClasses chưa được phân công
                        .Select(t => new ClassDto
                        {
                            Id = t.Id,
                            NameCode = t.Name + " - Lớp " + t.Code,
                        }).ToList(),
                Teachers = db.AspNetUsers
                             .Where(c => c.IsTeacher == true)
                             .Select(t => new TeacherDto
                             {
                                 Id = t.Id,
                                 FullNameWithMajor = t.FullName + " - " + (t.MajorId != null ? db.Majors.FirstOrDefault(m => m.Id == t.MajorId).Name : "Chưa có chuyên ngành")
                             }).ToList(),
            };

            return PartialView("~/Areas/Admin/Views/InternshipAssignment/Create.cshtml", viewModel);
        }


        // POST: Admin/InternshipAssignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm(InternshipApplicationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var internshipApplication = new InternshipApplication
                    {
                        CourseClassId = model.CourseClassId,
                        Code = model.Code,
                        Name = model.Name,
                        TeacherId = model.TeacherId,
                        MajorId = db.AspNetUsers.FirstOrDefault(m => m.Id == model.TeacherId).MajorId,
                        ApplicationDate = DateTime.Now,
                        Status = "Pending",
                        Remarks = model.Remarks
                    };

                    db.InternshipApplications.Add(internshipApplication);
                    db.SaveChanges();

                    SetAlert("Đã lưu thông tin thành công.", "success");
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception ex)
            {
                
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
    }
}