using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class Config
    {
        public Information Information { get; init; }
        public TheHive TheHive { get; init; }
        public Indicator Indicators { get; init; }


    }
}
