using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class Indicator
    {
        public List<RegistryIndicator> Registry { get; init; }
        public List<FileIndicator> File { get; init; }
        public List<ProcessIndicator> Process { get; init; }
        public List<EventIndicator> Event { get; init; }
        public List<PipeIndicator> Pipe { get; init; }
    }
}
