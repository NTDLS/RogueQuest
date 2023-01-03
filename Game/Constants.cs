using System.Windows.Forms;

namespace Game
{
    public static class Constants
    {
        public static int MaxAvailableStatsPool = 40;
        public static int MaxStartingStatLevel = 30;
        public static int MinStartingStatLevel = 1;
        public static int StartingStatLevel = 2;

        public static string SaveFolder
        {
            get
            {
                string path = Assets.Constants.GetUserAssetPath("Saves");

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public static string RecentSaveFilename
        {
            get
            {
                string path = $@"{SaveFolder}\Recent.txt";

                if (System.IO.File.Exists(path) == false)
                {
                    System.IO.File.WriteAllText(path, string.Empty);
                }

                return path;
            }
        }

        public static void Alert(string text)
        {
            MessageBox.Show(text, "RougeQuest");
        }

        public static void Alert(string text, string caption)
        {
            MessageBox.Show(text, $"RougeQuest :: {caption}");
        }
    }
}
