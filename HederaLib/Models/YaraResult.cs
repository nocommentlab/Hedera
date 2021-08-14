using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class YaraResult
    {
        public string RuleIdentifier { get; init; }
        public List<YaraMatch> Matches { get; set; }

        public YaraResult()
        {
            Matches = new();
        }
    }
}
