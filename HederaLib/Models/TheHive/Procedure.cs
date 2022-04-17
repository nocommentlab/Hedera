using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.TheHive
{
    public class Procedure
    {
        public string CaseId { get; set; }
        public string Tactic { get; set; }
        public string Description { get; set; }
        public string PatternId { get; set; }
        public long OccurDate { get; set; }
    }
}
