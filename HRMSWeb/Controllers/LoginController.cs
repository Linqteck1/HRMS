
using HRMSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMSWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        HRMSEntities db = new HRMSEntities();
        public ActionResult Index()
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            sess = null;
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string email, string password)
        {

            bool isTrailExist = false;
            int dayEnd = 0;
            int TotalDaysTrail = 0;
            string encpass = CRM_Common.Encrypt(password);
            AT_Users userlist = db.AT_Users.Where(x => x.Email == email && x.Password == encpass).FirstOrDefault<AT_Users>();


            if (userlist != null)
            {

                if (userlist.IsActive)
                {


                    if (db.AT_Role.Where(x => x.RoleID == userlist.RoleID).Select(x => x.IsActive).FirstOrDefault())
                    {
                        Session_CRM sess = new Session_CRM();

                        List<Permissions> pplst = (from P in db.AT_Pages
                                                   join PP in db.AT_Permission.Include("AT_PermissionActionJunc") on P.PageID equals PP.PageID
                                                   join M in db.AT_Modules on P.ModuleID equals M.ModuleID
                                                   join Per in db.AT_RolePermissionJunc on new { PermissionID = PP.PermissionID, IsGroup = false } equals new { PermissionID = Per.PermissionID, IsGroup = Per.IsGroup }
                                                   where Per.RoleID == userlist.RoleID && P.IsActive == true && PP.IsActive == true
                                                   select (new Permissions
                                                   {
                                                       AT_Pages = P,
                                                       AT_Permission = PP,
                                                       AT_Modules = M,
                                                       AT_RolePermissionJunc = Per,
                                                       AT_PermissionActionJunc = PP.AT_PermissionActionJunc.ToList()

                                                   })).OrderBy(x => x.AT_Pages.PageOrder).ToList();
                        List<Permissions> pplst2 = (from P in db.AT_Pages
                                                    join PP in db.AT_Permission.Include("AT_PermissionActionJunc") on P.PageID equals PP.PageID
                                                    join M in db.AT_Modules on P.ModuleID equals M.ModuleID
                                                    join Pj in db.AT_PermissionGroupJunc on PP.PermissionID equals Pj.PermissionID
                                                    join Per in db.AT_RolePermissionJunc on new { PermissionID = Pj.PermissionGroupID, IsGroup = true } equals new { PermissionID = Per.PermissionID, IsGroup = Per.IsGroup }
                                                    where Per.RoleID == userlist.RoleID && P.IsActive == true && PP.IsActive == true
                                                    select (new Permissions
                                                    {
                                                        AT_Pages = P,
                                                        AT_Permission = PP,
                                                        AT_Modules = M,
                                                        AT_RolePermissionJunc = Per,
                                                        AT_PermissionActionJunc = PP.AT_PermissionActionJunc.ToList()

                                                    })).OrderBy(x => x.AT_Pages.PageOrder).ToList();

                        List<Permissions> pplst3 = (from P in db.AT_Pages
                                                    join PP in db.AT_Permission.Include("AT_PermissionActionJunc") on P.PageID equals PP.PageID
                                                    join M in db.AT_Modules on P.ModuleID equals M.ModuleID
                                                    join Pj in db.AT_PermissionGroupJunc on PP.PermissionID equals Pj.PermissionID
                                                    join Per in db.AT_UserExtraPermissionjunc on new { PermissionGroupID = Pj.PermissionGroupID, IsExtrapermission = true } equals new { PermissionGroupID = Per.PermissionGroupID, IsExtrapermission = Per.AT_PermissionGroup.IsExtrapermission }
                                                    where Per.UserID == userlist.UserID && P.IsActive == true && PP.IsActive == true
                                                    select (new Permissions
                                                    {
                                                        AT_Pages = P,
                                                        AT_Permission = PP,
                                                        AT_Modules = M,
                                                        AT_UserExtraPermissionjunc = Per,
                                                        AT_PermissionActionJunc = PP.AT_PermissionActionJunc.ToList()

                                                    })).OrderBy(x => x.AT_Pages.PageOrder).ToList();

                        List<Permissions> finallst = pplst.Union(pplst2).Union(pplst3).ToList<Permissions>();

                        if (finallst.Count() > 0)
                        {
                            sess.AllPermissions = finallst;
                            var result = finallst.Select(z => z.AT_Modules).GroupBy(x => new { x.ModuleID }).Select(z => new AT_Modules
                            {
                                IsActive = z.FirstOrDefault().IsActive,
                                ModuleIcon = z.FirstOrDefault().ModuleIcon,
                                ModuleID = z.Key.ModuleID,
                                ModuleName = z.FirstOrDefault().ModuleName,
                                ModuleOrder = z.FirstOrDefault().ModuleOrder,
                                ParentID = z.FirstOrDefault().ParentID,
                                AT_Pages = z.FirstOrDefault().AT_Pages.ToList()
                            }).ToList();
                            sess.AT_Modules = result;
                            sess.User = userlist;
                            sess.User.CRM_URL = Request.Url.AbsoluteUri;
                            Session.Add("CRM_Session", sess);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.msg = "You have not rights for login!";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.msg = "User role inactive!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.msg = "User inactive!";
                    return View();
                }
            }
            else
            {
                ViewBag.msg = "Login Failed!";
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}