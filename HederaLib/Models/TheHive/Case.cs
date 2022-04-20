using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.TheHive
{
    public class Case
    {
        public string Status { get; set; }
        public int Severity { get; set; }
        public int Tlp { get; set; }
        public int Pap { get; set; }
        public long StartDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }

    }
}
