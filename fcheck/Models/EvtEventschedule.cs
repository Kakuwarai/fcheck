using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class EvtEventschedule
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Facilitator { get; set; }
        public string? Details { get; set; }
        public string? Venue { get; set; }
        public int Status { get; set; }
        public byte IsDelete { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
