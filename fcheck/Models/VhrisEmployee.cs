using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class VhrisEmployee
    {
        public string EmplId { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public string Fname { get; set; } = null!;
        public string? Mname { get; set; }
        public string? EmployeeName2 { get; set; }
        public string CorporateName { get; set; } = null!;
        public string? Departmentname { get; set; }
        public string? ImmediateId { get; set; }
        public string? ImmediateName { get; set; }
        public string? ImmediateEmail { get; set; }
        public string? Type { get; set; }
    }
}
