using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class ProfileController : BaseController
    {
        // GET: Admin/Profile
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = db.AspNetUsers.Find(userId);
            return View(model);
        }
    }
}