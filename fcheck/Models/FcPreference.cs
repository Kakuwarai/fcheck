using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcPreference
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? WithCamera { get; set; }
        public string? ShowBreakTime { get; set; }
        public byte[]? Branch { get; set; }
    }
}
