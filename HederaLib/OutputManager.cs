using ncl.hedera.HederaLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib
{
    /// <summary>
    /// This class manages the output result channel
    /// </summary>
    public class OutputManager
    {
        #region Costants
        private const string __REGISTRY_OUTPUT__ = "registry_evidence.json";
        #endregion

        public enum OUTPUT_MODE
        {
            TO_STDOUT,
            TO_FILE,
            TO_TCP // YES, THIS IS A NEW FEATURE :)
        }
        
        public OUTPUT_MODE outputMode = OUTPUT_MODE.TO_STDOUT;


        public static void WriteRegistryEvidenciesResult(List<RegistryKeyResult> lRegistryKeyResult, OUTPUT_MODE outputMode)
        {
            // To parse the output using Out-GridView. I know, it is PowerShell :)
            // $(Get - Content.\registry_evidence.json | ConvertFrom - Json)  |
            // Select - Object @{ Name = 'DateTime'; Expression = { $_.DATETIME_Datetime } }, 
            //   @{ Name = 'Result'; Expression = { $_.Result } }, 
            //   @{ Name = 'GUID'; Expression = { $_.RegistryIndicator.Guid } }, 
            //   @{ Name = 'Type'; Expression = { $_.RegistryIndicator.Type } }, 
            //   @{ Name = 'BaseKey'; Expression = { $_.RegistryIndicator.BaseKey } }, 
            //   @{ Name = 'Key'; Expression = { $_.RegistryIndicator.Key } }, 
            //   @{ Name = 'Key Path'; Expression = { $_.RegistryItem.STRING_Name } }, 
            //   @{ Name = 'ValueNameRegEx'; Expression = { $_.RegistryIndicator.ValueNameRegex } },
            //   @{ Name = 'ValueName'; Expression = { $_.RegistryItem.STRING_ValueName } }, 
            //   @{ Name = 'ValueDataRegEx'; Expression = { $_.RegistryIndicator.ValueDataRegex } },
            //   @{ Name = 'ValueData'; Expression = { $_.RegistryItem.STRING_ValueData } } |
            // Out - GridView

            File.WriteAllText(__REGISTRY_OUTPUT__, JsonSerializer.Serialize(lRegistryKeyResult));
        }


    }
}
