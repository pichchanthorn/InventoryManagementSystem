using System;
using System.Windows.Forms;
using InventoryManagementSystem.UI;

namespace InventoryManagementSystem
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

            bool showLogin = true;

            while (showLogin)
            {
                showLogin = false;

                using (var loginForm = new frmLogin())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
                    {
                        using (var mainForm = new frmMain(loginForm.AuthenticatedUser))
                        {
                            Application.Run(mainForm);

                            if (mainForm.LogoutRequested)
                            {
                                showLogin = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
