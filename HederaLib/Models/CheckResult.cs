using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class CheckResult
    {
        #region Properties
        public Guid GUID_ResultId { get; set; }
        public DateTime DATETIME_Datetime { get; set; }
        public string Hostname { get; set; }
        public bool Result { get; set; }
        #endregion

        
    }
}
