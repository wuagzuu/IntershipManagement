using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class ReportTemplateViewModel
    {
        public ReportTemplate Template { get; set; }
        public Report Report { get; set; }
    }
}