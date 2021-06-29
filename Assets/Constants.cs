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



        public static string GetAssetPath(string partialAssetPath)
        {
            return BaseAssetPath + partialAssetPath;
        }
    }
}