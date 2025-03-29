using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class MajorDetailViewModel
    {
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string EnterpriseName { get; set; }
        public string TeacherName { get; set; }
        public int Status { get; set; }
    }
}