using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HRMSWeb.Models
{
    public class Permissions
    {
        public AT_RolePermissionJunc AT_RolePermissionJunc { get; set; }
        public AT_Permission AT_Permission { get; set; }
        public List<AT_PermissionActionJunc> AT_PermissionActionJunc { get; set; }
        public AT_Pages AT_Pages { get; set; }
        public AT_Modules AT_Modules { get; set; }
        public AT_UserExtraPermissionjunc AT_UserExtraPermissionjunc { get; set; }

    }
    public class AllPermissions
    {
        public List<AT_Permission> AT_CRMC_Permission { get; set; }
        public List<AT_PermissionGroup> AT_CRM_PermissionGroup { get; set; }
        public List<AT_RolePermissionJunc> AT_RolePermissionJunc { get; set; }
    }
    [MetadataType(typeof(AT_PermissionGroupMetaData))]
    public partial class AT_PermissionGroup
    {

        public string json_Permissions { get; set; }
        public bool check { get; set; }
        public int juncID { get; set; }
    }

    public class ExtraPermissions
    {
        public int PermissionGroupID { get; set; }
        public int UserExtraPermissionjuncID { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
        public bool Allow { get; set; }
    }
    public partial class AT_Permission
    {
        public string Actions { get; set; }
    }
    public partial class AT_Users
    {
        public string CRM_URL { get; set; }
        public HttpPostedFileBase upload { get; set; }
    }
    [MetadataType(typeof(AT_StudentMetaData))]
    public partial class AT_Student
    {
        public string StudentStringID { get; set; }
        public HttpPostedFileBase AddmissionFormUpload { get; set; }
        public HttpPostedFileBase StudentPicUpload { get; set; }
    }
    [MetadataType(typeof(AT_ClassMetaData))]
    public partial class AT_Class
    {
        public IEnumerable<int> Section { get; set; }
        public string ClassStringID { get; set; }
    }
    [MetadataType(typeof(AT_SectionMetaData))]
    public partial class AT_Section
    {

    }
    [MetadataType(typeof(AT_SessionMetaData))]
    public partial class AT_Session
    {

    }
    public partial class AT_StudentClassLog
    {
        public int[] StudentIDs { get; set; }
    }
    public partial class AT_ChallanVoucherDetail
    {
        public int StudentID { get; set; }
        public int[] FeeTypeIDArray { get; set; }
        public int[] ClassID { get; set; }
        public string FeeTypeName { get; set; }
    }
    public partial class AT_ChallanVoucher
    {
        public AT_ChallanVoucherDetail detail { get; set; }
        public int[] FeeTypeIDArray { get; set; }
        public int[] ClassID { get; set; }
        public string FeeTypeName { get; set; }
    }
    public partial class AT_FeeTypesType
    {
        public AT_FeeTypesTypeDetail detail { get; set; }
    }
    public partial class AT_FeeTypesTypeDetail
    {
        public string[] MonthArray { get; set; }
        public string FeeTypeName { get; set; }
    }
    public partial class AT_FeeStructure
    {
        public string FeeSID { get; set; }
    }
}
