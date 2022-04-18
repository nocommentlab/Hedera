using CommandLine;
using ncl.hedera.HederaLib;
using ncl.hedera.HederaLib.Controllers;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
using System;
using System.Collections;
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

        #endregion

        #region Members
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

        private static async Task Main(string[] args)
        {

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(async (opts) => await CommandLineParsingOkCallback(opts))
                .WithNotParsed((errs) => CommandLineParsingNokCallback(errs));

        }
        #endregion

        #region Private Functions
        private static async Task CommandLineParsingOkCallback(CommandLineOptions opts)
        {
            // Deserialize the ioc configuration file
            Config deserializedYaml = HederaLib.DeserializeIoCConfiguration(opts.Idbf);

            // Checks if the TheHive connection property is correctly configured
            if (opts.Ip != null && opts.ApiKey != null)
            {
                HederaLib.TheHiveManager = new TheHiveManager(opts.Ip, opts.Port, opts.ApiKey);
                await HederaLib.TheHiveCreateCase(deserializedYaml.TheHive.Case);
                await HederaLib.TheHiveAddProcedures(deserializedYaml.TheHive.Procedures);
            }

            // Checks if the file has the correct structure and print its summary
            if (ValidateAndPrintIoCSummary(deserializedYaml))
            {

                Task.WaitAll(Task.Factory.StartNew(function: async () => await HederaLib.CheckFileIndicators(deserializedYaml.Indicators.File)),
                             Task.Factory.StartNew(function: async () => await  HederaLib.CheckProcessIndicators(deserializedYaml.Indicators.Process)),
                             Task.Factory.StartNew(function: async () => await HederaLib.CheckRegistryIndicators(deserializedYaml.Indicators.Registry)),
                             Task.Factory.StartNew(function: async () => await HederaLib.CheckPipeIndicators(deserializedYaml.Indicators.Pipe)));



                WriteLine("Scan Finished!".Info());
            }

            Read();

        }

        private static void CommandLineParsingNokCallback(IEnumerable errs)
        {
            Console.WriteLine("Command Line parameters provided were not valid!");
        }
        #endregion

    }
}
