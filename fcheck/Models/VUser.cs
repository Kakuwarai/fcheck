using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class VUser
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        public string Fullname { get; set; } = null!;
        public string Fullname2 { get; set; } = null!;
        public string? Nickname { get; set; }
        public string? EmailAddress { get; set; }
        public string? ContactNumber { get; set; }
        public string? HashCode { get; set; }
        public byte Status { get; set; }
        public byte IsDeactivated { get; set; }
        public string? DeactivateReason { get; set; }
        public byte IsRegistered { get; set; }
        public byte IsVerified { get; set; }
        public int? VerifiedById { get; set; }
        public string? VerifiedBye { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public long CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
