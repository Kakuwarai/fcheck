using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcAttendance
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string WorkPlace { get; set; } = null!;
        public string TimeIn { get; set; } = null!;
        public string? TimeOut { get; set; }
        public string? TotalTime { get; set; }
        public string? FirstBreakOut { get; set; }
        public string? FirstBreakIn { get; set; }
        public string? SecondBreakOut { get; set; }
        public string? SecondBreakIn { get; set; }
        public string? ThirdBreakOut { get; set; }
        public string? ThirdBreakIn { get; set; }
        public string? LatLongIn { get; set; }
        public string? LocationIn { get; set; }
        public string? LatLongOut { get; set; }
        public string? LocationOut { get; set; }
        public string? BranchIn { get; set; }
        public string? BranchOut { get; set; }
        public string? Department { get; set; }
        public string? Sbu { get; set; }
        public DateTime? Date { get; set; }
        public string? WorkPlaceOut { get; set; }
    }
}
