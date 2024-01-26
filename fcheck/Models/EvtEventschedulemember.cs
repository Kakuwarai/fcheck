using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class EvtEventschedulemember
    {
        public int Id { get; set; }
        public int EventScheduleId { get; set; }
        public string? EmployeeId { get; set; }
        public string? Team { get; set; }
        public string? RoomNumber { get; set; }
        public string? TableNumber { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
