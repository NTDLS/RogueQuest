using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Constants
    {
        public const String RegistryKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\NetworkDLS\\Rougue Quest";
        public static string _BaseAssetPath = null;

        public static string BaseAssetPath
        {
            get
            {
                if (_BaseAssetPath == null)
                {
                    RegistryKey hklm = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NetworkDLS\Rogue Quest", false);
                    _BaseAssetPath = (string)hklm.GetValue("AssetPath", "");

                    if (_BaseAssetPath.EndsWith("\\") == false)
                    {
                        _BaseAssetPath += "\\";
                    }
                }

                return _BaseAssetPath;
            }
        }

        public static string RecentScenariosFile
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RougeQuest\\Saves";

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                path += "\\RecentScenarios.txt";

                if (System.IO.File.Exists(path) == false)
                {
                    System.IO.File.WriteAllText(path, string.Empty);
                }

                return path;
            }
        }

        public static string GetAssetPath(string partialAssetPath)
        {
            return BaseAssetPath + partialAssetPath;
        }

        public static string GetAssetPath()
        {
            return BaseAssetPath;
        }
    }
}