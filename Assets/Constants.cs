using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Constants
    {
        public static string BasePath { get; set; } = @".\..\..\..\..\Assets\";

        public static string GetAssetPath(string partialAssetPath)
        {
            return BasePath + partialAssetPath;
        }
    }
}
