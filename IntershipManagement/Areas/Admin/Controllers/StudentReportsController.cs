using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class StudentReportsController : BaseController
    {
        // GET: Admin/StudentReports
        public ActionResult Index()
        {
            string studentId = User.Identity.GetUserId();
            var student = db.AspNetUsers.FirstOrDefault(c => c.Id == studentId);

            if (student == null)
            {
                return HttpNotFound("Student not found.");
            }

            var teacher = db.AspNetUsers.FirstOrDefault(c => c.UserName == student.Createby);

            if (teacher == null)
            {
                return HttpNotFound("Teacher not found.");
            }

            // Lấy danh sách các TemplateId mà giáo viên đã tạo
            var templateIds = db.ReportTemplates
                .Where(t => t.TeacherId == teacher.Id)
                .Select(t => t.Id)
                .Cast<int>()
                .ToList();

            // Lọc các báo cáo dựa trên danh sách TemplateId
            var reports = db.Reports
                .Where(r => r.StudentId == studentId && r.TemplateId.HasValue && templateIds.Contains(r.TemplateId.Value))
                .ToList();

            var reportTemplates = db.ReportTemplates
                .Where(t => templateIds.Contains(t.Id))
                .ToList();

            var viewModel = reportTemplates.Select(template => new ReportTemplateViewModel
            {
                Template = template,
                Report = reports.FirstOrDefault(r => r.TemplateId == template.Id)
            }).ToList();

            return View(viewModel);
        }
        public ActionResult Download(int id)
        {
            var template = db.ReportTemplates.Find(id);
            if (template == null)
            {
                return HttpNotFound();
            }

            // Chuyển đổi đường dẫn tương đối sang đường dẫn tuyệt đối
            var filePath = Server.MapPath(template.FilePath);

            // Kiểm tra xem file có tồn tại không
            if (!System.IO.File.Exists(filePath))
            {
                SetAlert("File không tồn tại.", "danger");
                return HttpNotFound("File không tồn tại.");
            }

            // Đọc file thành mảng byte
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Trả về file với kiểu nội dung Excel
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
        }
        public ActionResult Submit(int templateId)
        {
            var model = new Report
            {
                TemplateId = templateId
            };
            return PartialView("~/Areas/Admin/Views/StudentReports/Submit.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SubmitConfirm(Report model, HttpPostedFileBase Attachment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        var uploadDir = "~/File";
                        var uploadPath = Server.MapPath(uploadDir);
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Sử dụng Guid để tạo chuỗi ký tự ngẫu nhiên
                        var uniqueSuffix = Guid.NewGuid().ToString();
                        var fileName = Path.GetFileNameWithoutExtension(Attachment.FileName);
                        var fileExtension = Path.GetExtension(Attachment.FileName);
                        var uniqueFileName = $"{fileName}_{uniqueSuffix}{fileExtension}";

                        var filePath = Path.Combine(uploadPath, uniqueFileName);
                        Attachment.SaveAs(filePath);

                        model.FilePath = Path.Combine(uploadDir, uniqueFileName); // Lưu đường dẫn tương đối
                    }
                    var temp = db.ReportTemplates.FirstOrDefault(c => c.Id == model.TemplateId);

                    model.StudentId = User.Identity.GetUserId();
                    model.TeacherId = db.AspNetUsers.FirstOrDefault(c => c.Id == model.StudentId).Createby;
                    model.SubmissionDate = DateTime.Now;
                    model.SubmissionRound = temp.Round;
                    // Check trạng thái xem nộp đúng hạn k
                    if (model.SubmissionDate > temp.Deadline)
                    {
                        model.Status = "Quá hạn";
                    }
                    else
                    {
                        model.Status = "Đúng hạn";
                    }
                    SetAlert("Đã nộp báo cáo thành công.", "success");
                    db.Reports.Add(model);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }

            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return View(model);
        }
        public ActionResult ViewResults(int round)
        {
            string studentId = User.Identity.GetUserId();

            // Truy vấn để lấy báo cáo
            var report = db.Reports
                           .FirstOrDefault(r => r.StudentId == studentId && r.SubmissionRound == round);

            if (report == null)
            {
                return HttpNotFound("Báo cáo không tồn tại.");
            }

            // Truy vấn để lấy template dựa trên TemplateId
            var reportTemplate = db.ReportTemplates
                                   .FirstOrDefault(t => t.Id == report.TemplateId);

            // Tạo một view model hoặc truyền dữ liệu cần thiết vào view
            var viewModel = new ReportTemplateViewModel
            {
                Report = report,
                Template = reportTemplate
            };

            return PartialView("~/Areas/Admin/Views/StudentReports/ViewResults.cshtml", viewModel);
        }
    }
}