using HRMSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSWeb.Controllers
{
    public class BranchController : Controller
    {
        private HRMSEntities db = new HRMSEntities();
        // GET: Branch
        Session_CRM sess;
        ErrorHandling err;
        public BranchController()
        {
            sess = new Session_CRM();
            err = new ErrorHandling();
        }
        public ActionResult Index()
        {
            List<Branch> brnch = new List<Branch>();
            try
            {
                brnch= db.Branches.Where(x => x.IsActive == true).ToList();
             
            }
            catch (Exception ex)
            {

                err.WriteError(ex);
            }
            return View(brnch);
        }
        public ActionResult _New()
        {
            
            return PartialView("_New");
        }
        [HttpPost]
        public ActionResult _New(Branch branch)
        {
            string msg = "";

            sess = (Session_CRM)Session["CRM_Session"];
            var UserID = sess.User.UserID;
            if (branch.BranchID==0|| branch.BranchID==null)
            {
                Branch _branch = new Branch();
                _branch.Name = branch.Name;
                _branch.CreatedBy = UserID;
                _branch.CreateDate = DateTime.Now;
                _branch.IsActive = true;
                db.Branches.Add(_branch);
                db.SaveChanges();
            }
            else
            {

            }

            List<Branch> brnch = new List<Branch>();
            try
            {
                brnch = db.Branches.Where(x => x.IsActive == true).ToList();

            }
            catch (Exception ex)
            {

                err.WriteError(ex);
            }
            return PartialView("_Index", brnch);
        }

    }
}