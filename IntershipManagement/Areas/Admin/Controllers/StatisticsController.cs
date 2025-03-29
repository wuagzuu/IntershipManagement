using IntershipManagement.Areas.Admin.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class StatisticsController : BaseController
    {
        // GET: Admin/Statistics
        public ActionResult Index(DateTime? fromDate, DateTime? toDate)
        {
            var query = db.Internships.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(i => i.Createdate >= fromDate);
            if (toDate.HasValue)
                query = query.Where(i => i.Createdate <= toDate);

            var statistics = new InternshipStatisticsViewModel
            {
                TotalStudents = query.Select(i => i.StudentId).Distinct().Count(),
                TotalEnterprises = query.Select(i => i.EnterpriseId).Distinct().Count(),
                TotalTeachers = query.Select(i => i.TeacherId).Distinct().Count(),

                PendingCount = query.Count(i => i.IsConfirm == 0),
                ConfirmedCount = query.Count(i => i.IsConfirm == 1),
                InProgressCount = query.Count(i => i.Status == 1),
                CompletedCount = query.Count(i => i.Status == 2),

                // Thống kê theo chuyên ngành
                MajorStatistics = (from i in query
                                   join m in db.Majors on i.MajorId equals m.Id
                                   group i by new { m.Id, m.Name, m.Code } into g
                                   select new MajorStatisticsViewModel
                                   {
                                       MajorId = g.Key.Id,
                                       MajorName = g.Key.Name,
                                       MajorCode = g.Key.Code,
                                       StudentCount = g.Select(x => x.StudentId).Distinct().Count(),
                                       CompletionRate = (double)g.Count(x => x.Status == 2) / g.Count() * 100
                                   }).ToList(),

                // Thống kê theo doanh nghiệp
                EnterpriseStatistics = (from i in query
                                        join e in db.Enterprises on i.EnterpriseId equals e.Id
                                        group i by new { e.Id, e.Name, e.Province } into g
                                        select new EnterpriseStatisticsViewModel
                                        {
                                            EnterpriseId = g.Key.Id,
                                            EnterpriseName = g.Key.Name,
                                            Province = g.Key.Province,
                                            StudentCount = g.Select(x => x.StudentId).Distinct().Count()
                                        }).ToList()
            };

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(statistics);
        }
        public ActionResult ExportExcel(DateTime? fromDate, DateTime? toDate)
        {
            var statistics = GetStatistics(fromDate, toDate);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Thống kê thực tập");

                // Header
                worksheet.Cells[1, 1].Value = "BÁO CÁO THỐNG KÊ THỰC TẬP";
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                // Thông tin thời gian
                worksheet.Cells[2, 1].Value = "Từ ngày:";
                worksheet.Cells[2, 2].Value = fromDate?.ToString("dd/MM/yyyy") ?? "Tất cả";
                worksheet.Cells[2, 3].Value = "Đến ngày:";
                worksheet.Cells[2, 4].Value = toDate?.ToString("dd/MM/yyyy") ?? "Tất cả";

                // Thống kê tổng quan
                var row = 4;
                worksheet.Cells[row, 1].Value = "Tổng số sinh viên:";
                worksheet.Cells[row, 2].Value = statistics.TotalStudents;
                row++;
                worksheet.Cells[row, 1].Value = "Tổng số doanh nghiệp:";
                worksheet.Cells[row, 2].Value = statistics.TotalEnterprises;
                row++;
                worksheet.Cells[row, 1].Value = "Tổng số giáo viên:";
                worksheet.Cells[row, 2].Value = statistics.TotalTeachers;

                // Thống kê theo chuyên ngành
                row += 2;
                worksheet.Cells[row, 1].Value = "THỐNG KÊ THEO CHUYÊN NGÀNH";
                worksheet.Cells[row, 1, row, 4].Merge = true;
                worksheet.Cells[row, 1].Style.Font.Bold = true;

                row++;
                worksheet.Cells[row, 1].Value = "Mã ngành";
                worksheet.Cells[row, 2].Value = "Tên ngành";
                worksheet.Cells[row, 3].Value = "Số sinh viên";
                worksheet.Cells[row, 4].Value = "Tỷ lệ hoàn thành";

                foreach (var major in statistics.MajorStatistics)
                {
                    row++;
                    worksheet.Cells[row, 1].Value = major.MajorCode;
                    worksheet.Cells[row, 2].Value = major.MajorName;
                    worksheet.Cells[row, 3].Value = major.StudentCount;
                    worksheet.Cells[row, 4].Value = $"{major.CompletionRate:F2}%";
                }

                // Format
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"ThongKeThucTap_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        private InternshipStatisticsViewModel GetStatistics(DateTime? fromDate, DateTime? toDate)
        {
            var query = db.Internships.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(i => i.Createdate >= fromDate);
            if (toDate.HasValue)
                query = query.Where(i => i.Createdate <= toDate);

            var statistics = new InternshipStatisticsViewModel
            {
                TotalStudents = query.Select(i => i.StudentId).Distinct().Count(),
                TotalEnterprises = query.Select(i => i.EnterpriseId).Distinct().Count(),
                TotalTeachers = query.Select(i => i.TeacherId).Distinct().Count(),

                PendingCount = query.Count(i => i.Status == 0),
                ConfirmedCount = query.Count(i => i.IsConfirm == 1),
                InProgressCount = query.Count(i => i.Status == 1),
                CompletedCount = query.Count(i => i.Status == 2),

                // Thống kê theo chuyên ngành
                MajorStatistics = (from i in query
                                   join m in db.Majors on i.MajorId equals m.Id
                                   group i by new { m.Id, m.Name, m.Code } into g
                                   select new MajorStatisticsViewModel
                                   {
                                       MajorId = g.Key.Id,
                                       MajorName = g.Key.Name,
                                       MajorCode = g.Key.Code,
                                       StudentCount = g.Select(x => x.StudentId).Distinct().Count(),
                                       CompletionRate = (double)g.Count(x => x.Status == 2) / g.Count() * 100
                                   }).ToList(),

                // Thống kê theo doanh nghiệp
                EnterpriseStatistics = (from i in query
                                        join e in db.Enterprises on i.EnterpriseId equals e.Id
                                        group i by new { e.Id, e.Name, e.Province } into g
                                        select new EnterpriseStatisticsViewModel
                                        {
                                            EnterpriseId = g.Key.Id,
                                            EnterpriseName = g.Key.Name,
                                            Province = g.Key.Province,
                                            StudentCount = g.Select(x => x.StudentId).Distinct().Count()
                                        }).ToList()
            };
            return statistics;
        }
        public ActionResult GetMonthlyStats(int year)
        {
            var monthlyStats = db.Internships
                .Where(i => i.Createdate.HasValue && i.Createdate.Value.Year == year)
                .GroupBy(i => new { Month = i.Createdate.Value.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Completed = g.Count(x => x.Status == 2)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Json(monthlyStats, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int majorId)
        {
            var majorStats = (from i in db.Internships
                              where i.MajorId == majorId
                              select new MajorDetailViewModel
                              {
                                  StudentCode = i.StudentCode,
                                  StudentName = i.StudentName,
                                  EnterpriseName = i.EnterpriseName,
                                  TeacherName = i.TeacherName,
                                  Status = i.Status ?? 0
                              }).ToList();

            return PartialView("_MajorDetails", majorStats);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}