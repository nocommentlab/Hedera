using ncl.hedera.HederaLib;
using ncl.hedera.HederaLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;

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
                Console.WriteLine(string.Format("Author: {0}", OBJECT_Ioc["info"]["author"]));
                Console.WriteLine(string.Format("Date: {0}", OBJECT_Ioc["info"]["date"]));
                Console.WriteLine(string.Format("Description: {0}", ((string)OBJECT_Ioc["info"]["description"]).Replace("\\n", Environment.NewLine)));
                Console.WriteLine(string.Format("Registry IoC: {0}", OBJECT_Ioc["IoCs"]["registry"].Count));
                Console.WriteLine(string.Format("File IoC: {0}", OBJECT_Ioc["IoCs"]["file"].Count));
                Console.WriteLine(string.Format("Process IoC: {0}", OBJECT_Ioc["IoCs"]["process"].Count));
                Console.WriteLine("");
                BOOL_IsIocFileDescriptorValid = true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
                BOOL_IsIocFileDescriptorValid = false;
            }

            return BOOL_IsIocFileDescriptorValid;
        }
        private static async Task CheckRegistry(dynamic OBJECT_RegistryIoC)
        {
            foreach (dynamic registryIoC in OBJECT_RegistryIoC)
            {
                RegistryKeyResult registryKeyResult = await HederaLib.CheckRegistryKey(registryIoC);


                if (null != registryKeyResult && true == registryKeyResult.Result)
                {
                    switch (registryIoC["type"])
                    {
                        case "exists":
                            Console.WriteLine(String.Format("[*] - Detected IoC on registry!\n\tKey: {0},\n\tData Name: {1}[{2}],\n\tData Value: {3},\n\tType: {4}\n",
                                                    registryKeyResult.RegistryItem.STRING_Name,
                                                    registryKeyResult.RegistryItem.STRING_ValueName,
                                                    registryIoC["value_name_regex"],
                                                    registryKeyResult.RegistryItem.OBJECT_ValueData,
                                                    registryIoC["type"]
                                                    ));

                            break;
                        case "data_value_regex":
                            Console.WriteLine(String.Format("[*] - Detected IoC on registry!\n\tKey: {0},\n\tData Name: {1}[{2}],\n\tData Value: {3}[{4}],\n\tType: {5}\n",
                                                    registryKeyResult.RegistryItem.STRING_Name,
                                                    registryKeyResult.RegistryItem.STRING_ValueName,
                                                    registryIoC["value_name_regex"],
                                                    registryKeyResult.RegistryItem.OBJECT_ValueData,
                                                    registryIoC["value_data_regex"],
                                                    registryIoC["type"]
                                                    ));
                            break;
                    }

                }

            }

        }
        private static async Task CheckFile(dynamic OBJECT_FileIoC)
        {
            foreach (dynamic fileIoC in OBJECT_FileIoC)
            {
                List<FileResult> fileResults = await HederaLib.CheckFile(fileIoC);
                if (null != fileResults && true == fileResults.Count > 0)
                {
                    foreach (FileResult fileResult in fileResults)
                    {
                        switch (fileIoC["type"])
                        {
                            case "exists":

                                Console.WriteLine(String.Format("[*] - Detected IoC on file!\n\tPath: {0}, \n\tType: {1}\n",
                                                        fileResult.FileItem.STRING_Path,
                                                        fileIoC["type"]));
                                break;
                            case "hash":
                                Console.WriteLine(String.Format("[*] - Detected IoC on file!\n\tPath: {0}, \n\tType: {1},\n\tSHA256 HASH: {2}\n",
                                                        fileResult.FileItem.STRING_Path,
                                                        fileIoC["type"],
                                                        fileIoC["sha256_hash"]));
                                break;
                            case "imphash":
                                Console.WriteLine(String.Format("[*] - Detected IoC on file!\n\tPath: {0}, \n\tType: {1},\n\tIMPHASH: {2}\n",
                                                        fileResult.FileItem.STRING_Path,
                                                        fileIoC["type"],
                                                        fileIoC["value"]));
                                break;
                            case "yara":
                                Console.WriteLine(String.Format("[*] - Detected IoC on file!\n\tPath: {0}, \n\tType: {1}\n",
                                                        fileResult.FileItem.STRING_Path,
                                                        fileIoC["type"]));
                                break;
                            case "exists_regex":
                                Console.WriteLine(String.Format("[*] - Detected IoC on file!\n\tPath: {0}, \n\tType: {1},\n\tregex: {2}\n",
                                                        fileResult.FileItem.STRING_Path,
                                                        fileIoC["type"],
                                                        fileIoC["regex"]));
                                break;
                            default:
                                break;
                        }

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
                    switch (processIoC["type"])
                    {
                        case "exists":
                            Console.WriteLine(String.Format("[*] - Detected IoC on process!\n\tName: {0}, \n\tType: {1}\n",
                                                    processIoC["name"],
                                                    processIoC["type"]));
                            break;
                        case "hash":
                            Console.WriteLine(String.Format("[*] - Detected IoC on process!\n\tName: {0}, \n\tType: {1},\n\tSHA256 HASH: {2}\n",
                                                    processIoC["name"],
                                                    processIoC["type"],
                                                    processIoC["sha256_hash"]));
                            break;
                    }
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
                lEventLogEntries.ForEach(eventLogEntry =>
                {
                    Console.WriteLine($"[*] - Detected IoC on event!\n\tData: {eventLogEntry.TimeGenerated}, \n\tType: {eventLogEntry.EntryType},\n\tMessage: {eventLogEntry.Message}\n");
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

                Console.WriteLine("[*] - Scan Finished!");
            }

            Console.Read();
        }
        #endregion

    }
}
