using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models.TheHive
{
    public class Observable
    {
        public string DataType { get; set; }
        public int Tlp { get; set; }
        public bool Ioc { get; set; }
        public bool Sighted { get; set; }
        public string[] Tags { get; set; }
        public string[] Data { get; set; }
        public string Message { get; set; }

        public Observable()
        {
            Tags = Array.Empty<string>();
            Data = Array.Empty<string>();
        }
    }
}
