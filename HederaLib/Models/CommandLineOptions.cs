using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public sealed class CommandLineOptions
    {

        [Option("idbf", HelpText = "The Hedera IDBF file path", Required = true)]
        public string Idbf { get; set; }

        [Option('i', "ip", HelpText = "The TheHive ip address instance")]
        public string Ip { get; set; }

        [Option('p', "port", HelpText = "The TheHive port instance", Default = 9000)]
        public int Port { get; set; }

        [Option('a', "apikey", HelpText = "The TheHive hedera-scanner ApiKey")]
        public string ApiKey { get; set; }
    }
}
