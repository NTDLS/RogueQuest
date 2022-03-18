using System;
using System.IO;

namespace DummyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.EnumerateFiles(@"C:\NTDLS\RougueQuest\Assets\Tiles\Items\@inTheWorks\Weapons\Ranged", "*.png", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                File.Copy(file, Path.GetDirectoryName(file) + @"\" + Path.GetFileNameWithoutExtension(file) + ".Enchanted.png");
                File.Copy(file, Path.GetDirectoryName(file) + @"\" + Path.GetFileNameWithoutExtension(file) + ".Cursed.png");
            }


        }
    }
}
