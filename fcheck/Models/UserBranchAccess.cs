using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class UserBranchAccess
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string BranchCode { get; set; } = null!;
        public byte IsDefault { get; set; }
        public byte Status { get; set; }
        public long? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Branch BranchCodeNavigation { get; set; } = null!;
        public virtual User? CreatedByUser { get; set; }
        public virtual User? ModifiedByUser { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
