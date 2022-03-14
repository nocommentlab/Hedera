using ncl.hedera.HederaLib;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
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

        [Obsolete("WARNING: TO BE REWRITEN!!")]
        [SupportedOSPlatform("windows")]
        private static async Task CheckEvent(List<EventIndicator> lEventIndicators)
        {
            List<EventLogEntry> lEventLogEntries = null;
            foreach (EventIndicator eventIoC in lEventIndicators)
            {
                //bool? BOOL_Result = await HederaLib.CheckEvent(eventIoC, ref lEventLogEntries);
                lEventLogEntries?.ForEach(eventLogEntry =>
                {
                    WriteLine(($"Detected IoC on event!\n\tGUID: {eventIoC.Guid}" +
                              $"\n\tData: {eventLogEntry.TimeGenerated}," +
                              $"\n\tType: {eventLogEntry.EntryType}," +
                              $"\n\tMessage: {eventLogEntry.Message}\n").Match());
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
            Config deserializedYaml = HederaLib.DeserializeIoCConfiguration(_STRING_IocFile);

            // Checks if the file has the correct structure and print its summary
            if (ValidateAndPrintIoCSummary(deserializedYaml))
            {
                var CheckRegistryTask = Task.Factory.StartNew(async () => { if (deserializedYaml.Indicators.Registry != null) await HederaLib.CheckRegistryIndicators(deserializedYaml.Indicators.Registry); });
                var CheckFileTask = Task.Factory.StartNew(async () => { if (deserializedYaml.Indicators.File != null) await HederaLib.CheckFileIndicators(deserializedYaml.Indicators.File); });
                var CheckPipeTask = Task.Factory.StartNew(async () => { if (deserializedYaml.Indicators.Pipe != null) await HederaLib.CheckPipeIndicators(deserializedYaml.Indicators.Pipe); });
                var CheckProcessTask = Task.Factory.StartNew(async () => { if (deserializedYaml.Indicators.Process != null) await HederaLib.CheckProcessIndicators(deserializedYaml.Indicators.Process); });
                //var CheckEventTask = Task.Factory.StartNew(() => { if (deserializedYaml.Indicators.Event != null) CheckEvent(deserializedYaml.Indicators.Event); });


                Task.WaitAll(CheckFileTask, CheckProcessTask, CheckRegistryTask, CheckPipeTask);


                
                WriteLine("Scan Finished!".Info());
            }

            Read();
        }
        #endregion

    }
}
