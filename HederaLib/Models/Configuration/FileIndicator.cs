using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class FileIndicator : CommonIndicatorBase
    {
        public string Path { get; set; }
        public string Filename { get; init; }
        public bool IsRecursive { get; init; }
        public string Sha256Hash { get; init; }
        public string Value { get; init; }
        public string Rule { get; init; }
    }
}
