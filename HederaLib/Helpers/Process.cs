using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Helpers
{
    class Process : System.Diagnostics.Process
    {

        #region Constants
        #endregion

        #region Members
        #endregion

        #region Properties
        #endregion

        #region Public Functions
        /// <summary>
        /// Gets the list processes that are accessibile using the user that runs the software
        /// </summary>
        /// <returns></returns>
        public static List<System.Diagnostics.Process> GetAccessibleProcesses(string STRING_ProcessName)
        {

            List<System.Diagnostics.Process> lProcesses = new();
            foreach (System.Diagnostics.Process process in GetProcesses())
            {
                try
                {
                    if (process.MainModule.ModuleName != null)
                    {
                        lProcesses.Add(process);
                    }
                }
                catch (Exception) { }

            }
            return lProcesses.Where(process => Regex.IsMatch(process.MainModule?.ModuleName, STRING_ProcessName, RegexOptions.IgnoreCase)).ToList();
            
        }

        /// <summary>
        /// Retrieves the list of processes that have a specific executable name
        /// </summary>
        /// <param name="STRING_ExecutableName">The specific executable name</param>
        /// <returns>The list of processes found</returns>
        public static System.Diagnostics.Process[] GetProcessByExecutableName(string STRING_ExecutableName)
        {
            List<System.Diagnostics.Process> lProcesses = new();
            foreach (System.Diagnostics.Process process in GetProcesses())
            {
                try
                {
                    if (process.MainModule.ModuleName.Equals(STRING_ExecutableName))
                    {
                        lProcesses.Add(process);
                    }
                }
                catch (Exception) { }

            }

            return lProcesses.ToArray<System.Diagnostics.Process>();
        }
        #endregion

        #region Private Functions
        #endregion

    }
}