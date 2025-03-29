using IntershipManagement.Areas.Admin.Data;
using IntershipManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntershipManagement.Areas.Admin.Controllers
{
    public class ConfigJobController : BaseController
    {
        // GET: Admin/ConfigJob
        public ActionResult Index(int enterpriseId)
        {
            var model = from a in db.Jobs
                        where a.EnterpriseId == enterpriseId
                        join b in db.Majors on a.MajorId equals b.Id
                        select new JobViews()
                        {
                            Id = a.Id,
                            EnterpriseId = a.EnterpriseId,
                            MajorId = a.MajorId,
                            Name = a.Name,
                            Slot = a.Slot,
                            Createby = a.Createby,
                            Createdate = a.Createdate,
                            Editby = a.Editby,
                            Editdate = a.Editdate,
                            MajorName = b.Name
                        };
            ViewBag.Enterprise = db.Enterprises.FirstOrDefault(c => c.Id == enterpriseId);
            return View(model.OrderByDescending(c => c.Createdate));
        }
        public ActionResult Create(int enterpriseId)
        {
            ViewBag.ListMajor = db.Majors.ToList();
            var model = new Job();
            model.EnterpriseId = enterpriseId;
            return PartialView("~/Areas/Admin/Views/ConfigJob/Create.cshtml", model);
        }
        [HttpPost]
        public ActionResult CreateConfirm(Job model)
        {
            if (ModelState.IsValid)
            {
                model.Createdate = DateTime.Now;
                model.Createby = User.Identity.Name;
                db.Jobs.Add(model);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index", new { enterpriseId = model.EnterpriseId });
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index", new { enterpriseId = model.EnterpriseId });
        }
        public ActionResult Edit(int Id)
        {
            ViewBag.ListMajor = db.Majors.ToList();
            var model = db.Jobs.Find(Id);
            return PartialView("~/Areas/Admin/Views/ConfigJob/Edit.cshtml", model);
        }
        [HttpPost]
        public ActionResult EditConfirm(Job model)
        {
            if (ModelState.IsValid)
            {
                model.Editdate = DateTime.Now;
                model.Editby = User.Identity.Name;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index", new { enterpriseId = model.EnterpriseId });
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index", new { enterpriseId = model.EnterpriseId });
        }
    }
}