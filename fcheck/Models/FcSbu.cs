using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class FcSbu
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
