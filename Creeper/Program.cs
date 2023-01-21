using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Creeper.Helper;
using Creeper.Properties;

namespace Creeper
{
    internal static class Program
    {
        public static FormMain formMain;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (Settings.Default.Chinese)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-cn");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formMain = new FormMain();
            Application.Run(formMain);
        }
    }
}