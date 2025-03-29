using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class StudentClassController : BaseController
    {
        // GET: Admin/StudentClass
        public ActionResult Index()
        {
            return View();
        }
        // GET: Admin/StudentClass/Assign
        public ActionResult Assign(int courseClassId)
        {
            var students = from a in db.StudentClasses
                           where a.ClassId == courseClassId
                           join u in db.AspNetUsers on a.StudentId equals u.Id
                           join c in db.CourseClasses on a.ClassId equals c.Id
                           select new AssignViewModel
                           {
                               Id = a.Id,
                               StudentId = a.StudentId,
                               ClassId = a.ClassId,
                               EnrollmentDate = a.EnrollmentDate,
                               StudentName = u.FullName,
                               ClassName = c.Code,
                               GroupName = c.Name
                           };

            var model = new AssignStudentsViewModel
            {
                CourseClassId = courseClassId,
                Students = GetStudentsSelectList(),
                ListStudents = students.ToList()
            };

            return View(model);
        }
        // POST: Admin/StudentClass/Assign
        [HttpPost]
        public async Task<ActionResult> AssignConfirm(AssignStudentsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var studentId in model.SelectedStudentIds)
                    {
                        var studentClass = new StudentClass
                        {
                            ClassId = model.CourseClassId,
                            StudentId = studentId,
                            EnrollmentDate = DateTime.UtcNow
                        };

                        db.StudentClasses.Add(studentClass);
                    }

                    await db.SaveChangesAsync();
                    SetAlert("Sinh viên đã được phân lớp thành công.", "success");
                    return RedirectToAction("Index", "CourseClass");
                }
            }
            catch (Exception ex)
            {

            }
            SetAlert("Phân lớp không thành công. Hãy kiểm tra lại thông tin.", "danger");
            return View(model);
        }
        public ActionResult UploadFile()
        {
            List<AssignViewModel> list_users = new List<AssignViewModel>();
            return View(list_users);
        }
        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            var listUsers = new List<AssignViewModel>();
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName))
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            throw new Exception("No worksheet found in uploaded file.");

                        var noOfRows = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= noOfRows; row++)
                        {
                            string groupName = worksheet.Cells[row, 1]?.Text?.Trim();
                            string userName = worksheet.Cells[row, 2]?.Text?.Trim();

                            if (!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(userName))
                            {
                                var group = db.CourseClasses.FirstOrDefault(c => c.Name == groupName);
                                if (group == null) { continue; }

                                var student = db.AspNetUsers.FirstOrDefault(u => u.UserName == userName);
                                if (student == null) { continue; }

                                if (!db.StudentClasses.Any(sc => sc.ClassId == group.Id && sc.StudentId == student.Id))
                                {
                                    var studentClass = new StudentClass
                                    {
                                        ClassId = group.Id,
                                        StudentId = student.Id,
                                        EnrollmentDate = DateTime.UtcNow
                                    };

                                    db.StudentClasses.Add(studentClass);

                                    listUsers.Add(new AssignViewModel
                                    {
                                        StudentId = student.Id,
                                        ClassId = group.Id,
                                        StudentName = student.FullName,
                                        ClassName = group.Code,
                                        GroupName = group.Name,
                                        EnrollmentDate = studentClass.EnrollmentDate
                                    });
                                }
                            }
                        }

                        db.SaveChanges();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "File is invalid or not uploaded.";
                }
            }
            catch (Exception ex)
            {
                
            }

            return View(listUsers);
        }

        public ActionResult Delete(int Id, int courseClassId)
        {
            try
            {
                var model = db.StudentClasses.Find(Id);
                db.StudentClasses.Remove(model);
                db.SaveChanges();
                SetAlert("Xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }
            return RedirectToAction("Assign", new { courseClassId = courseClassId });
        }
        private IEnumerable<SelectListItem> GetStudentsSelectList()
        {
            return db.AspNetUsers.Where(u => u.IsStudent == true).Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            });
        }
    }
}