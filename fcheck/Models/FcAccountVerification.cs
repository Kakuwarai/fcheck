using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcAccountVerification
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public int Verified { get; set; }
        public string VerificationCode { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
