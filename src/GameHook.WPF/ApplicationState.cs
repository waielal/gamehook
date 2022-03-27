namespace GameHook.WPF
{
    public static class ApplicationState
    {
        public static bool IsLogWindowOpen { get; private set; }

        public static void OpenLogWindow()
        {
            if (IsLogWindowOpen) { return; }

            var window = new LogWindow
            {
                ShowInTaskbar = false
            };

            window.Show();
            IsLogWindowOpen = true;
        }

        public static void OnLogWindowClosed()
        {
            IsLogWindowOpen = false;
        }
    }
}
