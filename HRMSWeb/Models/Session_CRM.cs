
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSWeb.Models
{
    public class Session_CRM
    {

            public AT_Users User { get; set; }
            public List<Permissions> AllPermissions { get; set; }
            public List<AT_Modules> AT_Modules { get; set; }         
           
        

     

    }
}