using CefSharp;
using CefSharp.WinForms;

namespace MessagesforWeb
{
    internal static class Program
    {
        public static Messages messages_form;
        static string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string cacheDirPath { get; set; } = default!;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            cacheDirPath = Path.Combine(AppPath, "CEF");
            CefSettings settings = new CefSettings();
            settings.CachePath = cacheDirPath;
            Cef.Initialize(settings);

            messages_form = new Messages();
            Application.Run(messages_form);
        }

        public static void reset(object sender, EventArgs e)
        {
            Cef.Shutdown();
            Thread.Sleep(200);
            Application.Restart();
            try
            {
                Directory.Delete(cacheDirPath, true);
            }
            catch (Exception ex)
            {

            }
            Environment.Exit(0);
        }
    }
}