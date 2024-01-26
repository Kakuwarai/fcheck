using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class Pbcattbl
    {
        public string PbtTnam { get; set; } = null!;
        public int? PbtTid { get; set; }
        public string PbtOwnr { get; set; } = null!;
        public short? PbdFhgt { get; set; }
        public short? PbdFwgt { get; set; }
        public string? PbdFitl { get; set; }
        public string? PbdFunl { get; set; }
        public short? PbdFchr { get; set; }
        public short? PbdFptc { get; set; }
        public string? PbdFfce { get; set; }
        public short? PbhFhgt { get; set; }
        public short? PbhFwgt { get; set; }
        public string? PbhFitl { get; set; }
        public string? PbhFunl { get; set; }
        public short? PbhFchr { get; set; }
        public short? PbhFptc { get; set; }
        public string? PbhFfce { get; set; }
        public short? PblFhgt { get; set; }
        public short? PblFwgt { get; set; }
        public string? PblFitl { get; set; }
        public string? PblFunl { get; set; }
        public short? PblFchr { get; set; }
        public short? PblFptc { get; set; }
        public string? PblFfce { get; set; }
        public string? PbtCmnt { get; set; }
    }
}
