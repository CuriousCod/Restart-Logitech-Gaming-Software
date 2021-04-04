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
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            // Minimize to notification area stuff
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipText = "The app is running. Double-click to show window.";
            notifyIcon.BalloonTipTitle = "Restart LCore";
            notifyIcon.Text = "Restart LCore";

            using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream) {
                notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }
            
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);

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

        private void RestartProcess(string proc, string procLocation)
        {   
            KillProcess(proc);

            AppendLog($"Starting process {procLocation}");
            StartProcess(procLocation);
        }

        private void AppendLog(string text)
        {
            Log.Text += $"{DateTime.Now} - {text} \n";
        }

        private void KillProcess(string procName)
        {
            Process[] procs = Process.GetProcessesByName(procName);

            foreach (Process proc in procs)
            {
                if (proc.ProcessName == procName)
                {
                    proc.Kill();
                    AppendLog($"{proc.ProcessName} killed");
                }
            }
        }

        private void StartProcess(string procName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = procName;
            proc.Start();
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RestartProcess("LCore", @"C:\Program Files\Logitech Gaming Software\Lcore.exe");
            }
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
    }
}
