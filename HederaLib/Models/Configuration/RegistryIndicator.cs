﻿using ncl.hedera.HederaLib.Models.TheHive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class RegistryIndicator : CommonIndicatorBase
    {
        public string BaseKey { get; init; }
        public string Key { get; set; }
        public string ValueNameRegex { get; init; }
        public string ValueDataRegex { get; init; }
        public bool IsRecursive { get; init; }

        public Observable Observable { get; set; }

    }
}
