using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class TeacherReportsController : BaseController
    {
        // GET: Admin/TeacherReports
        public ActionResult Index()
        {
            var teacherId = User.Identity.Name;
            var reports = from a in db.Reports
                          where a.TeacherId == teacherId
                          select new ReportViews()
                          {
                              Id = a.Id,
                              TemplateId = a.TemplateId,
                              StudentId = a.StudentId,
                              SubmissionDate = a.SubmissionDate,
                              Description = a.Description,
                              Status = a.Status,
                              TeacherComments = a.TeacherComments,
                              Grade = a.Grade,
                              SubmissionRound = a.SubmissionRound,
                              FilePath = a.FilePath,
                              TeacherId = a.TeacherId,
                              IsCompleted = a.IsCompleted,
                              StudentCode = db.AspNetUsers.FirstOrDefault(c => c.Id == a.StudentId).Code,
                              Title = db.ReportTemplates.FirstOrDefault(c=>c.Id == a.TemplateId).Title,
                          };
            return View(reports);
        }
        public ActionResult Evaluate(int id)
        {
            var report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Areas/Admin/Views/TeacherReports/Evaluate.cshtml", report);
        }
        [HttpPost]
        public ActionResult Evaluate(Report model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var report = db.Reports.Find(model.Id);
                    if (report != null)
                    {
                        report.TeacherComments = model.TeacherComments;
                        report.Grade = model.Grade;
                        report.IsCompleted = model.IsCompleted;
                        db.SaveChanges();
                    }
                    SetAlert("Đã lưu thông tin thành công.", "success");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
    }
}