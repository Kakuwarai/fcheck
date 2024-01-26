using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcHealthDeclaration
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string? HealthDeclaration { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
