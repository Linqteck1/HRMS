using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace HRMSWeb.Models
{
    public class ErrorHandling
    {
        public void WriteError(Exception errorMessage)
        {
            string path = "";
            try
            {
                path = "~/Logs/" + DateTime.Now.ToString("MMM dd yyyy") + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                using (StreamWriter w = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    string err = "Error in: " + System.Web.HttpContext.Current.Request.Url.ToString() +
                                  ". Error Message:" + errorMessage.InnerException.ToString();
                    w.WriteLine(err);
                    w.WriteLine("__________________________");
                    w.Flush();
                    w.Close();
                }

            }
            catch (Exception ex)
            {
            }

        }
    }
}