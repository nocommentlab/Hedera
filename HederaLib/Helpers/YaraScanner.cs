using libyaraNET;
using ncl.hedera.HederaLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Helpers
{
    public class YaraScanner
    {

        #region Members
        public enum Type
        {
            BINARY_TEXT,
            MEMORY
        }

        #endregion
        public static List<YaraResult> VerifyBinTxtYaraRule(string STRING_FilePath, string STRING_YaraRule)
        {
            List<YaraResult> yaraResults = null;

            List<ScanResult> results = QuickScan.File(STRING_FilePath, STRING_YaraRule);


            if (results.Any())
            {
                yaraResults = new();

                yaraResults.Add(new YaraResult
                {
                    RuleIdentifier = results[0].MatchingRule.Identifier,
                });
            }

            return yaraResults;
        }

        public static List<YaraResult> VerifyProcessYaraRule(System.Diagnostics.Process process, string STRING_YaraRule)
        {
            List<YaraResult> yaraResults = new();


            List<ScanResult> results = QuickScan.Process(process.Id, STRING_YaraRule);

            if (results.Any())
            { 

                yaraResults.Add(new YaraResult
                {
                    RuleIdentifier = results[0].MatchingRule.Identifier,
                });
            }



            return yaraResults;
        }

    }
}
