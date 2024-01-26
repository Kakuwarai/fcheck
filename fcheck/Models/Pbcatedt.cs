using System;
using System.Collections.Generic;

namespace fcheck.Models
{
    public partial class Pbcatedt
    {
        public string PbeName { get; set; } = null!;
        public string? PbeEdit { get; set; }
        public short? PbeType { get; set; }
        public int? PbeCntr { get; set; }
        public short PbeSeqn { get; set; }
        public int? PbeFlag { get; set; }
        public string? PbeWork { get; set; }
    }
}
