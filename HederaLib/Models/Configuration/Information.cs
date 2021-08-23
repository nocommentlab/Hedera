using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class Information
    {
        public string Guid { get; init; }
        public string Author { get; init; }
        public string Date { get; set; }
        public string Modified { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        
    }
}
