using ncl.hedera.HederaLib.Controllers;
using ncl.hedera.HederaLib.Helpers;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
using ncl.hedera.HederaLib.Models.TheHive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Text.Json;
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
        private static TheHiveManager _theHiveManager = null;
        #endregion

        #region Properties
        public static TheHiveManager TheHiveManager
        {
            set { _theHiveManager = value; }
        }
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


        public static async Task TheHiveCreateCase(Case theHiveNewCase)
        {
            await _theHiveManager.CreateCase(theHiveNewCase);
        }

        public static async Task TheHiveAddProcedures(List<Procedure> theHiveNewProcedures)
        {
            foreach (Procedure procedure in theHiveNewProcedures)
            {
                await _theHiveManager.AddProcedureToCase(procedure);
            }

        }
        [SupportedOSPlatform("windows")]
        public static async Task CheckRegistryIndicators(List<RegistryIndicator> lRegistryIndicator)
        {
            List<RegistryKeyResult> registryKeyResults = new();

            if (lRegistryIndicator != null)
            {

                foreach (RegistryIndicator registryIoC in lRegistryIndicator)
                {
                    foreach (RegistryKeyResult registryKeyResult in await CheckRegistryKey(registryIoC))
                    {
                        if (registryKeyResult is not null)
                        {
                            registryKeyResults.Add(registryKeyResult);

                            // Sends the result to TheHive
                            if (null != _theHiveManager)
                            {
                                registryIoC.Observable.DataType = "registry";
                                registryIoC.Observable.Data.Add(JsonSerializer.Serialize(registryKeyResult.RegistryItem));
                                await _theHiveManager.AddObservableToCase(registryIoC.Observable);
                            }
                            
                        }
                    }

                }

                OutputManager.WriteEvidenciesResult<RegistryKeyResult>(registryKeyResults, OutputManager.OUTPUT_MODE.TO_FILE, OutputManager.__REGISTRY_OUTPUT__);
            }
        }


        [SupportedOSPlatform("windows")]
        public static async Task CheckPipeIndicators(List<PipeIndicator> lPipeIndicators)
        {
            List<PipeResult> lPipeResults = new();

            if (lPipeIndicators != null)
            {
                foreach (PipeIndicator pipeIoC in lPipeIndicators)
                {

                    foreach (PipeResult pipeResult in await CheckPipe(pipeIoC))
                    {
                        lPipeResults.Add(pipeResult);
                    }

                }
                OutputManager.WriteEvidenciesResult<PipeResult>(lPipeResults, OutputManager.OUTPUT_MODE.TO_FILE, OutputManager.__PIPE_OUTPUT__);
            }

        }

        [SupportedOSPlatform("windows")]
        public static async Task CheckProcessIndicators(List<ProcessIndicator> lProcessIndicators)
        {
            List<ProcessResult> lProcessResults = new();

            if (lProcessIndicators != null)
            {
                foreach (ProcessIndicator processIoC in lProcessIndicators)
                {

                    foreach (ProcessResult processResult in await CheckProcess(processIoC))
                    {
                        lProcessResults.Add(processResult);
                    }

                }

                OutputManager.WriteEvidenciesResult<ProcessResult>(lProcessResults, OutputManager.OUTPUT_MODE.TO_FILE, OutputManager.__PROCESS_OUTPUT__);
            }
        }


        [SupportedOSPlatform("windows")]
        public static async Task CheckFileIndicators(List<FileIndicator> lFileIndicator)
        {
            List<FileResult> lfileResults = new();


            if (lFileIndicator != null)
            {
                foreach (FileIndicator fileIoC in lFileIndicator)
                {
                    foreach (FileResult fileResult in await CheckFile(fileIoC))
                    {
                        if (null != fileResult)
                        {
                            lfileResults.Add(fileResult);
                        }
                    }
                }

                OutputManager.WriteEvidenciesResult<FileResult>(lfileResults, OutputManager.OUTPUT_MODE.TO_FILE, OutputManager.__FILE_OUTPUT__);
            }
        }



        /// <summary>
        /// Checks the registry IoC
        /// </summary>
        /// <param name="OBJECT_RegistryIoC">The Registry IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        private static Task<List<RegistryKeyResult>> CheckRegistryKey(RegistryIndicator registryIoc)
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
        private static Task<List<FileResult>> CheckFile(FileIndicator fileIoc)
        {
            bool BOOL_TempResult;

            List<FileResult> lFileResult;
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
                        "yara" => YaraScanner.VerifyBinTxtYaraRule(STRING_FilePath: fileItem.STRING_Path, STRING_YaraRule: fileIoc.Rule).Count > 0,
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

                lFileResult.Add(new FileResult() { FileIndicator = fileIoc });

            }

            return Task.FromResult(lFileResult);
        }


        /// <summary>
        /// Checks the file IoC
        /// </summary>
        /// <param name="OBJECT_FileIoC">The Pipe IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        private static Task<List<PipeResult>> CheckPipe(PipeIndicator pipeIoC)
        {
            bool BOOL_TempResult;
            List<PipeResult> lPipeResult = new();
            List<string> lNamedPipe;


            lNamedPipe = Utils.GetSystemNamedPipes();

            if (null != lNamedPipe)
            {
                foreach (string namedPipe in lNamedPipe)
                {
                    BOOL_TempResult = pipeIoC.Type switch
                    {
                        "exists" => Regex.IsMatch(namedPipe, pipeIoC.Name),
                        _ => false
                    };

                    lPipeResult.Add(new PipeResult
                    {
                        Result = BOOL_TempResult,
                        Name = namedPipe,
                        PipeIndicator = pipeIoC

                    });

                }
            }
            else
            {
                lPipeResult.Add(new PipeResult { PipeIndicator = pipeIoC });
            }

            return Task.FromResult(lPipeResult);
        }

        /// <summary>
        /// Checks the process IoC
        /// </summary>
        /// <param name="OBJECT_ProcessIoC">The Process IoC object</param>
        /// <returns>True if the IoC exists, otherwise false</returns>
        private static Task<List<ProcessResult>> CheckProcess(ProcessIndicator processIoC)
        {
            bool BOOL_CheckResult = false;

            List<System.Diagnostics.Process> lProcesses;
            List<ProcessResult> lProcessResult = new();

            lProcesses = Helpers.Process.GetAccessibleProcesses(processIoC.Name);

            if (null != lProcesses && lProcesses.Count > 0)
            {
                foreach (System.Diagnostics.Process process in lProcesses)
                {

                    BOOL_CheckResult = processIoC.Type switch
                    {
                        "exists" => process.MainModule.ModuleName.Equals(processIoC.Name),
                        "hash" => string.Compare(Utils.CalculateSha256Hash(process.MainModule.FileName), processIoC.Sha256Hash, true) == 0,
                        "memory" => YaraScanner.VerifyProcessYaraRule(process, processIoC.Rule).Count > 0,
                        _ => false,
                    };

                    lProcessResult.Add(new ProcessResult
                    {
                        Result = BOOL_CheckResult,
                        Name = process.ProcessName,
                        ProcessIndicator = processIoC

                    });

                }

            }
            else
            {
                lProcessResult.Add(new ProcessResult { ProcessIndicator = processIoC });
            }


            return Task.FromResult(lProcessResult);
        }

        [Obsolete("WARNING: TO BE REWRITEN!!")]
        [SupportedOSPlatform("windows")]
        private static Task<bool> CheckEvent(dynamic OBJECT_EventIoC, ref List<EventLogEntry> lEventLogEntries)
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
