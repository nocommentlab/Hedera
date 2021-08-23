using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class EventIndicator : CommonIndicatorBase
    {
        public string Channel { get; init; }
        public ushort Id { get; init; }
        public string DatetimeStart { get; init; }
        public string DatetimeEnd { get; init; }
        public string DatetimeFormat { get; init; }
    }
}
