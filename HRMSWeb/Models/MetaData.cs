using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace HRMSWeb.Models
{

    public class AT_PermissionGroupMetaData
    {
        [Required(ErrorMessage = "Group Name is required.")]
        public string Name { get; set; }
    }
    public class AT_RoleMetaData
    {
        [Required(ErrorMessage = "Role Name is required.")]
        public string RoleName { get; set; }
    }
    public class AT_StudentMetaData
    {
        [Required(ErrorMessage = "Student Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Father Name is required.")]
        public string FatherName { get; set; }
        [Required(ErrorMessage = "Class is required.")]
        public int ClassID { get; set; }
        [Required(ErrorMessage = "Section is required.")]
        public int SectionID { get; set; }
        [Required(ErrorMessage = "Religion is required.")]
        public string Religion { get; set; }
        [Required(ErrorMessage = "Nationality is required.")]
        public string Nationality { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public System.DateTime DOB { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Mobile # is required.")]
        public string MobNo { get; set; }
    }
    public class AT_ClassMetaData
    {
        [Required(ErrorMessage = "Class Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Section is required.")]
        public IEnumerable<int> Section { get; set; }
    }
    public class AT_SectionMetaData
    {
        [Required(ErrorMessage = "Section Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Strength is required.")]
        public int Strength { get; set; }
    }
    public class AT_SessionMetaData
    {
        [Required(ErrorMessage = "Session is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Start Date is required."),DataType(DataType.Date)]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required."), DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
