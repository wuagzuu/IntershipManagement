using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class InternshipStatisticsViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalEnterprises { get; set; }
        public int TotalTeachers { get; set; }

        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }

        public List<MajorStatisticsViewModel> MajorStatistics { get; set; }
        public List<EnterpriseStatisticsViewModel> EnterpriseStatistics { get; set; }
    }

    public class MajorStatisticsViewModel
    {
        public int MajorId { get; set; }
        public string MajorName { get; set; }
        public string MajorCode { get; set; }
        public int StudentCount { get; set; }
        public double CompletionRate { get; set; }
    }

    public class EnterpriseStatisticsViewModel
    {
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string Province { get; set; }
        public int StudentCount { get; set; }
    }

}