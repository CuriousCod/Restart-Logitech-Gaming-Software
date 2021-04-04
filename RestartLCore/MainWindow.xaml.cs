using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestartLCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Windows.Forms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            // Track windows session events
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            // Initialize notification area features
            InitializeNotificationArea();

        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    // Going into lock/standby screen
                    break;
                case SessionSwitchReason.SessionUnlock:
                    // Back from lock/standby
                    RestartProcess("LCore", @"C:\Program Files\Logitech Gaming Software\Lcore.exe");
                    break;
                default:
                    break;
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RestartProcess("LCore", @"C:\Program Files\Logitech Gaming Software\Lcore.exe");
            }
        }

        public void AppendLog(string text)
        {
            Log.Text += $"{DateTime.Now} - {text} \n";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (notifyIcon != null)
                    notifyIcon.ShowBalloonTip(2000);
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckTrayIcon();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
            if (notifyIcon != null)
                notifyIcon.ShowBalloonTip(2000);
        }
    }
}
