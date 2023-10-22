using CVAiO.Bplus.Core;
using CVAiO.Bplus.MainGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Diagnostics.Process[] processes = null;
            string strCurrentProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper();
            processes = System.Diagnostics.Process.GetProcessesByName(strCurrentProcess);
            if (processes.Length > 1)
            {
                frmMessageBox.Show(EMessageIcon.Error, string.Format("Program \'{0}\' is already Running", System.Diagnostics.Process.GetCurrentProcess().ProcessName));
                return;
            }
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // Nếu gặp lỗi :
            // System.IO.FileNotFoundException: 'Could not load file or assembly
            // 'CVAiO.Bplus.GUI.ByUser, Version = 1.0.0.0, Culture = neutral,
            // PublicKeyToken = null' or one of its dependencies. The system cannot
            // find the file specified.'
            // Nguyên nhân do source code down về chưa được build
            // Chọn: Build -> Build Solution hoặc Ctrl + Shift + B
            Application.Run(new CVAiO.Bplus.MainGUI.MainGUI()); 
            //Application.Run(new ProcessCreatorUI());
            //Application.Run(new AutoCalibrationUI());
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogWriter.Instance.LogException("CurrentDomain_UnhandledException: " + e.ToString());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogWriter.Instance.LogException("Application_ThreadException: " + e.ToString());
        }
    }
}
