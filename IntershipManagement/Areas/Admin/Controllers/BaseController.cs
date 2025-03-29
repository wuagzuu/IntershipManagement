using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin - Quản trị toàn hệ thống, Giáo viên, Sinh viên")]
    public class BaseController : Controller
    {
        //khởi tạo connect db
        public IntershipManagementEntities db = new IntershipManagementEntities();

        //check xem sinh viên đã được phân công thực tập chưa
        public bool CheckInternship(string userId)
        {
            // Kiểm tra xem sinh viên đã tồn tại trong bảng Internship và đã được duyệt chưa
            var isConfirmed = db.Internships
                                .Any(i => i.StudentId == userId && i.IsConfirm == 1);

            if (isConfirmed)
            {
                // Nếu sinh viên đã được duyệt, trả về false (không thể tạo thêm phiếu thực tập)
                return false;
            }
            // Lấy danh sách tất cả các ClassId trong InternshipApplications
            var classIds = db.InternshipApplications.Select(a => a.CourseClassId).Distinct();

            // Kiểm tra xem sinh viên có mặt trong các lớp liên quan hay không
            var isAssigned = db.StudentClasses.Any(sc => classIds.Contains(sc.ClassId) && sc.StudentId == userId);

            return isAssigned;
        }
        //check xem giáo viên đã được phân công thực tập chưa
        public bool CheckTeacherInternship(string teacherId)
        {
            // Kiểm tra xem có InternshipApplication nào liên quan đến giáo viên không
            var hasAssignments = db.InternshipApplications.Any(a => a.TeacherId == teacherId);

            return hasAssignments;
        }

        //theme alert
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;

            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
                TempData["Alert"] = "Success!";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
                TempData["Alert"] = "Warning!";
            }
            else if (type == "danger")
            {
                TempData["AlertType"] = "alert-danger";
                TempData["Alert"] = "Danger!";
            }
            else if (type == "info")
            {
                TempData["AlertType"] = "alert-info";
                TempData["Alert"] = "Info!";
            }
            else if (type == "primary")
            {
                TempData["AlertType"] = "alert-primary";
                TempData["Alert"] = "Primary!";
            }
            else
            {
                TempData["AlertType"] = "alert-default";
                TempData["Alert"] = "Nothing!";
            }

        }
    }
}