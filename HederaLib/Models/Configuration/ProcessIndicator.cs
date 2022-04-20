using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class ProcessIndicator : CommonIndicatorBase
    {
        public string Name { get; init; }
        public string Value { get; set; }

        public string Rule { get; set; }
    }
}
