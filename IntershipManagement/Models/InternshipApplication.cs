//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IntershipManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InternshipApplication
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CourseClassId { get; set; }
        public System.DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string TeacherId { get; set; }
        public Nullable<int> MajorId { get; set; }
    
        public virtual CourseClass CourseClass { get; set; }
    }
}
