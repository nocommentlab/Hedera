using Microsoft.Win32;
using ncl.hedera.HederaLib.Models;
using PeNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Helpers
{
    class Utils
    {
        #region Constants
        #endregion

        #region Members
        #endregion

        #region Properties
        #endregion

        #region Public Functions
        /// <summary>
        /// Calcualtes the SHA256 Hash of given file
        /// </summary>
        /// <param name="STRING_FilePath"></param>
        /// <returns>The SHA256 Hash</returns>
        public static string CalculateSha256Hash(string STRING_FilePath)
        {
            string STRING_Sha256Hash = String.Empty;
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hashValue = mySHA256.ComputeHash(File.ReadAllBytes(STRING_FilePath));
                for (int i = 0; i < hashValue.Length; i++)
                {
                    STRING_Sha256Hash += $"{hashValue[i]:X2}";
                }
            }

            return STRING_Sha256Hash.ToLower();
        }

        public static string CalculateImphash(string STRING_FilePath)
        {

            PeFile peFile = new(STRING_FilePath);
            return peFile.ImpHash.ToLower();
        }
        /// <summary>
        /// Retrieves the 
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static string GetCurrentUserSID()
        {
            NTAccount ntAccount = new(WindowsIdentity.GetCurrent().Name);
            SecurityIdentifier securityIdentifier = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
            return securityIdentifier.ToString();
        }

        /// <summary>
        /// Replace the templates
        /// </summary>
        /// <param name="STRING_TextWithTemplate">The string that contains templates</param>
        /// <returns>The string with the replaced templates</returns>
        [SupportedOSPlatform("windows")]
        public static string ReplaceTemplate(string STRING_TextWithTemplate)
        {
            /* Replace the template specified in the IoC file*/
            if (STRING_TextWithTemplate.Contains("{{sid}}"))
            {
                STRING_TextWithTemplate = STRING_TextWithTemplate.Replace("{{sid}}", GetCurrentUserSID());
            }

            if (STRING_TextWithTemplate.Contains("{{user}}"))
            {
                STRING_TextWithTemplate = STRING_TextWithTemplate.Replace("{{user}}", WindowsIdentity.GetCurrent().Name.Split('\\')[1]);
            }

            return STRING_TextWithTemplate;
        }

        /// <summary>
        /// Reads the registry data value of given registry key. It supports recursive mode
        /// </summary>
        /// <param name="OBJECT_RegistryIoC">The dynamic RegistryIoC object reads from yaml file</param>
        /// <returns>The registry data value, otherwise null</returns>
        [SupportedOSPlatform("windows")]
        public static RegistryItem ReadRegistryDataValue(dynamic OBJECT_RegistryIoC)
        {
            RegistryItem registryItem = null;

            RegistryKey REGISTRY_RegistryKey;

            RegistryKey REGISTRYKEY_BaseRegistry = SetBaseRegistryKey(OBJECT_RegistryIoC["base_key"]);

            if (null != REGISTRYKEY_BaseRegistry)
            {
                REGISTRY_RegistryKey = REGISTRYKEY_BaseRegistry.OpenSubKey(OBJECT_RegistryIoC["key"]);
                if (null != REGISTRY_RegistryKey)
                {
                    if (bool.Parse(OBJECT_RegistryIoC["is_recursive"]))
                        SearchSubKeys(REGISTRY_RegistryKey, OBJECT_RegistryIoC["value_name_regex"], ref registryItem);
                    else
                    {
                        string STRING_ValueName = REGISTRY_RegistryKey.GetValueNames().ToList<string>()
                                                .FirstOrDefault(valueName => Regex.IsMatch(valueName, OBJECT_RegistryIoC["value_name_regex"]));
                        /*List<string> test = REGISTRY_RegistryKey.GetValueNames().ToList<string>()
                                                .Where(valueName => Regex.IsMatch(valueName, OBJECT_RegistryIoC["value_name_regex"])).ToList();*/
                        if (STRING_ValueName != null)
                        {
                            registryItem = new RegistryItem
                            {
                                STRING_Name = REGISTRY_RegistryKey.Name,

                                STRING_ValueName = STRING_ValueName,
                                OBJECT_ValueData = REGISTRY_RegistryKey.GetValue(STRING_ValueName)
                            };
                        }

                    }

                }
            }

            return registryItem;
        }

        /// <summary>
        /// Converts the string registry base key read from yaml file to a RegistryKey object
        /// </summary>
        /// <param name="STRING_Basekey">The base registry key</param>
        /// <returns>The corrisponded RegistryKey object</returns>
        [SupportedOSPlatform("windows")]
        public static RegistryKey SetBaseRegistryKey(string STRING_Basekey)
        {
            RegistryKey REGISTRYKEY_SelectedKey = STRING_Basekey switch
            {
                "HKEY_USERS" => Registry.Users,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_PERFORMANCE_DATA" => Registry.PerformanceData,
                _ => null,
            };
            return REGISTRYKEY_SelectedKey;
        }

        /// <summary>
        /// Checks if a file exists. It supports recursive mode.
        /// </summary>
        /// <param name="OBJECT_FileIoC">The dynamic FileIoC object reads from yaml file</param>
        /// <returns>True if exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static FileItem IsFileExists(dynamic OBJECT_FileIoC)
        {

            string STRING_Filename = null;

            FileItem fileItem = null;

            List<string> LIST_AccessibleFiles = new();

            if (false != bool.Parse(OBJECT_FileIoC["is_recursive"]))
            {
                GetAllAccessibileFiles(OBJECT_FileIoC["path"], LIST_AccessibleFiles);
                STRING_Filename = LIST_AccessibleFiles.Where(accessibleFile => Regex.IsMatch(Path.GetFileName(accessibleFile), OBJECT_FileIoC["filename"], RegexOptions.IgnoreCase)).FirstOrDefault();

                /* Free the memory */
                LIST_AccessibleFiles.Clear();
                LIST_AccessibleFiles = null;
                GC.Collect();
            }
            else
            {
                STRING_Filename = File.Exists(OBJECT_FileIoC["path"]) ? OBJECT_FileIoC["path"] : null;
            }

            if (null != STRING_Filename)
            {
                fileItem = new FileItem
                {
                    STRING_Path = STRING_Filename,
                    FILEATTRIBUTES_Attributes = File.GetAttributes(OBJECT_FileIoC["path"]),
                    DATETIME_UTCCreationTime = File.GetCreationTimeUtc(OBJECT_FileIoC["path"]),
                    FILESECURITY_ACL = null
                };
            }
            return fileItem;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Checks if a subkey exists
        /// </summary>
        /// <param name="REGISTRYKEY_RootKey">The root key</param>
        /// <param name="STRING_SearchedValue">The searched key value</param>
        /// <param name="registryItem">The result of search: true if exists, otherwise false</param>
        [SupportedOSPlatform("windows")]
        private static void SearchSubKeys(RegistryKey REGISTRYKEY_RootKey, String STRING_SearchedValue, ref RegistryItem registryItem)
        {
            string[] vSTRING_Subkeys = REGISTRYKEY_RootKey.GetSubKeyNames();
            int INT32_Idx = 0;
            while (INT32_Idx < vSTRING_Subkeys.Length && registryItem == null)
            {
                try
                {
                    using (RegistryKey REGISTRYKEY_Key = REGISTRYKEY_RootKey.OpenSubKey(vSTRING_Subkeys[INT32_Idx]))
                    {
                        try
                        {

                            if (REGISTRYKEY_Key.GetValueNames().Where(valueName => Regex.IsMatch(valueName, STRING_SearchedValue)).Any())
                            {
                                registryItem = new RegistryItem
                                {
                                    STRING_Name = REGISTRYKEY_Key.Name,
                                    STRING_ValueName = REGISTRYKEY_Key.GetValueNames().Where(valueName => Regex.IsMatch(valueName, STRING_SearchedValue)).First()
                                };
                                registryItem.OBJECT_ValueData = REGISTRYKEY_Key.GetValue(registryItem.STRING_ValueName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        SearchSubKeys(REGISTRYKEY_Key, STRING_SearchedValue, ref registryItem);
                    }
                }
                catch (System.Security.SecurityException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                INT32_Idx++;
            }

        }

        /// <summary>
        /// Gets all accessible files in recursive mode
        /// </summary>
        /// <param name="STRING_BasePath">The base path</param>
        /// <param name="lSTRING_AccessibleFiles">All of accessible files found</param>
        private static void GetAllAccessibileFiles(string STRING_BasePath, IList<string> lSTRING_AccessibleFiles)
        {
            try
            {
                Directory.GetFiles(STRING_BasePath)
                    .ToList()
                    .ForEach(s => lSTRING_AccessibleFiles.Add(s));

                Directory.GetDirectories(STRING_BasePath)
                    .ToList()
                    .ForEach(s => GetAllAccessibileFiles(s, lSTRING_AccessibleFiles));
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception) { }
        }

        #endregion

    }
}
