using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class ProcessResult : CheckResult
    {
        //public FileItem FileItem { get; set; }
        public string Name { get; set; }
        public ProcessIndicator ProcessIndicator { get; set; }

        public ProcessResult()
        {
            this.GUID_ResultId = Guid.NewGuid();
            this.Hostname = Dns.GetHostName();
            this.DATETIME_Datetime = DateTime.Now;
        }

    }
}
