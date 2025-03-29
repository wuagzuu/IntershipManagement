using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntershipManagement.Models;

namespace IntershipManagement.Areas.Admin.Data
{
    public class UserView : AspNetUser
    {
        public string Role { get; set; }
        public string MajorName { get; set; }
    }
}