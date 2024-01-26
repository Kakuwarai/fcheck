using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fcheck.Models
{
    public partial class UserMenu
    {

        [Key]
        [Column(Order = 1)]
        public int MenuId { get; set; }

        [Key]
        [Column(Order = 2)] 
        public long UserId { get; set; }
        public byte Add { get; set; }
        public byte Edit { get; set; }
        public byte Delete { get; set; }
        public string BranchCode { get; set; } = null!;
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
