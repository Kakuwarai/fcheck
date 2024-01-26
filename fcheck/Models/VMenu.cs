using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class VMenu
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentMenuId { get; set; }
        public string? ParentMenu { get; set; }
        public int? Sort { get; set; }
        public byte IsMobile { get; set; }
        public byte MobileDefault { get; set; }
        public byte IsBrowser { get; set; }
        public byte BrowserDefault { get; set; }
        public string? Icon { get; set; }
        public byte IsTransaction { get; set; }
        public byte Status { get; set; }
        public string StatusName { get; set; } = null!;
        public byte IsDelete { get; set; }
        public long? CreatedByUserId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
