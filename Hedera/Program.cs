using ncl.hedera.HederaLib;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using static System.Console;


namespace Hedera
{
    class Program
    {
        #region Costants
        private const string DEFAULT_CONFIG_FILE = @"ioc.yaml";
        #endregion

        #region Members
        private static string _STRING_IocFile = String.Empty;
        #endregion

        #region Properties

        #endregion

        #region Private Functions
        /// <summary>
        /// Validate the IoC configuration file
        /// </summary>
        /// <param name="OBJECT_Ioc">The deserialized Yaml File</param>
        /// <returns>True if the yaml file is correct validated, otherwise false</returns>
        private static bool ValidateAndPrintIoCSummary(Config config)
        {
            bool BOOL_IsIocFileDescriptorValid;
            try
            {
                WriteLine($"Description: {config.Information.Description}".Info());
                WriteLine($"Author: {config.Information.Author}".Info());
                WriteLine($"Date: {config.Information.Date}".Info());
                WriteLine($"Modified: {config.Information.Modified}".Info());
                WriteLine($"GUID: {config.Information.Guid}".Info());
                WriteLine($"Status: {config.Information.Status}".Info());
                WriteLine($"Registry IoC: { config.Indicators.Registry?.Count}".Info());
                WriteLine($"File IoC: {config.Indicators.File?.Count}".Info());
                WriteLine($"Process IoC: {config.Indicators.Process?.Count }".Info());
                WriteLine($"Event IoC: {config.Indicators.Event?.Count}".Info());
                WriteLine($"Pipe IoC: {config.Indicators.Pipe?.Count}".Info());
                WriteLine("");

                BOOL_IsIocFileDescriptorValid = true;
            }
            catch (KeyNotFoundException e)
            {
                WriteLine(e.Message.Error());
                BOOL_IsIocFileDescriptorValid = false;
            }

            return BOOL_IsIocFileDescriptorValid;
        }

        [SupportedOSPlatform("windows")]
        private static async Task CheckRegistry(List<RegistryIndicator> lRegistryIndicator)
        {
            foreach (RegistryIndicator registryIoC in lRegistryIndicator)
            {
                List<RegistryKeyResult> lRegistryKeyResult = await HederaLib.CheckRegistryKey(registryIoC);

                foreach (RegistryKeyResult registryKeyResult in lRegistryKeyResult)
                {
                    if ((registryKeyResult is not null) && (registryKeyResult.Result))
                    {
                        string STRING_OutputTrace = registryIoC.Type switch
                        {
                            "exists" => $"Detected IoC on registry!\n\tGUID: {registryIoC.Guid}" +
                                                  $"\n\tKey: {registryKeyResult.RegistryItem.STRING_Name}," +
                                                  $"\n\tData Name: {registryKeyResult.RegistryItem.STRING_ValueName}[{ registryIoC.ValueNameRegex}]," +
                                                  $"\n\tData Value: {registryKeyResult.RegistryItem.OBJECT_ValueData}," +
                                                  $"\n\tType: {registryIoC.Type}\n",

                            "data_value_regex" => $"Detected IoC on registry!\n\tGUID: {registryIoC.Guid}" +
                                                  $"\n\tKey: {registryKeyResult.RegistryItem.STRING_Name}," +
                                                  $"\n\tData Name: {registryKeyResult.RegistryItem.STRING_ValueName}[{registryIoC.ValueNameRegex}]," +
                                                  $"\n\tData Value: {registryKeyResult.RegistryItem.OBJECT_ValueData}[{registryIoC.ValueDataRegex}]," +
                                                  $"\n\tType: {registryIoC.Type}\n",

                            _ => throw new NotImplementedException()
                        };

                        WriteLine(STRING_OutputTrace.Warning());

                    }
                }
            }

        }
        
        [SupportedOSPlatform("windows")]
        private static async Task CheckFile(List<FileIndicator> lFileIndicators)
        {
            foreach (FileIndicator fileIoC in lFileIndicators)
            {
                List<FileResult> fileResults = await HederaLib.CheckFile(fileIoC);

                if (fileResults is not null && fileResults.Count > 0)
                {
                    foreach (FileResult fileResult in fileResults)
                    {
                        string STRING_OutputTrace = fileIoC.Type switch
                        {
                            "exists" => $"Detected IoC on file!\n\tGUID: {fileIoC.Guid}"+
                                      $"\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                      $"\n\tType: {fileIoC.Type}\n",

                            "hash" => $"Detected IoC on file!\n\tGUID: {fileIoC.Guid}" +
                                    $"\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                    $"\n\tType: {fileIoC.Type}," +
                                    $"\n\tSHA256 HASH: {fileIoC.Sha256Hash}\n",

                            "imphash" => $"Detected IoC on file!\n\tGUID: {fileIoC.Guid}" +
                                        $"\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                        $"\n\tType: {fileIoC.Type}," +
                                        $"\n\tIMPHASH: {fileIoC.Value}\n",

                            "yara" => $"Detected IoC on file!\n\tGUID: {fileIoC.Guid}" +
                                     $"\n\tPath: { fileResult.FileItem.STRING_Path}," +
                                     $"\n\tType: { fileIoC.Type}\n",

                            "exists_regex" => $"Detected IoC on file!\n\tGUID: {fileIoC.Guid}" +
                                             $"\n\tPath: { fileResult.FileItem.STRING_Path}," +
                                             $"\n\tType: { fileIoC.Type}," +
                                             $"\n\tregex: { fileIoC.Filename}\n",
                            _ => throw new NotImplementedException()
                        };

                        WriteLine(STRING_OutputTrace.Warning());

                    }
                }
            }
        }
        
        private static async Task CheckProcess(List<ProcessIndicator> lProcessIndicators)
        {
            foreach (ProcessIndicator processIoC in lProcessIndicators)
            {
                bool? BOOL_Result = await HederaLib.CheckProcess(processIoC);
                if (true == BOOL_Result)
                {
                    string STRING_OutputTrace = processIoC.Type switch
                    {
                        "exists" => $"Detected IoC on process!\n\tGUID: {processIoC.Guid}" +
                                    $"\n\tName: {processIoC.Name}," +
                                    $"\n\tType: {processIoC.Type}\n",
                        "hash" => $"Detected IoC on process!\n\tGUID: {processIoC.Guid}" +
                                  $"\n\tName: {processIoC.Name}," +
                                  $"\n\tType: {processIoC.Type}," +
                                  $"\n\tSHA256 HASH: {processIoC.Sha256Hash}\n",
                        _ => throw new NotImplementedException()
                    };

                    WriteLine(STRING_OutputTrace.Warning());
                }
            }
        }
        
        [SupportedOSPlatform("windows")]
        private static async Task CheckEvent(List<EventIndicator> lEventIndicators)
        {
            List<EventLogEntry> lEventLogEntries = null;
            foreach (EventIndicator eventIoC in lEventIndicators)
            {
                bool? BOOL_Result = await HederaLib.CheckEvent(eventIoC, ref lEventLogEntries);
                lEventLogEntries?.ForEach(eventLogEntry =>
                {
                    WriteLine(($"Detected IoC on event!\n\tGUID: {eventIoC.Guid}" +
                              $"\n\tData: {eventLogEntry.TimeGenerated}," +
                              $"\n\tType: {eventLogEntry.EntryType}," +
                              $"\n\tMessage: {eventLogEntry.Message}\n").Warning());
                }
                );
            }
            lEventLogEntries.Clear();
            lEventLogEntries = null;
            GC.Collect();
        }

        [SupportedOSPlatform("windows")]
        private static async Task CheckPipe(List<PipeIndicator> lPipeIndicators)
        {
            foreach (PipeIndicator pipeIoC in lPipeIndicators)
            {
                List<PipeResult> pipeResults = await HederaLib.CheckPipe(pipeIoC);

                if (pipeResults is not null && pipeResults.Count > 0)
                {
                    foreach (PipeResult pipeResult in pipeResults)
                    {
                        string STRING_OutputTrace = pipeIoC.Type switch
                        {
                            "exists" => $"Detected IoC on pipe!\n\tGUID: {pipeIoC.Guid}" +
                                      $"\n\tPath: {pipeResult.Name}," +
                                      $"\n\tType: {pipeIoC.Type}\n",

                 
                            _ => throw new NotImplementedException()
                        };

                        WriteLine(STRING_OutputTrace.Warning());

                    }
                }
            }
        }

        #endregion

        #region Public Functions
        private static void Main(string[] args)
        {

            // Loads the explicit IoC, otherwise, loads the default 
            _STRING_IocFile = (args.Length == 0) ? DEFAULT_CONFIG_FILE : args[0];

            // Deserialize the ioc configuration file
            Config deserializedYaml = HederaLib.DeserializeIoCConfiguration(_STRING_IocFile);

            // Checks if the file has the correct structure and print its summary
            if (ValidateAndPrintIoCSummary(deserializedYaml))
            {

                var CheckEventTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.Event != null) CheckEvent(deserializedYaml.Indicators.Event); });
                var CheckFileTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.File != null) CheckFile(deserializedYaml.Indicators.File); });
                var CheckProcessTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.Process != null) CheckProcess(deserializedYaml.Indicators.Process); });
                var CheckRegistryTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.Registry != null) CheckRegistry(deserializedYaml.Indicators.Registry); });
                var CheckPipeTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.Pipe != null) CheckPipe(deserializedYaml.Indicators.Pipe); });

                Task.WaitAll(CheckEventTask, CheckFileTask, CheckProcessTask, CheckRegistryTask, CheckPipeTask);



                WriteLine("Scan Finished!".Info());
            }

            Read();
        }
        #endregion

    }
}
