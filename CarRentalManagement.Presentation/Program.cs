using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRentalManagement.Presentation
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ĐỔI FrmCreateAdmin() THÀNH FrmLogin() TẠI ĐÂY:
            Application.Run(new FrmLogin());
        }
    }
}