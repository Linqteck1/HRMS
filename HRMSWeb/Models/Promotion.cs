//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMSWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Promotion
    {
        public int PromotionID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> DesignationID { get; set; }
    
        public virtual Designation Designation { get; set; }
    }
}
