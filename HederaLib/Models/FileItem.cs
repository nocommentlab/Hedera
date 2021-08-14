using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Models
{
    public class FileItem
    {
        public string STRING_Path { get; set; }
        public FileAttributes FILEATTRIBUTES_Attributes { get; set; }
        public DateTime DATETIME_UTCCreationTime { get; set; }
        public FileSecurity FILESECURITY_ACL { get; set; }


    }
}
