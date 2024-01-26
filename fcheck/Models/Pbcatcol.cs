using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class Pbcatcol
    {
        public string PbcTnam { get; set; } = null!;
        public int? PbcTid { get; set; }
        public string PbcOwnr { get; set; } = null!;
        public string PbcCnam { get; set; } = null!;
        public short? PbcCid { get; set; }
        public string? PbcLabl { get; set; }
        public short? PbcLpos { get; set; }
        public string? PbcHdr { get; set; }
        public short? PbcHpos { get; set; }
        public short? PbcJtfy { get; set; }
        public string? PbcMask { get; set; }
        public short? PbcCase { get; set; }
        public short? PbcHght { get; set; }
        public short? PbcWdth { get; set; }
        public string? PbcPtrn { get; set; }
        public string? PbcBmap { get; set; }
        public string? PbcInit { get; set; }
        public string? PbcCmnt { get; set; }
        public string? PbcEdit { get; set; }
        public string? PbcTag { get; set; }
    }
}
