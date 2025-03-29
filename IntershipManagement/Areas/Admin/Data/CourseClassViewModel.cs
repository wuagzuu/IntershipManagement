using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Data
{
    public class CourseClassViewModel: CourseClass
    {
        public string TeacherName { get; set; }
        public string CourseGroupName { get; set; }
        public int StudentCount { get; set; }
        public IEnumerable<SelectListItem> Teachers { get; set; }
    }
}