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
        public class RestartProgram
        {
            public string name { get; set; }
            public string path { get; set; }
            public string windowName { get; set; }

        }

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private RestartProgram restartable = new RestartProgram();

        public MainWindow()
        {
            InitializeComponent();

            // Track windows session events
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            // Initialize notification area features
            InitializeNotificationArea();

            // Init restartable program object
            restartable.name = "LCore";
            restartable.path = @"C:\Program Files\Logitech Gaming Software\LCore.exe";
            restartable.windowName = "Logitech Gaming Software";

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
                    if ((bool)checkbox_RestartOnLogin.IsChecked)
                        RestartProcess(restartable);
                    break;
                default:
                    break;
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RestartProcess(restartable);
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

        async private void RestartProcess(RestartProgram restartable)
        {
            buttonRestartProgram.IsEnabled = false;

            await Task.Run(() =>
            {
                ProcessHandling procHandle = new ProcessHandling();

                List<string> killedProcs = procHandle.KillProcess(restartable.name);

                if (killedProcs.Count == 0)
                {

                    // Required when updating UI from other thread
                    Dispatcher.Invoke(() =>
                    {
                        AppendLog($"Could not kill {restartable.name}");
                    });
                    
                }
                else
                {
                    foreach (string killedProc in killedProcs)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            AppendLog($"{killedProc} killed");
                        });   
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    AppendLog($"Starting process {restartable.path}");
                });
                

                procHandle.StartProcess(restartable.path);

                string results = procHandle.CloseAppWindow(restartable.windowName);

                Dispatcher.Invoke(() =>
                {
                    AppendLog(results);
                    buttonRestartProgram.IsEnabled = true;
                });

            });

        }

    }
}
