
using HRMSWeb.Models;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstituteMS.Controllers
{ //[SessionAuthorizeAttribute]
    public class AddPermissionController : Controller
    {
        //
        // GET: /AddPermission/
        HRMSEntities db = new HRMSEntities();

        public ActionResult Index()
        {
            GenerateClass();
            
            return View(db.AT_Permission.ToList());
        }

        [HttpGet]
        public ActionResult Save(int id)
        {
            ViewBag.PageList = db.AT_Pages.ToList();
            var lst = db.AT_Permission.Where(x => x.PermissionID == id).FirstOrDefault();
            if(lst!=null)
            {
                lst.Actions = string.Join(",", db.AT_PermissionActionJunc.Where(x => x.PermissionID == id).Select(x => x.Action).ToList());
            }
            return PartialView("Save", lst);
        }

       

        [HttpPost]
        public ActionResult Save(AT_Permission per)
        {
            string msg = "";
            try
            {
                Session_CRM sess = (Session_CRM)Session["CRM_Session"];

                if (per.PermissionID > 0)
                {

                    db.AT_Permission.Attach(per);
                    db.UpdateExcept<AT_Permission>(per);
                }
                else
                {

                    db.Entry(per).State = System.Data.Entity.EntityState.Added;
                }
                db.SaveChanges();

                db.AT_PermissionActionJunc.Where(x => x.PermissionID == per.PermissionID).ToList().ForEach(tol => db.AT_PermissionActionJunc.Remove(tol));
                db.SaveChanges();
                string[] act =  per.Actions.Split(',');
                for (int i = 0; i < act.Length ; i++)
                {
                    AT_PermissionActionJunc PA = new AT_PermissionActionJunc();
                    PA.PermissionID = per.PermissionID;
                    PA.Action = act[i].ToString();
                    db.Entry(PA).State = System.Data.Entity.EntityState.Added;
                     db.SaveChanges();
                }
                GenerateClass();
                msg = "Permission saved successfully!";

            }
            catch (Exception ex)
            {
               
                msg = ex.Message;
            }
            ViewBag.msg = msg;
            db = new HRMSEntities();
            return PartialView("_Index", db.AT_Permission.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }



        public void GenerateClass()
        {
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            CodeNamespace globalNamespace = new CodeNamespace();
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration("PermissionEnum");
            targetClass.Attributes = MemberAttributes.Public;          
            targetClass.IsClass = true;
            
            globalNamespace.Imports.Add(new CodeNamespaceImport("System"));
            targetUnit.Namespaces.Add(globalNamespace);

         CodeNamespace samples = new CodeNamespace();
            var dblst = db.AT_Permission.Where(x => x.IsActive == true).ToList();
            foreach (var item in dblst)
            {
               
                CodeMemberField cField = new CodeMemberField();
                cField.Attributes = MemberAttributes.Public | MemberAttributes.Const;               
                cField.Type = new CodeTypeReference(typeof(string));
                cField.Name = item.Permission.Replace(" ","");
                cField.InitExpression = new CodePrimitiveExpression(item.Permission);
                targetClass.Members.Add(cField);
               
               
            }
          samples.Types.Add(targetClass);
          targetUnit.Namespaces.Add(samples);
        
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            string path = Server.MapPath("\\Models\\PermissionEnum.cs");
            using (StreamWriter sourceWriter = new StreamWriter(path))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }


    }
}
