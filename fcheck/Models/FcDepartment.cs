using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcDepartment
    {
        public int Id { get; set; }
        public string? Departments { get; set; }
        public string? Status { get; set; }
        public int? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
