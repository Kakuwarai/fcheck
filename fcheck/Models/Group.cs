using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class Group
    {
        public Group()
        {
            Branches = new HashSet<Branch>();
        }

        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ContactId { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string Status { get; set; } = null!;
        public byte IsDelete { get; set; }
        public long? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
    }
}
