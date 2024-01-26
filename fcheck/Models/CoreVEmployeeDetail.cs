using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class CoreVEmployeeDetail
    {
        public string EmplId { get; set; } = null!;
        public string Employeename { get; set; } = null!;
        public string? Employeename2 { get; set; }
        public string Lname { get; set; } = null!;
        public string Fname { get; set; } = null!;
        public string? Mname { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Status { get; set; }
        public string Sex { get; set; } = null!;
        public string Civilstatus { get; set; } = null!;
        public string? Emailadd { get; set; }
        public DateTime? Datehired { get; set; }
        public DateTime Effectivitydate { get; set; }
        public string? Corporate { get; set; }
        public string CorporateName { get; set; } = null!;
        public string? Branch { get; set; }
        public string Branchname { get; set; } = null!;
        public string? Brancharea { get; set; }
        public string? Position { get; set; }
        public string? Positionname { get; set; }
        public string? Department { get; set; }
        public string? Departmentname { get; set; }
        public string Section { get; set; } = null!;
        public string? Sectionname { get; set; }
        public string? Levelcode { get; set; }
        public string? Levelname { get; set; }
        public string? Joblevelgroup { get; set; }
        public string? Class { get; set; }
        public string? Type { get; set; }
        public string Typedescription { get; set; } = null!;
        public string Employeepresentaddress { get; set; } = null!;
        public string Employeehomeaddress { get; set; } = null!;
        public string? Employeepresentcontact { get; set; }
        public string? Homecontact { get; set; }
        public string? ImmediateId { get; set; }
        public string? ImmediateName { get; set; }
        public DateTime? ImmediateBday { get; set; }
        public string? ImmediateStatus { get; set; }
        public string? ImmediateEmail { get; set; }
        public DateTime? ImmediateDhired { get; set; }
        public string? ImmediateSbu { get; set; }
        public string? ImmediateBranch { get; set; }
        public string? ImmediatePosition { get; set; }
        public string? ImmediateDepartment { get; set; }
        public string? ImmediateSection { get; set; }
        public string? ImmediateLevel { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public string? Sss { get; set; }
        public string? Districtcode { get; set; }
        public string? Districtname { get; set; }
    }
}
