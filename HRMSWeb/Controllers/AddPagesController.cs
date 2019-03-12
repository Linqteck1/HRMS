using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMSWeb.Models;

namespace HRMSWeb.Controllers
{
   [SessionAuthorizeAttribute]
    public class AddPagesController : Controller
    {

        HRMSEntities db = new HRMSEntities();
        public ActionResult Index()
        {
            return View(db.AT_Pages.ToList());
        }
        [HttpGet]
        public ActionResult Save(int id)
        {
            ViewBag.ModuleList = db.AT_Modules.Where(x => x.IsActive == true && x.ParentID == 0).ToList();
            return PartialView("Save", db.AT_Pages.Where(x => x.PageID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Save(AT_Pages peg)
        {
            string msg = "";
            try
            {
                Session_CRM sess = (Session_CRM)Session["CRM_Session"];
                if (peg.PageID > 0)
                {
                    db.AT_Pages.Attach(peg);
                    db.UpdateExcept<AT_Pages>(peg);
                }
                else
                {
                    db.Entry(peg).State = System.Data.Entity.EntityState.Added;
                }
                db.SaveChanges();
                msg = "Page saved successfully!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ViewBag.msg = msg;
            db = new HRMSEntities();
            return PartialView("_Index", db.AT_Pages.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}