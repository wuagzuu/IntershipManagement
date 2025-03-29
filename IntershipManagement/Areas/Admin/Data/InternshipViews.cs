using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class InternshipViews : Internship
    {
        public string TeacherCode { get; set; }
        public string MajorCode { get; set; }
        public List<Enterprise> ListEnterprise { get; set; }
    }
}