using ncl.hedera.HederaLib.Models.TheHive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class TheHive
    {
        public Case Case { get; set; }
        public List<Procedure> Procedures { get; set; }
    }
}
