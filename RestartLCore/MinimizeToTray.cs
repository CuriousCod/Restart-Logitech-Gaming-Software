using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestartLCore
{
    public class Asd{ public string status = "bar"; }

    public partial class MainWindow : Window
    {

        private void InitializeNotificationArea()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipText = "The app is running. Double-click to show window.";
            notifyIcon.BalloonTipTitle = "Restart LCore";
            notifyIcon.Text = "Restart LCore";

            // Get the icon from resources
            using (System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream)
            {
                notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }

            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);

            // Context menu for notification icon
            var contextMenuNotifyIcon = new System.Windows.Forms.ContextMenu();
            var menuItemRestore = new System.Windows.Forms.MenuItem();
            var menuItemExit = new System.Windows.Forms.MenuItem();

            contextMenuNotifyIcon.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { menuItemRestore, menuItemExit });

            menuItemRestore.Index = 0;
            menuItemRestore.Text = "Restore";
            menuItemRestore.Click += new System.EventHandler(menuItemRestore_Click);

            menuItemExit.Index = 1;
            menuItemExit.Text = "Exit";
            menuItemExit.Click += new System.EventHandler(menuItemExit_Click);

            notifyIcon.ContextMenu = contextMenuNotifyIcon;

        }

        private void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        private void ShowTrayIcon(bool show)
        {
            if (notifyIcon != null)
                notifyIcon.Visible = show;
        }
        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuItemRestore_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }
    }
}
