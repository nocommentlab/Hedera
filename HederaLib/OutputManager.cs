using ncl.hedera.HederaLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib
{
    /// <summary>
    /// This class manages the output result channel
    /// </summary>
    public class OutputManager
    {
        #region Costants
        public const string __REGISTRY_OUTPUT__ = "registry_evidences.json";
        public const string __FILE_OUTPUT__ = "file_evidences.json";
        public const string __PIPE_OUTPUT__ = "pipe_evidences.json";
        #endregion

        public enum OUTPUT_MODE
        {
            TO_STDOUT,
            TO_FILE,
            TO_TCP // YES, THIS IS A NEW FEATURE :)
        }
        
        public OUTPUT_MODE outputMode = OUTPUT_MODE.TO_STDOUT;


        public static void WriteEvidenciesResult<T>(List<T> lResult, 
                                                    OUTPUT_MODE outputMode, 
                                                    string STRING_Path = null)
        {

            switch (outputMode)
            {
                case OUTPUT_MODE.TO_STDOUT:
                    break;
                case OUTPUT_MODE.TO_FILE:
                    if (STRING_Path is not null)
                        File.WriteAllText(STRING_Path, JsonSerializer.Serialize(lResult));

                    else
                        throw new ArgumentNullException(STRING_Path);
                    break;

                case OUTPUT_MODE.TO_TCP:
                    break;
                default:
                    break;
            }
        }

        


    }
}
