using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class EvtEvent
    {
        public int Id { get; set; }
        public string? Alias { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }
        public byte IsDelete { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
