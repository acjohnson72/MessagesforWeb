namespace MessagesforWeb
{
    public class ApplicationSettings
    {
        public int WindowHeight { get; set; } = 800;
        public int WindowWidth { get; set; } = 600;
        public FormWindowState WindowState { get; set; }
        public bool ShowTrayInfo { get; set; }
        public bool OpenMinimized { get; set; }
        public bool StartWithWindows { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
