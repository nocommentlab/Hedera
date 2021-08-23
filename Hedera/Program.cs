using ncl.hedera.HederaLib;
using ncl.hedera.HederaLib.Models;
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
        private static HederaLib _hederaLib = null;
        #endregion

        #region Properties

        #endregion

        #region Private Functions
        /// <summary>
        /// Validate the IoC configuration file
        /// </summary>
        /// <param name="OBJECT_Ioc">The deserialized Yaml File</param>
        /// <returns>True if the yaml file is correct validated, otherwise false</returns>
        private static bool ValidateAndPrintIoCSummary(dynamic OBJECT_Ioc)
        {
            bool BOOL_IsIocFileDescriptorValid;
            try
            {

                //WriteLine(string.Format("Author: {0}", OBJECT_Ioc["info"]["author"]));
                WriteLine($"Description: {OBJECT_Ioc["info"]["description"]}".Info());
                WriteLine($"Author: {OBJECT_Ioc["info"]["author"]}".Info());
                WriteLine($"Date: {OBJECT_Ioc["info"]["date"]}".Info());
                WriteLine($"Modified: {OBJECT_Ioc["info"]["modified"]}".Info());
                WriteLine($"RoleId: {OBJECT_Ioc["info"]["id"]}".Info());
                WriteLine($"Status: {OBJECT_Ioc["info"]["status"]}".Info());
                WriteLine($"Registry IoC: { OBJECT_Ioc["IoCs"]["registry"].Count}".Info());
                WriteLine($"File IoC: {OBJECT_Ioc["IoCs"]["file"].Count}".Info());
                WriteLine($"Process IoC: {OBJECT_Ioc["IoCs"]["process"].Count}".Info());
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
        private static async Task CheckRegistry(dynamic OBJECT_RegistryIoC)
        {
            foreach (dynamic registryIoC in OBJECT_RegistryIoC)
            {
                RegistryKeyResult registryKeyResult = await HederaLib.CheckRegistryKey(registryIoC);


                if ((registryKeyResult is not null) && (registryKeyResult.Result))
                {
                    string STRING_OutputTrace = (string)registryIoC["type"] switch
                    {
                        "exists" => $"Detected IoC on registry!\n\tKey: {registryKeyResult.RegistryItem.STRING_Name}," +
                                              $"\n\tData Name: {registryKeyResult.RegistryItem.STRING_ValueName}[{ registryIoC["value_name_regex"]}]," +
                                              $"\n\tData Value: {registryKeyResult.RegistryItem.OBJECT_ValueData}," +
                                              $"\n\tType: {registryIoC["type"]}\n",

                        "data_value_regex" => $"Detected IoC on registry!\n\tKey: {registryKeyResult.RegistryItem.STRING_Name}," +
                                              $"\n\tData Name: {registryKeyResult.RegistryItem.STRING_ValueName}[{registryIoC["value_name_regex"]}]," +
                                              $"\n\tData Value: {registryKeyResult.RegistryItem.OBJECT_ValueData}[{registryIoC["value_data_regex"]}]," +
                                              $"\n\tType: {registryIoC["type"]}\n",
                        _ => throw new NotImplementedException()
                    };

                    WriteLine(STRING_OutputTrace.Warning());

                }

            }

        }
        private static async Task CheckFile(dynamic OBJECT_FileIoC)
        {
            foreach (dynamic fileIoC in OBJECT_FileIoC)
            {
                List<FileResult> fileResults = await HederaLib.CheckFile(fileIoC);

                if (fileResults is not null && fileResults.Count > 0)
                {
                    foreach (FileResult fileResult in fileResults)
                    {
                        string STRING_OutputTrace = (string)fileIoC["type"] switch
                        {
                            "exists" => $"Detected IoC on file!\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                      $"\n\tType: {fileIoC["type"]}\n",

                            "hash" => $"Detected IoC on file!\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                    $"\n\tType: {fileIoC["type"]}," +
                                    $"\n\tSHA256 HASH: {fileIoC["sha256_hash"]}\n",

                            "imphash" => $"Detected IoC on file!\n\tPath: {fileResult.FileItem.STRING_Path}," +
                                        $"\n\tType: {fileIoC["type"]}," +
                                        $"\n\tIMPHASH: {fileIoC["value"]}\n",

                            "yara" => $"Detected IoC on file!\n\tPath: { fileResult.FileItem.STRING_Path}," +
                                     $"\n\tType: { fileIoC["type"]}\n",

                            "exists_regex" => $"Detected IoC on file!\n\tPath: { fileResult.FileItem.STRING_Path}," +
                                             $"\n\tType: { fileIoC["type"]}," +
                                             $"\n\tregex: { fileIoC["regex"]}\n",
                            _ => throw new NotImplementedException()
                        };

                        WriteLine(STRING_OutputTrace.Warning());

                    }
                }
            }
        }
        private static async Task CheckProcess(dynamic OBJECT_ProcessIoC)
        {
            foreach (dynamic processIoC in OBJECT_ProcessIoC)
            {
                bool? BOOL_Result = await HederaLib.CheckProcess(processIoC);
                if (true == BOOL_Result)
                {
                    string STRING_OutputTrace = (string)processIoC["type"] switch
                    {
                        "exists" => $"Detected IoC on process!\n\tName: {processIoC["name"]}," +
                                   $"\n\tType: {processIoC["type"]}\n",
                        "hash" => $"Detected IoC on process!\n\tName: {processIoC["name"]}," +
                                   $"\n\tType: {processIoC["type"]}," +
                                   $"\n\tSHA256 HASH: {processIoC["sha256_hash"]}\n",
                        _ => throw new NotImplementedException()
                    };

                    WriteLine(STRING_OutputTrace.Warning());
                }
            }
        }
        [SupportedOSPlatform("windows")]
        private static async Task CheckEvent(dynamic OBJECT_EventIoC)
        {
            List<EventLogEntry> lEventLogEntries = null;
            foreach (dynamic eventIoC in OBJECT_EventIoC)
            {
                bool? BOOL_Result = await HederaLib.CheckEvent(eventIoC, ref lEventLogEntries);
                lEventLogEntries?.ForEach(eventLogEntry =>
                {
                    WriteLine(($"Detected IoC on event!\n\tData: {eventLogEntry.TimeGenerated}," +
                              $"\n\tType: {eventLogEntry.EntryType}," +
                              $"\n\tMessage: {eventLogEntry.Message}\n").Warning());
                }
                );
            }
            lEventLogEntries.Clear();
            lEventLogEntries = null;
            GC.Collect();
        }
        #endregion

        #region Public Functions
        private static void Main(string[] args)
        {

            // Loads the explicit IoC, otherwise, loads the default 
            _STRING_IocFile = (args.Length == 0) ? DEFAULT_CONFIG_FILE : args[0];

            // Deserialize the ioc configuration file
            _hederaLib = new HederaLib(_STRING_IocFile);
            var deserializedYaml = _hederaLib.DeserializeIoCConfiguration();

            // Checks if the file has the correct structure and print its summary
            if (ValidateAndPrintIoCSummary(deserializedYaml))
            {

                var CheckEventTask = Task.Factory.StartNew(() => { if (((Dictionary<Object, Object>)deserializedYaml["IoCs"]).ContainsKey(@"event")) CheckEvent(deserializedYaml["IoCs"][@"event"]); });
                var CheckFileTask = Task.Factory.StartNew(() => { if (((Dictionary<Object, Object>)deserializedYaml["IoCs"]).ContainsKey(@"file")) CheckFile(deserializedYaml["IoCs"][@"file"]); });
                var CheckProcessTask = Task.Factory.StartNew(() => { if (((Dictionary<Object, Object>)deserializedYaml["IoCs"]).ContainsKey(@"process")) CheckProcess(deserializedYaml["IoCs"][@"process"]); });
                var CheckRegistryTask = Task.Factory.StartNew(() => { if (((Dictionary<Object, Object>)deserializedYaml["IoCs"]).ContainsKey(@"registry")) CheckRegistry(deserializedYaml["IoCs"][@"registry"]); });

                Task.WaitAll(CheckEventTask, CheckFileTask, CheckProcessTask, CheckRegistryTask);

                WriteLine("Scan Finished!".Info());
            }

            Console.Read();
        }
        #endregion

    }
}
