using System;
using System.Windows.Forms;
using HotelManagementSystem.Database;

namespace HotelManagementSystem;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        DatabaseHelper.InitializeDatabase();
        var loginForm = new LoginForm();
        if (loginForm.ShowDialog() == DialogResult.OK && loginForm.LoggedInUserId != null)
        {
            Application.Run(new DashboardForm());
        }
    }    
}