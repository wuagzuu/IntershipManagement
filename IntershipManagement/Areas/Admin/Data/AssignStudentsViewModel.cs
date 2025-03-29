using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Data
{
    public class AssignStudentsViewModel
    {
        public int CourseClassId { get; set; }
        public int CourseGroupId { get; set; }
        public IEnumerable<string> SelectedStudentIds { get; set; }
        public IEnumerable<SelectListItem> Students { get; set; }
        public IEnumerable<AssignViewModel> ListStudents { get; set; }
    }
    public class AssignViewModel: StudentClass
    {
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string GroupName { get; set; }

    }
}