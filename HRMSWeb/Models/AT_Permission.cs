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
    
    public partial class AT_Permission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AT_Permission()
        {
            this.AT_PermissionActionJunc = new HashSet<AT_PermissionActionJunc>();
            this.AT_PermissionGroupJunc = new HashSet<AT_PermissionGroupJunc>();
        }
    
        public int PermissionID { get; set; }
        public int PageID { get; set; }
        public string Permission { get; set; }
        public bool IsActive { get; set; }
    
        public virtual AT_Pages AT_Pages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AT_PermissionActionJunc> AT_PermissionActionJunc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AT_PermissionGroupJunc> AT_PermissionGroupJunc { get; set; }
    }
}
