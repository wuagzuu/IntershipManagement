using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class ReportViews : Report
    {
        public string StudentCode { get; set; }
        public string Title { get; set; }
    }
}