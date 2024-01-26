using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class VBranch
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Alias { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ContactId { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string? Area { get; set; }
        public string? Region { get; set; }
        public string? MapReference { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int? CompanyId { get; set; }
        public string? Company { get; set; }
        public int? GroupId { get; set; }
        public string? Group { get; set; }
        public byte Status { get; set; }
        public byte IsDelete { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
