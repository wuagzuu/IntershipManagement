using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Areas.Admin.Data
{
    public class InternshipApplicationViewModel:InternshipApplication
    {
        public string TeacherName { get; set; }
        public int StudentCount { get; set; }

        public IEnumerable<ClassDto> CourseClasses { get; set; }
        public IEnumerable<TeacherDto> Teachers { get; set; }
    }
    public class TeacherDto
    {
        public string Id { get; set; }
        public string FullNameWithMajor { get; set; }
    }
    public class ClassDto
    {
        public int Id { get; set; }
        public string NameCode { get; set; }
    }
}