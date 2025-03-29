using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class StudentClassIndexViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
    }
}