using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class EvtEventschedulelog
    {
        public int Id { get; set; }
        public int? EventScheduleId { get; set; }
        public string? EmployeeId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
