using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class MajorController : BaseController
    {
        // GET: Admin/Major
        public ActionResult Index()
        {
            var model = from a in db.Majors
                        select a;
            return View(model.OrderByDescending(c => c.Createdate));
        }
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Major/Create.cshtml", null);
        }
        [HttpPost]
        public ActionResult CreateConfirm(Major model)
        {
            if (ModelState.IsValid)
            {
                model.Createdate = DateTime.Now;
                model.Createby = User.Identity.Name;
                db.Majors.Add(model);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(long Id)
        {
            var model = db.Majors.Find(Id);
            return PartialView("~/Areas/Admin/Views/Major/Edit.cshtml", model);
        }
        [HttpPost]
        public ActionResult EditConfirm(Major model)
        {
            if (ModelState.IsValid)
            {
                model.Editdate = DateTime.Now;
                model.Editby = User.Identity.Name;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");
        }
    }
}