using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace IntershipManagement.Areas.Admin.Controllers
{
    public class CourseClassController : BaseController
    {
        // GET: Admin/CourseClass
        public ActionResult Index()
        {
            var courseClasses = db.CourseClasses.ToList();

            var model = courseClasses.Select(c => new CourseClassViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                StudentCount = c.StudentClasses.Count(),
                StartDate = c.StartDate,
                EndDate = c.EndDate
            }).ToList();

            return View(model);
        }

        // GET: Admin/CourseClass/Create
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/CourseClass/Create.cshtml", null);
        }

        // POST: Admin/CourseClass/Create
        [HttpPost]
        public async Task<ActionResult> CreateConfirm(CourseClassViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var courseClass = new CourseClass
                    {
                        Name = model.Name,
                        Code = model.Code,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };

                    db.CourseClasses.Add(courseClass);
                    await db.SaveChangesAsync();
                    SetAlert("Lớp học đã được tạo thành công.", "success");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }
            SetAlert("Tạo lớp học không thành công. Hãy kiểm tra lại thông tin.", "danger");
            return View(model);
        }
        // GET: Admin/CourseClass/Edit/{id}
        public ActionResult Edit(int id)
        {
            var courseClass = db.CourseClasses.Find(id);
            if (courseClass == null)
            {
                return HttpNotFound();
            }

            var model = new CourseClassViewModel
            {
                Id = courseClass.Id,
                Name = courseClass.Name,
                Code = courseClass.Code,
                StartDate = courseClass.StartDate,
                EndDate = courseClass.EndDate,
                Teachers = GetTeachersSelectList()
            };

            return View(model);
        }

        // POST: Admin/CourseClass/Edit/{id}
        [HttpPost]
        public async Task<ActionResult> Edit(CourseClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var courseClass = db.CourseClasses.Find(model.Id);
                if (courseClass == null)
                {
                    return HttpNotFound();
                }

                courseClass.Name = model.Name;
                courseClass.Code = model.Code;
                courseClass.StartDate = model.StartDate;
                courseClass.EndDate = model.EndDate;

                await db.SaveChangesAsync();
                SetAlert("Lớp học đã được cập nhật thành công.", "success");
                return RedirectToAction("Index");
            }

            SetAlert("Cập nhật lớp học không thành công. Hãy kiểm tra lại thông tin.", "danger");
            model.Teachers = GetTeachersSelectList();
            return View(model);
        }
        private IEnumerable<SelectListItem> GetTeachersSelectList()
        {
            return db.AspNetUsers.Where(u => u.IsTeacher == true).Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            });
        }
    }
}