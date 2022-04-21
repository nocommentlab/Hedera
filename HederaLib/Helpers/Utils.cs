using libyaraNET;
using Microsoft.Win32;
using ncl.hedera.HederaLib.Models;
using ncl.hedera.HederaLib.Models.Configuration;
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
        private const string NAMEDPIPE_BASE_DIR = @"\\.\\pipe\\";
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
            string STRING_ImpHash;
            PeFile peFile = null;
            try
            {
                peFile = new(STRING_FilePath);
            }
            catch (Exception) { }

            STRING_ImpHash = peFile?.ImpHash ?? String.Empty;

            return STRING_ImpHash;
        }

        

        

        public static List<string> GetSystemNamedPipes()
        {
            return Directory.GetFiles(NAMEDPIPE_BASE_DIR).ToList();
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

            if(STRING_TextWithTemplate.Contains("{{machine_name}}"))
            {
                STRING_TextWithTemplate = STRING_TextWithTemplate.Replace("{{machine_name}}", Environment.MachineName);
            }
            return STRING_TextWithTemplate;
        }

        /// <summary>
        /// Reads the registry data value of given registry key. It supports recursive mode
        /// </summary>
        /// <param name="OBJECT_RegistryIoC">The dynamic RegistryIoC object reads from yaml file</param>
        /// <returns>The registry data value, otherwise null</returns>
        [SupportedOSPlatform("windows")]
        public static List<RegistryItem> ReadRegistryDataValue(RegistryIndicator registryIoc)
        {
            
            List<RegistryItem> lRegistryItem = null;

            RegistryKey REGISTRY_RegistryKey;

            RegistryKey REGISTRYKEY_BaseRegistry = SetBaseRegistryKey(registryIoc.BaseKey);

            if (null != REGISTRYKEY_BaseRegistry)
            {
                REGISTRY_RegistryKey = REGISTRYKEY_BaseRegistry.OpenSubKey(registryIoc.Key);

                if (null != REGISTRY_RegistryKey)
                {
                    if (registryIoc.IsRecursive)
                    {
                        SearchSubKeys(REGISTRY_RegistryKey, registryIoc.ValueName, ref lRegistryItem);
                        
                    }
                    else
                    {

                        List<string> lSTRING_ValueNames = REGISTRY_RegistryKey.GetValueNames()
                                                          .Where(valueName => Regex.IsMatch(valueName, registryIoc.ValueName))
                                                          .ToList();

                        if (lSTRING_ValueNames.Count > 0)
                        {
                            lRegistryItem = new();

                            foreach (string STRING_ValueName in lSTRING_ValueNames)
                            {
                                if (STRING_ValueName != null)
                                {
                                    lRegistryItem.Add(new RegistryItem
                                    {
                                        STRING_Name = REGISTRY_RegistryKey.Name,
                                        STRING_ValueName = STRING_ValueName,
                                        OBJECT_ValueData = REGISTRY_RegistryKey.GetValue(STRING_ValueName)
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return lRegistryItem;
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


        [SupportedOSPlatform("windows")]
        public static List<string> IsPipeExists(PipeIndicator pipeIoC)
        {
            return GetSystemNamedPipes().Where(pipe => Regex.IsMatch(pipe, pipeIoC.Name)).ToList();

             
        }
        /// <summary>
        /// Checks if a file exists. It supports recursive mode.
        /// </summary>
        /// <param name="OBJECT_FileIoC">The dynamic FileIoC object reads from yaml file</param>
        /// <returns>True if exists, otherwise false</returns>
        [SupportedOSPlatform("windows")]
        public static List<FileItem> IsFileExists(FileIndicator fileIoc)
        {

            List<string> lSTRING_Filename = null;

            List<FileItem> lFileItems = null;

            List<string> LIST_AccessibleFiles = new();

            if (fileIoc.IsRecursive)
            {
                GetAllAccessibileFiles(fileIoc.Path, LIST_AccessibleFiles);
                lSTRING_Filename = LIST_AccessibleFiles.Where(accessibleFile => Regex.IsMatch(Path.GetFileName(accessibleFile), fileIoc.Name, RegexOptions.IgnoreCase)).ToList();

                /* Free the memory */
                LIST_AccessibleFiles.Clear();
                LIST_AccessibleFiles = null;
                GC.Collect();
            }
            else
            {

                if (File.Exists(fileIoc.Path))
                {
                    lSTRING_Filename = new();
                    lSTRING_Filename.Add(fileIoc.Path);

                }

            }

            if (null != lSTRING_Filename && lSTRING_Filename.Count > 0)
            {
                lFileItems = new();
                foreach (string STRING_Filename in lSTRING_Filename)
                {

                    lFileItems.Add(
                     new FileItem
                     {
                         STRING_Path = STRING_Filename,
                         FILEATTRIBUTES_Attributes = File.GetAttributes(STRING_Filename),
                         DATETIME_UTCCreationTime = File.GetCreationTimeUtc(STRING_Filename),
                         FILESECURITY_ACL = null
                     });
                }

            }
            return lFileItems;
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
        private static void SearchSubKeys(RegistryKey REGISTRYKEY_RootKey, String STRING_SearchedValue, ref List<RegistryItem> lRegistryItem)
        {

            RegistryItem registryItem = null;
            string[] vSTRING_Subkeys = REGISTRYKEY_RootKey.GetSubKeyNames();
            int INT32_Idx = 0;

            if (vSTRING_Subkeys.Length > 0 )
            {
                lRegistryItem ??= new();

                while (INT32_Idx < vSTRING_Subkeys.Length)
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
                                    
                                    lRegistryItem.Add(registryItem);

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            SearchSubKeys(REGISTRYKEY_Key, STRING_SearchedValue, ref lRegistryItem);
                        }
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    INT32_Idx++;
                }
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
