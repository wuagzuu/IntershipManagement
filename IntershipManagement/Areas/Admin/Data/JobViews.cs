using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class JobViews:Job
    {
        public string MajorName { get; set; }
    }
}