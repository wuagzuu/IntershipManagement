using IntershipManagement.Areas.Admin.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(c => c.Id == userId);
            if (user != null)
            {
                Session["RoleId"] = user.AspNetRoles.First().Name;
                Session["FullNameUser"] = user.FullName;
                Session["EmailUser"] = user.Email;
                Session["AddressUser"] = user.Address + " " + user.Ward + " " + user.District + " " + user.Province;
            }
            return RedirectToAction("Index", "Profile");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public PartialViewResult _LoadNotification()
        {
            var query = db.Notifications.OrderByDescending(a => a.Createdate).Where(a => a.SentTo == User.Identity.Name && a.IsRead == null);

            return PartialView(query);
        }
        [HttpPost]
        public JsonResult ReadNotif(long Id)
        {
            var model = db.Notifications.Find(Id);
            model.IsRead = true;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult loadDataNotif()
        {
            var query = db.Notifications.OrderByDescending(a => a.Createdate).Where(a => a.SentTo == User.Identity.Name).Take(10);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult loadModalNotif()
        {
            var query = db.Notifications.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.SentTo == User.Identity.Name);
            if (query.IsRead == null)
            {
                return PartialView("~/Areas/Admin/Views/Home/_loadModalNotif.cshtml", query);
            }
            else
            {
                return PartialView("~/Areas/Admin/Views/Home/_loadModalNotif.cshtml", null);
            }
        }
    }
}