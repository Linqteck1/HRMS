using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Threading;
using System.Data.Entity;

using HRMSWeb.Models;

namespace InstituteMS.Controllers
{
   // [SessionAuthorizeAttribute]
    public class CRM_SettingController : Controller
    {
         HRMSEntities db = new HRMSEntities();
        public ActionResult Index()
        {
            //Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            //var Result = db.AT_GenSetting.Where(x => x.CRMID == sess.User.ClientID).FirstOrDefault();
            return View();
        }
        #region // Agent Area
        public ActionResult AgentIndex()
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            return PartialView("AgentIndex", db.AT_Users.Where(a => a.IsDeleted != true).ToList());
        }
        public ActionResult AgentDelete(int id)
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            string msg = "";
            AT_Users usr = db.AT_Users.Where(x => x.UserID == id).FirstOrDefault();
            try
            {
                if (usr != null)
                {
                    usr.IsDeleted = true;
                    usr.UpdateBy = sess.User.UserID;
                    usr.UpdateDate = DateTime.Now;
                    db.AT_Users.Attach(usr);
                    db.UpdateOnly<AT_Users>(usr, x => x.IsDeleted, x => x.UpdateBy, x => x.UpdateDate);
                  
                    db.SaveChanges();
                    msg = "User Successfully Deleted.";
                }
            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }
            db = new  HRMSEntities();
            ViewBag.msg = msg;
            return PartialView("_AgentIndex", db.AT_Users.Where(a =>  a.IsDeleted != true).ToList());
        }
        [HttpGet]
        public ActionResult _AgentSave(int id)
        {
            //ViewBag.TypeList = db.vt_UserType.ToList();
            ViewBag.RoleList = db.AT_Role.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
            var Result = db.AT_Users.Where(x => x.UserID == id).FirstOrDefault();
            return PartialView("_AgentSave", Result);
        }

        [HttpPost]
        public ActionResult _AgentSave(AT_Users usr)
        {
            string msg = "";
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            try
            {


              //  var UserType = db.AT_UserType.Where(x => x.UserType == "Agent").Select(x => x.TypeID).FirstOrDefault();
                //usr.TypeID = UserType;
               // usr.ClientID = sess.User.ClientID;
              //  AT_Agent det = usr.Agent;
                usr.Password = CRM_Common.Encrypt(usr.Password);
                if (usr.UserID > 0)
                {
                    usr.UpdateBy = sess.User.UserID;
                    usr.UpdateDate = DateTime.Now;

                    db.AT_Users.Attach(usr);
                    db.UpdateExcept<AT_Users>(usr, x => x.CreateBy, x => x.CreateDate);
                    db.SaveChanges();
                }
                else
                {

                    usr.CreateBy = sess.User.UserID;
                    usr.CreateDate = DateTime.Now;
                    db.Entry(usr).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                }
                msg = "User saved successfully!";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.ToString().Contains("UNIQUE"))
                {
                    msg = "Conflict";
                }
                else
                {
                    msg = ex.Message;
                }
            }
            ViewBag.msg = msg;
            db = new HRMSEntities();
            ViewBag.msg = msg;
            return PartialView("_AgentIndex", db.AT_Users.Where(a =>  a.IsDeleted != true).ToList());
        }

        [HttpGet]
        public ActionResult GetPermission(int id)
        {

            List<ExtraPermissions> lst = (from pg in db.AT_PermissionGroup
                                          join uep in db.AT_UserExtraPermissionjunc on new { PermissionGroupID = pg.PermissionGroupID, UserID = id } equals new { PermissionGroupID = uep.PermissionGroupID, UserID = uep.UserID } into pa
                                          from j in pa.DefaultIfEmpty()
                                          where (j.UserID == id || j.UserID == null) && pg.IsActive == true && pg.IsExtrapermission == true
                                          select (new ExtraPermissions
                                          {
                                              PermissionGroupID = pg == null ? 0 : pg.PermissionGroupID,
                                              Name = pg == null ? "" : pg.Name,
                                              UserExtraPermissionjuncID = j == null ? 0 : j.UserExtraPermissionjuncID,
                                              Allow = j == null ? false : true,
                                              UserID = id
                                          })).ToList();


            return PartialView("_ExtraPermission", lst);
        }
        [HttpPost]
        public ActionResult SaveExtraPermission(string lst)
        {
            string msg = "";
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            List<ExtraPermissions> lstEP = JsonConvert.DeserializeObject<List<ExtraPermissions>>(lst);
            foreach (var item in lstEP)
            {
                if (item.Allow)
                {
                    var isexist = db.AT_UserExtraPermissionjunc.Where(x => x.UserID == item.UserID && x.PermissionGroupID == item.PermissionGroupID).FirstOrDefault();
                    if (isexist == null)
                    {
                        AT_UserExtraPermissionjunc jnc = new AT_UserExtraPermissionjunc();
                        jnc.PermissionGroupID = item.PermissionGroupID;
                        jnc.UserID = item.UserID;
                       // jnc.ClientID = sess.User.ClientID;
                        db.Entry(jnc).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();
                    }

                }
                else
                {
                    var isexist = db.AT_UserExtraPermissionjunc.Where(x => x.UserID == item.UserID && x.PermissionGroupID == item.PermissionGroupID).FirstOrDefault();
                    if (isexist != null)
                    {
                        db.AT_UserExtraPermissionjunc.Where(x => x.PermissionGroupID == item.PermissionGroupID && x.UserID == item.UserID).ToList().ForEach(tol => db.AT_UserExtraPermissionjunc.Remove(tol));
                        db.SaveChanges();
                    }
                }
            }


            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // Role Area
        public ActionResult RoleIndex()
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            return PartialView("RoleIndex", db.AT_Role.Where(x => x.IsDeleted != true).ToList());
        }
        [HttpPost]
        public ActionResult RoleDelete(int id)
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            string msg = "";
            try
            {
                AT_Role role = db.AT_Role.Where(x => x.RoleID == id).FirstOrDefault();
                if (role != null)
                {
                    if (db.AT_Users.Where(x => x.RoleID == role.RoleID && x.IsDeleted != true).Count() > 0)
                    {
                        return Json("Conflict");
                    }
                    //role.RoleID = id;
                    //role.IsDeleted = true;
                    //role.UpdateBy = sess.User.UserID;
                    //role.UpdateDate = DateTime.Now;
                    //db.AT_Role.Attach(role);
                    db.AT_RolePermissionJunc.RemoveRange(db.AT_RolePermissionJunc.Where(x => x.RoleID == role.RoleID));
                    db.AT_Role.Remove(role);
                    db.SaveChanges();
                    msg = "Role deleted successfully!";
                }
            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }

            ViewBag.msg = msg;
            return PartialView("_RoleIndex", db.AT_Role.Where(x => x.IsDeleted != true).ToList());

        }
        [HttpGet]
        public ActionResult _RoleSave(int id)
        {
            return PartialView("_RoleSave", db.AT_Role.Where(x => x.RoleID == id && x.IsDeleted == false).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult _RoleSave(AT_Role role)
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            string msg = "";
            try
            {
                if (role.RoleID > 0)
                {
                    if (role.IsActive != true)
                    {
                        if (db.AT_Users.Where(x => x.RoleID == role.RoleID && x.IsDeleted != true).Count() > 0)
                        {
                            return Json("Conflict");
                        }
                    }
                    role.UpdateBy = sess.User.UserID;
                    role.UpdateDate = DateTime.Now;
                    db.AT_Role.Attach(role);
                    db.UpdateExcept<AT_Role>(role, x => x.CreateBy, x => x.CreateDate);
                }
                else
                {
                    role.CreateBy = sess.User.UserID;
                    role.CreateDate = DateTime.Now;
                    db.Entry(role).State = System.Data.Entity.EntityState.Added;
                }
                db.SaveChanges();
                msg = "Role saved successfully!";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.ToString().Contains("UNIQUE"))
                {
                    msg = "Conflict";
                }
                else
                {
                    msg = ex.Message;
                }
            }
            ViewBag.msg = msg;
            return PartialView("_RoleIndex", db.AT_Role.Where(x => x.IsDeleted != true).ToList());
        }
        #endregion

        #region //Permission Area
        public ActionResult PermissionIndex(int id)
        {

            ViewBag.PermissionGroup = (from P in db.AT_PermissionGroup
                                       join Pgj in db.AT_RolePermissionJunc on new { PermissionID = P.PermissionGroupID, RoleID = id, IsGroup = true } equals new { PermissionID = Pgj.PermissionID, RoleID = Pgj.RoleID, IsGroup = Pgj.IsGroup } into pa
                                       from j in pa.DefaultIfEmpty()
                                       where (j.RoleID == id || j.RoleID == null) && P.IsActive == true && P.IsExtrapermission == false
                                       select new
                                       {
                                           P.PermissionGroupID,
                                           RolePermissionJuncID = j == null ? 0 : j.RolePermissionJuncID,
                                           P.Name,
                                           Allow = j == null ? false : true

                                       }).ToList().ListToDataTable();

            ViewBag.Permission = (from P in db.AT_Permission
                                  join Pgj in db.AT_RolePermissionJunc on new { PermissionID = P.PermissionID, RoleID = id, IsGroup = false } equals new { PermissionID = Pgj.PermissionID, RoleID = Pgj.RoleID, IsGroup = Pgj.IsGroup } into pa
                                  from j in pa.DefaultIfEmpty()
                                  where (j.RoleID == id || j.RoleID == null) && P.IsActive == true
                                  select new
                                  {
                                      P.PermissionID,
                                      RolePermissionJuncID = j == null ? 0 : j.RolePermissionJuncID,
                                      P.Permission,
                                      Allow = j == null ? false : true

                                  }).ToList().ListToDataTable();
            ViewBag.RoleID = id;
            return PartialView("PermissionIndex");
        }
        [HttpPost]
        public JsonResult GetGroupPermission(string strlstPP)
        {

            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            DataTable lstPP = JsonConvert.DeserializeObject<DataTable>(strlstPP);
            List<int> gid = new List<int>();
            foreach (DataRow item in lstPP.Rows)
            {
                if (Convert.ToBoolean(item["Allow"]))
                    gid.Add(Convert.ToInt32(item["PermissionGroupID"]));
            }
            var lst = db.AT_PermissionGroupJunc.Where(x => gid.Contains(x.PermissionGroupID) && x.AT_Permission.IsActive == true).Select(x => x.AT_Permission.Permission).ToList();


            return Json(new { lst = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PermissionSave(string strlstPermission, string strlstGroup, int RoleID)
        {
            string msg = "";
            try
            {
                Session_CRM sess = (Session_CRM)Session["CRM_Session"];
                DataTable lstPermission = JsonConvert.DeserializeObject<DataTable>(strlstPermission);
                DataTable lstGroup = JsonConvert.DeserializeObject<DataTable>(strlstGroup);


                db.AT_RolePermissionJunc.Where(x => x.RoleID == RoleID).ToList().ForEach(tol => db.AT_RolePermissionJunc.Remove(tol));
                db.SaveChanges();


                foreach (DataRow Permission in lstPermission.Rows)
                {
                    if (Convert.ToBoolean(Permission["Allow"]))
                    {
                        AT_RolePermissionJunc RP = new AT_RolePermissionJunc();
                        RP.ClientID = 1;
                        RP.PermissionID = Convert.ToInt32(Permission["PermissionID"]);
                        RP.RoleID = RoleID;
                        RP.IsGroup = false;
                        db.AT_RolePermissionJunc.Add(RP);
                    }

                }

                foreach (DataRow Group in lstGroup.Rows)
                {
                    if (Convert.ToBoolean(Group["Allow"]))
                    {
                        AT_RolePermissionJunc RP = new AT_RolePermissionJunc();
                        RP.ClientID = 1;
                        RP.PermissionID = Convert.ToInt32(Group["PermissionGroupID"]);
                        RP.RoleID = RoleID;
                        RP.IsGroup = true;
                        db.AT_RolePermissionJunc.Add(RP);
                    }
                }
                db.SaveChanges();
                msg = "Permission saved successfully!";

            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }
            ViewBag.msg = msg;
            return PartialView("RoleIndex", db.AT_Role.Where(x => x.IsDeleted == false).ToList());
        }
        #endregion

        #region //Permission Group Area
        public ActionResult PermissionGroupIndex()
        {
            return PartialView("PermissionGroupIndex", db.AT_PermissionGroup.ToList());
        }

        [HttpGet]
        public ActionResult _PermissionGroupSave(int id)
        {
            ViewBag.Permission = (from P in db.AT_Permission
                                  join Pgj in db.AT_PermissionGroupJunc on new { PermissionID = P.PermissionID, PermissionGroupID = id } equals new { PermissionID = Pgj.PermissionID, PermissionGroupID = Pgj.PermissionGroupID } into pa
                                  from j in pa.DefaultIfEmpty()
                                  where (j.PermissionGroupID == id || j.PermissionGroupID == null) && P.IsActive == true
                                  select new
                                  {
                                      P.PermissionID,
                                      PermissionGroupID = j == null ? 0 : j.PermissionGroupID,
                                      P.Permission,
                                      Allow = j == null ? false : true

                                  }).ToList().ListToDataTable();
            return PartialView("_PermissionGroupSave", db.AT_PermissionGroup.Where(x => x.PermissionGroupID == id).FirstOrDefault());
        }


        [HttpPost]
        public ActionResult _PermissionGroupSave(AT_PermissionGroup pg)
        {
            string msg = "";
            try
            {

                Session_CRM sess = (Session_CRM)Session["CRM_Session"];
                DataTable lstP = JsonConvert.DeserializeObject<DataTable>(pg.json_Permissions);
                if (pg.PermissionGroupID > 0)
                {
                   // pg.ClientID = sess.User.ClientID;
                    pg.UpdateBy = sess.User.UserID;
                    pg.UpdateDate = DateTime.Now;
                    db.AT_PermissionGroup.Attach(pg);
                    db.UpdateExcept<AT_PermissionGroup>(pg, x => x.CreateBy, x => x.CreateDate);
                }
                else
                {
                //    pg.ClientID = sess.User.ClientID;
                    pg.CreateBy = sess.User.UserID;
                    pg.CreateDate = DateTime.Now;
                    db.Entry(pg).State = System.Data.Entity.EntityState.Added;
                }

                db.AT_PermissionGroupJunc.Where(x => x.PermissionGroupID == pg.PermissionGroupID).ToList().ForEach(tol => db.AT_PermissionGroupJunc.Remove(tol));
                db.SaveChanges();

                foreach (DataRow item in lstP.Rows)
                {
                    if (Convert.ToBoolean(item["Allow"]))
                    {
                        AT_PermissionGroupJunc pgj = new AT_PermissionGroupJunc();
                        pgj.PermissionGroupID = pg.PermissionGroupID;
                        pgj.PermissionID = Convert.ToInt32(item["PermissionID"]);
                        pgj.ClientID = 1;
                        db.AT_PermissionGroupJunc.Add(pgj);
                    }
                }

                db.SaveChanges();
                msg = "Permission Group saved successfully!";

            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }
            ViewBag.msg = msg;
            return PartialView("_PermissionGroupIndex", db.AT_PermissionGroup.ToList());

        }
        #endregion

      #region //UserProfile Area
        public ActionResult UserProfileIndex()
        {
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            return PartialView("UserProfileIndex", db.AT_Users.Where(x => x.UserID == sess.User.UserID).FirstOrDefault());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UserProfileSave(AT_Users usr)
        {
            string msg = "";
            Session_CRM sess = (Session_CRM)Session["CRM_Session"];
            try
            {
                var modal = db.AT_Users.AsNoTracking().Where(x => x.UserID == usr.UserID).FirstOrDefault();
             //   usr.TypeID = modal.TypeID;
                usr.RoleID = modal.RoleID;
                usr.UserID = sess.User.UserID;
                usr.Password = usr.Password != modal.Password ? CRM_Common.Encrypt(usr.Password) : usr.Password;

                if (usr.upload != null && usr.upload.ContentLength > 0)
                {
                    usr.PicName = usr.upload.FileName;
                    Guid g;
                    g = Guid.NewGuid();
                    string targetFolder = Server.MapPath("~/App_Data/Images");
                    string targetPath = Path.Combine(targetFolder, g.ToString() + Path.GetExtension(usr.upload.FileName));
                    usr.upload.SaveAs(targetPath);
                    usr.PicName = "/App_Data/Images/" + g.ToString() + Path.GetExtension(usr.upload.FileName);
                    usr.PicGuid = g.ToString();
                    db.AT_Users.Attach(usr);
                    db.UpdateOnly<AT_Users>(usr, x => x.FirstName, x => x.LastName,
                                              x => x.PicName, x => x.PicGuid, x => x.Email, x => x.Phone, x => x.Password);
                    if (sess != null)
                    {
                        sess.User.PicGuid = usr.PicGuid;
                        sess.User.PicName = usr.PicName;
                       }
                }
                else
                {
                    db.AT_Users.Attach(usr);
                    db.UpdateOnly<AT_Users>(usr, x => x.FirstName, x => x.LastName,
                                              x => x.Email, x => x.Phone, x => x.Password);
                }
                db.SaveChanges();
                msg = "Update successfully!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ViewBag.msg = msg;
            return PartialView("UserProfileIndex", db.AT_Users.Where(x => x.UserID == sess.User.UserID).FirstOrDefault());

        }
        #endregion

      
    }
}