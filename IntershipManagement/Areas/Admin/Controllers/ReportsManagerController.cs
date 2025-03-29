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
    public class ReportsManagerController : BaseController
    {
        // GET: Admin/ReportsManager
        public ActionResult Index()
        {
            var teacherId = User.Identity.GetUserId();
            var templates = db.ReportTemplates.Where(t => t.TeacherId == teacherId).ToList();
            return View(templates);
        }
        public ActionResult Create()
        {
            // Giả sử bạn có thể lấy TeacherId từ thông tin người dùng đăng nhập
            string teacherId = User.Identity.GetUserId(); // Hoặc cách khác để lấy TeacherId
            var isInternship = CheckTeacherInternship(teacherId);
            if (!isInternship)
            {
                return Json(new { success = false, message = "Bạn chưa được phân công thực tập. Hãy kiểm tra lại." }, JsonRequestBehavior.AllowGet);
            }
            // Danh sách tất cả các đợt từ 1 đến 12
            var allRounds = Enumerable.Range(1, 12).ToList();

            // Lấy danh sách các đợt đã được tạo bởi giáo viên này từ cơ sở dữ liệu
            var createdRounds = db.ReportTemplates
                                  .Where(rt => rt.TeacherId == teacherId)
                                  .Select(rt => rt.Round)
                                  .Distinct()
                                  .ToList();
            var createdRoundsInt = createdRounds.Where(r => r.HasValue).Select(r => r.Value).ToList();

            // Lọc ra các đợt chưa được tạo
            var availableRounds = allRounds.Except(createdRoundsInt).ToList();
            ViewBag.Rounds = new SelectList(availableRounds);

            return PartialView("~/Areas/Admin/Views/ReportsManager/Create.cshtml", null);
        }
        [HttpPost]
        public ActionResult CreateConfirm(ReportTemplate model, HttpPostedFileBase Attachment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {
                        // Đảm bảo thư mục lưu trữ tồn tại
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
                    model.CreatedDate = DateTime.Now;
                    model.TeacherId = User.Identity.GetUserId();
                    db.ReportTemplates.Add(model);
                    db.SaveChanges();
                    SetAlert("Đã lưu thông tin thành công.", "success");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }

            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return View(model);
        }

    }
}