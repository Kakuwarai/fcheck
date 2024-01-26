using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcAccountBranch
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? BranchId { get; set; }
        public string? Branch { get; set; }
        public int? Permission { get; set; }
    }
}
