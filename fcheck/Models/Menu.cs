using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class Menu
    {
        public int Id { get; set; }
        public string? MenuName { get; set; }
        public int? ParentMenuId { get; set; }
        public int? Sort { get; set; }
        public byte IsMobile { get; set; }
        public byte MobileDefault { get; set; }
        public byte IsBrowser { get; set; }
        public byte BrowserDefault { get; set; }
        public string? Icon { get; set; }
        public byte IsTransaction { get; set; }
        public byte Status { get; set; }
        public byte IsDelete { get; set; }
        public long? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
