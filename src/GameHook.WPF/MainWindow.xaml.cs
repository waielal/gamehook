using AutoUpdaterDotNET;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GameHook.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region WindowManagement
        bool IsClosing = false;
        bool CanClose = false;
        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowControlButton_Maximize_Refresh();
        }
        private void WindowControlButton_Minimize_Click(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void WindowControlButton_Maximize_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void WindowControlButton_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void WindowControlButton_Maximize_Refresh()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowBorderBorder.Visibility = Visibility.Hidden;
                WindowControlButton_Maximize.Visibility = Visibility.Collapsed;
                WindowControlButton_MaximizeRestore.Visibility = Visibility.Visible;
            }
            else
            {
                WindowBorderBorder.Visibility = Visibility.Visible;
                WindowControlButton_Maximize.Visibility = Visibility.Visible;
                WindowControlButton_MaximizeRestore.Visibility = Visibility.Collapsed;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CanClose == true)
            {
                return;
            }
            if (IsClosing == false)
            {
                var ta = new DoubleAnimation();
                ta.Duration = TimeSpan.FromSeconds(0.2);
                QuadraticEase EasingFunction = new QuadraticEase();
                EasingFunction.EasingMode = EasingMode.EaseOut;
                ta.EasingFunction = EasingFunction;
                ta.To = 0;
                ta.Completed += (_, _) => { CanClose = true; Close(); };
                BeginAnimation(OpacityProperty, ta);
                IsClosing = true;
                e.Cancel = true;
            }
            else
            {
                e.Cancel = true;
            }
        }

        #region MaximizingFix
        CompositionTarget WindowCompositionTarget { get; set; }

        double CachedMinWidth { get; set; }

        double CachedMinHeight { get; set; }

        Cmr.Win32.POINT CachedMinTrackSize { get; set; }

#pragma warning disable CS8605 // Unboxing a possibly null value.
        IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    Cmr.Win32.MINMAXINFO mmi = (Cmr.Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Cmr.Win32.MINMAXINFO));
                    IntPtr monitor = Cmr.Win32.MonitorFromWindow(hwnd, 0x00000002 /*MONITOR_DEFAULTTONEAREST*/);
                    if (monitor != IntPtr.Zero)
                    {
                        Cmr.Win32.MONITORINFO monitorInfo = new Cmr.Win32.MONITORINFO { };
                        Cmr.Win32.GetMonitorInfo(monitor, monitorInfo);
                        Cmr.Win32.RECT rcWorkArea = monitorInfo.rcWork;
                        Cmr.Win32.RECT rcMonitorArea = monitorInfo.rcMonitor;
                        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                        if (!CachedMinTrackSize.Equals(mmi.ptMinTrackSize) || CachedMinHeight != MinHeight && CachedMinWidth != MinWidth)
                        {
                            mmi.ptMinTrackSize.x = (int)((CachedMinWidth = MinWidth) * WindowCompositionTarget.TransformToDevice.M11);
                            mmi.ptMinTrackSize.y = (int)((CachedMinHeight = MinHeight) * WindowCompositionTarget.TransformToDevice.M22);
                            CachedMinTrackSize = mmi.ptMinTrackSize;
                        }
                    }
                    Marshal.StructureToPtr(mmi, lParam, true);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }
#pragma warning restore CS8605 // Unboxing a possibly null value.
        #endregion
        #endregion WindowManagement

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    AutoUpdater.ShowUpdateForm(args);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    MessageBox.Show(
                        @"There was a problem reaching our update server. Please try again later." +
                        Environment.NewLine +
                        Environment.NewLine +
                        @"For additional assistance please visit our website at https://gamehook.io/",
                        @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        public MainWindow()
        {
            if (BuildEnvironment.IsReleaseBuild)
            {
                AutoUpdater.AppTitle = "GameHook";

                AutoUpdater.Synchronous = true;
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.HttpUserAgent = "AutoUpdater";
                AutoUpdater.ReportErrors = true;
                AutoUpdater.RunUpdateAsAdmin = false;

                AutoUpdater.LetUserSelectRemindLater = false;
                AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
                AutoUpdater.RemindLaterAt = 1;

                string jsonPath = Path.Combine(BuildEnvironment.ConfigurationDirectory, "updater.json");
                AutoUpdater.PersistenceProvider = new JsonFilePersistenceProvider(jsonPath);

                AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;

                AutoUpdater.Start("https://cdn.gamehook.io/GameHookWpf_AutoUpdater.xml");
            }

            InitializeComponent();

            WindowControlButton_Maximize_Refresh();

            #region MaximizingFix
            SourceInitialized += (s, e) =>
            {
                WindowCompositionTarget = PresentationSource.FromVisual(this).CompositionTarget;
                System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle).AddHook(WindowProc);
            };
            #endregion

            WindowBorderTitle.Text = $"GameHook {BuildEnvironment.AssemblyVersion}";

            Task.Run(async () => await Program.Start());
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(BuildEnvironment.ConfigurationDirectory, "WebView2"));
            await WebView.EnsureCoreWebView2Async(env);

            WebView.Source = new Uri("http://localhost:8085");
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
