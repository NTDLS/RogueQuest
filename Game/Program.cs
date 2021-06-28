using System;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Count() == 0)
            {
                Application.Run(new FormMain());
                return;
            }

            string testFor = "";

            string levelFile = "";
            int levelIndex = 0;

            foreach (var arg in args)
            {
                testFor = "/levelfile:";
                if (arg.ToLower().StartsWith(testFor))
                {
                    levelFile = arg.Substring(testFor.Length);
                }
                testFor = "/levelindex:";
                if (arg.ToLower().StartsWith(testFor))
                {
                    levelIndex = Int32.Parse(arg.Substring(testFor.Length));
                }
            }


            Application.Run(new FormMain(levelFile, levelIndex));
        }
    }
}
