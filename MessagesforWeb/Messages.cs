using CefSharp;

using Microsoft.Win32;

using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Linq;

namespace MessagesforWeb
{
    public partial class Messages : Form
    {
        NameValueCollection ConfigAppSettings = ConfigurationManager.AppSettings;

        static string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        string browserUrl { get; set; }
        bool IsAuthenticated { get; set; }
        bool WindowRestore { get; set; }

        public void InitBrowser()
        {

            if (IsAuthenticated) { browserUrl = "https://messages.google.com/"; }
            else { browserUrl = "https://messages.google.com/web/authentication"; }

            browser.Size = new Size(GetApplicationSettings().WindowWidth, GetApplicationSettings().WindowHeight);
            browser.Load(browserUrl);
            browser.AddressChanged += OnBrowserAddressChanged;

        }

        public Messages()
        {
            InitializeComponent();
            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = this.ContextMenuStrip;

            GetApplicationSettings();

            InitBrowser();

        }

        //public event EventHandler<AddressChangedEventArgs> AddressChanged;

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (e.Address.Contains("https://messages.google.com/web/authentication"))
            {
                IsAuthenticated = false;
                return;
            }
            else
            {
                IsAuthenticated = true;
            }
            if (IsAuthenticated)
            {
                if (e.Address.Contains("https://messages.google.com/web/authentication"))
                {
                    browser.Load("https://messages.google.com/web/authentication");
                }
                else
                {
                    SetApplicationSettings("IsAuthenticated", "True");
                }
            }
            else
            {
                if (e.Address.Contains("https://messages.google.com/web/authentication"))
                {
                    browser.Load("https://messages.google.com/web/authentication");
                }
            }
            this.InvokeOnUiThreadIfRequired(() => Text = e.Address);
        }

        private void Messages_Load(object sender, EventArgs e)
        {

        }

        // generate a method to notify the user that the application is running in the background
        private void Messages_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowRestore = false;
                Hide();
                ShowIcon = false;
                notifyIcon1.Visible = true;
            }
        }

        // Show a Balloon Tip when the application is minimized

        // Add a notification listener for the application
        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Click here to hide this message in the future.", "Messages for Web", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                SetApplicationSettings("ShowTrayInfo", "False");
            }
        }



        private void Messages_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                WindowRestore = false;
                Hide();
                ShowIcon = false;
                notifyIcon1.Visible = true;
                if (GetApplicationSettings().ShowTrayInfo)
                {
                    //var notification = notifyIcon1.ShowBalloonTip(3000, "Messages for Web", "Click here to hide this message in the future.", ToolTipIcon.Info);

                    //notifyIcon1.BalloonTipClicked += (sender, e) =>
                    //{
                    //    DialogResult result = MessageBox.Show("Click here to hide this message in the future.", "Messages for Web", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    if (result == DialogResult.OK ) {
                    //        Settings.Default.ShowTrayInfo = false;
                    //        Settings.Default.Save();
                    //    }
                    //};

                    notifyIcon1.BalloonTipClicked += this.NotifyIcon1_BalloonTipClicked;
                }
            }
            else if (WindowState == FormWindowState.Normal)
            {
                //if (ConfigAppSettings.Get("WindowHeight") == null) { ConfigAppSettings.Set("WindowHeight", this.Height.ToString()); }
                //if (ConfigAppSettings.Get("WindowWidth") == null) { ConfigAppSettings.Set("WindowWidth", this.Width.ToString()); }

                //if (ConfigAppSettings.Get("WindowHeight") != this.Height.ToString()) { ConfigAppSettings.Set("WindowHeight", this.Height.ToString()); }
                //if (ConfigAppSettings.Get("WindowWidth") != this.Width.ToString()) { ConfigAppSettings.Set("WindowWidth", this.Width.ToString()); }

            }


        }


        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowRestore = false;
            ShowInTaskbar = true;

            this.WindowState = FormWindowState.Normal;
            this.Height = GetApplicationSettings().WindowHeight;
            this.Width = GetApplicationSettings().WindowWidth;
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.Reload();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetApplicationSettings("IsAuthenticated", "False");
            Program.reset(null, new EventArgs());

        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // Get/Set Application Settings
        public ApplicationSettings GetApplicationSettings()
        {
            // flag == 0 = read setting value
            // flag == 1 = write setting value

            ApplicationSettings applicationSettings = new ApplicationSettings();
            var xmldoc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"));
            var elements = xmldoc.Elements("ApplicationSettings").Elements();
            IsAuthenticated = bool.Parse(elements.FirstOrDefault(x => x.Name == "IsAuthenticated").Value);



            applicationSettings.WindowWidth = int.Parse(elements.FirstOrDefault(x => x.Name == "WindowWidth").Value);
            applicationSettings.WindowHeight = int.Parse(elements.FirstOrDefault(x => x.Name == "WindowHeight").Value);
            applicationSettings.ShowTrayInfo = bool.Parse(elements.FirstOrDefault(x => x.Name == "ShowTrayInfo").Value);
            applicationSettings.StartWithWindows = bool.Parse(elements.FirstOrDefault(x => x.Name == "StartWithWindows").Value);
            applicationSettings.IsAuthenticated = bool.Parse(elements.FirstOrDefault(x => x.Name == "IsAuthenticated").Value);

            var winstate = elements.FirstOrDefault(x => x.Name == "WindowState").Value;
            switch (winstate)
            {
                case "Normal":
                    applicationSettings.WindowState = FormWindowState.Normal;
                    break;
                case "Minimized":
                    applicationSettings.WindowState = FormWindowState.Minimized;
                    break;
                case "Maximized":
                    applicationSettings.WindowState = FormWindowState.Maximized;
                    break;
                default:
                    applicationSettings.WindowState = FormWindowState.Normal;
                    break;
            }
            //}

            //// save screen size to xml file
            //if (flag == 1 && this.WindowState == FormWindowState.Normal)
            //{
            //    elements.FirstOrDefault(x => x.Name == "WindowWidth").Value = this.Width.ToString();
            //    elements.FirstOrDefault(x => x.Name == "WindowHeight").Value = this.Height.ToString();
            //    elements.FirstOrDefault(x => x.Name == "WindowState").Value = this.WindowState.ToString();
            //    xmldoc.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"));
            //}

            //if (flag == 1 && this.WindowState == FormWindowState.Minimized)
            //{
            //    elements.FirstOrDefault(x => x.Name == "WindowState").Value = this.WindowState.ToString();
            //    xmldoc.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"));
            //}

            // If startwithwindows is true check startToolStripMenuItem is checked
            if (bool.Parse(elements.FirstOrDefault(x => x.Name == "StartWithWindows").Value))
            {
                startToolStripMenuItem.Checked = true;
            }
            else
            {
                startToolStripMenuItem.Checked = false;
            }

            return applicationSettings;
        }

        public void SetApplicationSettings(string settingName, string value)
        {
            ApplicationSettings applicationSettings = new ApplicationSettings();
            var xmldoc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"));
            var elements = xmldoc.Elements("ApplicationSettings").Elements();
            elements.FirstOrDefault(x => x.Name == settingName).Value = value;
            xmldoc.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.xml"));
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startToolStripMenuItem.Checked)
            {
                SetApplicationSettings("StartWithWindows", "True");
                key.SetValue("Messages for Web", "\"" + Application.ExecutablePath + "\"");
            }
            else
            {
                SetApplicationSettings("StartWithWindows", "False");
                key.DeleteValue("Messages for Web", false);
            }

        }
    }
}