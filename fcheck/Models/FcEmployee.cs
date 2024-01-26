using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcEmployee
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        public string? Fullname { get; set; }
        public string? Sbu { get; set; }
        public string? Department { get; set; }
        public string? DeviceId { get; set; }
        public string Status { get; set; } = null!;
        public int Isdelete { get; set; }
        public int? Branch { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ImmediateHeadId { get; set; }
        public string? ImmediateHeadName { get; set; }
        public string? ImmediateEmail { get; set; }
    }
}
