using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class RegistryKeyResult : CheckResult, ISerializable
    {
        public RegistryItem RegistryItem { get; set; }
        public RegistryIndicator RegistryIndicator { get; set; }

        public RegistryKeyResult()
        {
            this.GUID_ResultId = Guid.NewGuid();
            this.Hostname = Dns.GetHostName();
            this.DATETIME_Datetime = DateTime.Now;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
