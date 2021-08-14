using ncl.hedera.HederaLib.Helpers;
using ncl.hedera.HederaLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ncl.hedera.HederaLib
{
    public class HederaLib
    {
        #region Costants

        #endregion

        #region Members
        private readonly string _STRING_IocConfigFile = null;
        #endregion

        #region Properties
        #endregion

        #region Private Functions

        #endregion

        #region Public Functions

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HederaLib()
        {
            throw new Exception("Default constructor not implemented, call IoCScannerLib(string iocConfigFile)");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="STRING_IocConfigFile">The IoC configuration file path</param>
        public HederaLib(string STRING_IocConfigFile)
        {
            _STRING_IocConfigFile = STRING_IocConfigFile;
        }

        /// <summary>
        /// Deserialize the Yaml configuration file
        /// </summary>
        /// <returns>The dynamic object deserialized</returns>
        public dynamic DeserializeIoCConfiguration()
        {
            StringReader stringReader = new(File.ReadAllText(_STRING_IocConfigFile));

            var deserializer = new Deserializer();
            return deserializer.Deserialize<dynamic>(stringReader);

        }

        /// <summary>
        /// Checks the registry IoC
        /// </summary>
        /// <param name="OBJECT_RegistryIoC">The Registry IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static Task<RegistryKeyResult> CheckRegistryKey(dynamic OBJECT_RegistryIoC)
        {

            RegistryKeyResult registryKeyResult = null;

            OBJECT_RegistryIoC["key"] = Utils.ReplaceTemplate(OBJECT_RegistryIoC["key"]);
            // Extracts the registry data value
            RegistryItem OBJECT_ExtractedDataValue = Utils.ReadRegistryDataValue(OBJECT_RegistryIoC);

            if (null != OBJECT_ExtractedDataValue)
            {

                registryKeyResult = new();

                registryKeyResult.Result = (string)OBJECT_RegistryIoC["type"] switch
                {
                    "exists" => (OBJECT_ExtractedDataValue != null),
                    "data_value_regex" => ((null != OBJECT_ExtractedDataValue) &&
                                            Regex.IsMatch(OBJECT_ExtractedDataValue.OBJECT_ValueData.ToString(), OBJECT_RegistryIoC["value_data_regex"], RegexOptions.IgnoreCase)),
                    _ => false
                };

                //switch ((string)OBJECT_RegistryIoC["type"])
                //{
                //    case "exists":
                //        registryKeyResult.Result = (OBJECT_ExtractedDataValue != null);
                //        break;

                //    case "data_value_regex":
                //        registryKeyResult.Result = ((null != OBJECT_ExtractedDataValue) &&
                //                            Regex.IsMatch(OBJECT_ExtractedDataValue.OBJECT_ValueData.ToString(), OBJECT_RegistryIoC["value_data_regex"], RegexOptions.IgnoreCase));
                //        break;
                //    default:
                //        registryKeyResult.Result = false;
                //        break;
                //}


                registryKeyResult.RegistryItem = OBJECT_ExtractedDataValue;

                registryKeyResult.GUID_ResultId = Guid.NewGuid();
                registryKeyResult.Hostname = Dns.GetHostName();
                registryKeyResult.DATETIME_Datetime = DateTime.Now;
            }

            return Task.FromResult<RegistryKeyResult>(registryKeyResult);
        }

        /// <summary>
        /// Checks the file IoC
        /// </summary>
        /// <param name="OBJECT_FileIoC">The File IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        public static Task<FileResult> CheckFile(dynamic OBJECT_FileIoC)
        {

            FileResult fileResult = null;
            FileItem fileItem;

            OBJECT_FileIoC["path"] = Utils.ReplaceTemplate(OBJECT_FileIoC["path"]);

            fileItem = Utils.IsFileExists(OBJECT_FileIoC);

            if (null != fileItem)
            {
                fileResult = new FileResult
                {
                    Result = (string)OBJECT_FileIoC["type"] switch
                    {
                        "exists" => true,
                        "hash" => (Utils.CalculateSha256Hash(fileItem.STRING_Path).Equals(OBJECT_FileIoC["sha256_hash"].ToLower())),
                        "imphash"=> (Utils.CalculateImphash(fileItem.STRING_Path).Equals(OBJECT_FileIoC["value"].ToLower())),
                        _ => false
                    },


                    FileItem = fileItem,
                    GUID_ResultId = Guid.NewGuid(),
                    Hostname = Dns.GetHostName(),
                    DATETIME_Datetime = DateTime.Now
                };

            }
            return Task.FromResult<FileResult>(fileResult);
        }

        /// <summary>
        /// Checks the process IoC
        /// </summary>
        /// <param name="OBJECT_ProcessIoC">The Process IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        public static Task<bool> CheckProcess(dynamic OBJECT_ProcessIoC)
        {
            bool BOOL_CheckResult = false;
            BOOL_CheckResult = (string)OBJECT_ProcessIoC["type"] switch
            {
                "exists" => Helpers.Process.GetAccessibleProcesses().Where(process => process.MainModule.ModuleName.Equals(OBJECT_ProcessIoC["name"])).Any(),
                "hash" => Helpers.Process.GetProcessByExecutableName((string)OBJECT_ProcessIoC["name"]).Where(process => string.Compare(Helpers.Utils.CalculateSha256Hash(process.MainModule.FileName), OBJECT_ProcessIoC["sha256_hash"], true) == 0).Any(),
                _ => false,
            };
            return Task.FromResult<bool>(BOOL_CheckResult);
        }

        [SupportedOSPlatform("windows")]
        public static Task<bool> CheckEvent(dynamic OBJECT_EventIoC, ref List<EventLogEntry> lEventLogEntries)
        {
            bool BOOL_CheckResult = true;
            EventLog remoteEventLogs;

            remoteEventLogs = new EventLog(OBJECT_EventIoC["log"]);
            lEventLogEntries = remoteEventLogs.Entries.Cast<EventLogEntry>().ToList();
            switch ((string)OBJECT_EventIoC["type"])
            {

                case "exists":
                    lEventLogEntries = lEventLogEntries.Where(eventLogEntry => (eventLogEntry.InstanceId == int.Parse(OBJECT_EventIoC["event_id"]) &&
                                                       (!OBJECT_EventIoC.ContainsKey("datetime_start") || eventLogEntry.TimeGenerated > DateTime.ParseExact(OBJECT_EventIoC["datetime_start"], OBJECT_EventIoC["datetime_format"], CultureInfo.InvariantCulture)) &&
                                                       (!OBJECT_EventIoC.ContainsKey("datetime_end") || eventLogEntry.TimeGenerated < DateTime.ParseExact(OBJECT_EventIoC["datetime_end"], OBJECT_EventIoC["datetime_format"], CultureInfo.InvariantCulture)))).ToList();

                    BOOL_CheckResult = lEventLogEntries.Count > 0;
                    break;
            }


            return Task.FromResult<bool>(BOOL_CheckResult);
        }
    }
    #endregion
}
