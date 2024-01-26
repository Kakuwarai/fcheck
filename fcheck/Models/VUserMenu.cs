using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class VUserMenu
    {
        public int? ParentMenuId { get; set; }
        public string? ParentIcon { get; set; }
        public int? ParentSort { get; set; }
        public string? ParentMenu { get; set; }
        public int MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? Icon { get; set; }
        public long UserId { get; set; }
        public byte Add { get; set; }
        public byte Edit { get; set; }
        public byte Delete { get; set; }
        public long? CreatedByUserId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
