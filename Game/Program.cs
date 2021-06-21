using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            foreach (var arg in args)
            {
                testFor = "/map:";
                if (arg.ToLower().StartsWith(testFor))
                {
                    string value = arg.Substring(testFor.Length);
                    Application.Run(new FormMain(mapPath: value));
                }
                testFor = "/game:";
                if (arg.ToLower().StartsWith(testFor))
                {
                    string value = arg.Substring(testFor.Length);
                    Application.Run(new FormMain(gamePath: value));
                }
            }
        }
    }
}
