using Microsoft.Win32;
using System.IO;

namespace Assets
{
    public class Constants
    {
        public const string RegistryKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\NetworkDLS\\Rougue Quest";
        public static string _BaseCommonAssetPath = null;
        public static string _BaseUserAssetPath = null;

        public static string BaseCommonAssetPath
        {
            get
            {
                if (_BaseCommonAssetPath == null)
                {
                    RegistryKey hklm = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NetworkDLS\Rogue Quest", false);
                    _BaseCommonAssetPath = (string)hklm.GetValue("CommonAssetPath", "");

                    if (_BaseCommonAssetPath.EndsWith("\\") == false)
                    {
                        _BaseCommonAssetPath += "\\";
                    }
                }

                return _BaseCommonAssetPath;
            }
        }

        public static string BaseUserAssetPath
        {
            get
            {
                if (_BaseUserAssetPath == null)
                {
                    RegistryKey hklm = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NetworkDLS\Rogue Quest", false);
                    _BaseUserAssetPath = (string)hklm.GetValue("UserAssetPath", "");

                    if (_BaseUserAssetPath.EndsWith("\\") == false)
                    {
                        _BaseUserAssetPath += "\\";
                    }
                }

                return _BaseUserAssetPath;
            }
        }

        public static string RecentScenariosFile
        {
            get
            {
                string path = System.IO.Path.Combine(BaseUserAssetPath, "Saves");

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                path += "\\RecentScenarios.txt";

                if (File.Exists(path) == false)
                {
                    File.WriteAllText(path, string.Empty);
                }

                return path;
            }
        }

        public static string GetUserAssetPath(string partialAssetPath)
        {
            return Path.Combine(BaseUserAssetPath, partialAssetPath);
        }

        public static string GetUserAssetPath()
        {
            return BaseUserAssetPath;
        }

        public static string GetCommonAssetPath(string partialAssetPath)
        {
            return Path.Combine(BaseCommonAssetPath, partialAssetPath);
        }

        public static string GetCommonAssetPath()
        {
            return BaseCommonAssetPath;
        }
    }
}
