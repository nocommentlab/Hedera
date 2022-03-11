using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class PipeResult : CheckResult
    {
        public string Name { get; set; }
        public PipeIndicator PipeIndicator { get; set; }


        public PipeResult()
        {
            GUID_ResultId = Guid.NewGuid();
            Hostname = Dns.GetHostName();
            DATETIME_Datetime = DateTime.Now;
        }
    }
}
