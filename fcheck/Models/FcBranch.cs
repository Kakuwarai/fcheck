using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcBranch
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string Status { get; set; } = null!;
        public int IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
