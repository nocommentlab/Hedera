using ncl.hedera.HederaLib.Helpers;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        [Obsolete]
        public HederaLib(string STRING_IocConfigFile)
        {
            _STRING_IocConfigFile = STRING_IocConfigFile;
        }

        /// <summary>
        /// Deserialize the Yaml configuration file
        /// </summary>
        /// <returns>The dynamic object deserialized</returns>
        public static Config DeserializeIoCConfiguration(string STRING_IocFile)
        {

            Config CONFIG_HederaConfiguration = null;
            try
            {
                var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                   .Build();

                CONFIG_HederaConfiguration = deserializer.Deserialize<Config>(File.ReadAllText(STRING_IocFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return CONFIG_HederaConfiguration;

        }


        [SupportedOSPlatform("windows")]
        public static async Task CheckRegistryIndicators(List<RegistryIndicator> lRegistryIndicator)
        {
            List<RegistryKeyResult> registryKeyResults = new();

            foreach (RegistryIndicator registryIoC in lRegistryIndicator)
            {
                foreach (RegistryKeyResult registryKeyResult in await CheckRegistryKey(registryIoC))
                {
                    if (registryKeyResult is not null)
                    {
                        registryKeyResults.Add(registryKeyResult);
                    }
                }

            }

            OutputManager.WriteRegistryEvidenciesResult(registryKeyResults, OutputManager.OUTPUT_MODE.TO_FILE);
        }

        [SupportedOSPlatform("windows")]
        public static async Task CheckFileIndicators(List<FileIndicator> lFileIndicator)
        {
            List<FileResult> fileResults = new();

            foreach (FileIndicator fileIoC in lFileIndicator)
            {
                foreach (FileResult fileResult in await CheckFile(fileIoC))
                {
                    if (null != fileResult)
                    {
                        fileResults.Add(fileResult);
                    }
                }
            }

            // Add OutputManager
        }
        /// <summary>
        /// Checks the registry IoC
        /// </summary>
        /// <param name="OBJECT_RegistryIoC">The Registry IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static Task<List<RegistryKeyResult>> CheckRegistryKey(RegistryIndicator registryIoc)
        {

            List<RegistryKeyResult> lRegistryKeyResult = null;
            RegistryKeyResult registryKeyResult = null;

            registryIoc.Key = Utils.ReplaceTemplate(registryIoc.Key);

            lRegistryKeyResult = new();


            // Extracts the registry data value
            List<RegistryItem> lOBJECT_ExtractedDataValue = Utils.ReadRegistryDataValue(registryIoc);

            if (null != lOBJECT_ExtractedDataValue && lOBJECT_ExtractedDataValue.Count > 0)
            {
                foreach (RegistryItem registryItem in lOBJECT_ExtractedDataValue)
                {

                    lRegistryKeyResult.Add(new RegistryKeyResult()
                    {
                        Result = registryIoc.Type switch
                        {
                            "exists" => (registryItem != null),
                            "data_value_regex" => ((null != registryItem) &&
                                                    Regex.IsMatch(registryItem.OBJECT_ValueData.ToString(), registryIoc.ValueDataRegex, RegexOptions.IgnoreCase)),
                            _ => false
                        },

                        RegistryItem = registryItem,
                        RegistryIndicator = registryIoc
                    });

                }

            }
            else
            {
                registryKeyResult = new();
                registryKeyResult.RegistryIndicator = registryIoc;

                // Adds the element to the the list when the Registry not exists
                lRegistryKeyResult.Add(registryKeyResult);
            }




            return Task.FromResult(lRegistryKeyResult);

        }

        /// <summary>
        /// Checks the file IoC
        /// </summary>
        /// <param name="OBJECT_FileIoC">The File IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static Task<List<FileResult>> CheckFile(FileIndicator fileIoc)
        {
            bool BOOL_TempResult;

            List<FileResult> lFileResult = null;
            List<FileItem> lFileItem;


            lFileResult = new();

            fileIoc.Path = Utils.ReplaceTemplate(fileIoc.Path);

            lFileItem = Utils.IsFileExists(fileIoc);

            if (null != lFileItem && lFileItem.Count > 0)
            {
                

                foreach (FileItem fileItem in lFileItem)
                {
                    BOOL_TempResult = fileIoc.Type switch
                    {
                        "exists" => true,
                        "hash" => Utils.CalculateSha256Hash(fileItem.STRING_Path).Equals(fileIoc.Sha256Hash.ToLower()),
                        "imphash" => Utils.CalculateImphash(fileItem.STRING_Path).Equals(fileIoc.Value.ToLower()),
                        "yara" => Utils.VerifyYaraRule(STRING_FilePath: fileItem.STRING_Path, STRING_YaraRule: fileIoc.Rule).Count > 0,
                        _ => false
                    };


                    lFileResult.Add(new FileResult
                    {
                        Result = BOOL_TempResult,
                        FileItem = fileItem,
                        FileIndicator = fileIoc

                    });
                }

            }
            else /* If the file doesn't exist, add only the FileIndicator */
            {

                lFileResult.Add(new FileResult() {FileIndicator = fileIoc  });

            }

            return Task.FromResult(lFileResult);
        }


        /// <summary>
        /// Checks the file IoC
        /// </summary>
        /// <param name="OBJECT_FileIoC">The File IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static Task<List<PipeResult>> CheckPipe(PipeIndicator pipeIoC)
        {
            bool BOOL_TempResult;
            List<PipeResult> lPipeResult = null;
            List<string> lNamedPipe;


            lNamedPipe = Utils.CheckNamedPipeExists(pipeIoC.Name);

            if (null != lNamedPipe)
            {
                lPipeResult = new();

                foreach (string namedPipe in lNamedPipe)
                {
                    BOOL_TempResult = pipeIoC.Type switch
                    {
                        "exists" => true,
                        _ => false
                    };

                    if (true == BOOL_TempResult)
                    {
                        lPipeResult.Add(new PipeResult
                        {
                            Result = true,
                            Name = namedPipe,
                            GUID_ResultId = Guid.NewGuid(),
                            Hostname = Dns.GetHostName(),
                            DATETIME_Datetime = DateTime.Now
                        });

                    }
                }

            }

            return Task.FromResult(lPipeResult);
        }

        /// <summary>
        /// Checks the process IoC
        /// </summary>
        /// <param name="OBJECT_ProcessIoC">The Process IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        public static Task<bool> CheckProcess(ProcessIndicator processIoc)
        {
            bool BOOL_CheckResult = false;
            BOOL_CheckResult = processIoc.Type switch
            {
                "exists" => Helpers.Process.GetAccessibleProcesses().Where(process => process.MainModule.ModuleName.Equals(processIoc.Name)).Any(),
                "hash" => Helpers.Process.GetProcessByExecutableName(processIoc.Name).Where(process => string.Compare(Helpers.Utils.CalculateSha256Hash(process.MainModule.FileName), processIoc.Sha256Hash, true) == 0).Any(),
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
