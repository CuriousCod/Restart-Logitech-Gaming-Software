using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RestartLCore
{
    public class ProcessHandling
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;

        public List<string> KillProcess(string procName)
        {
            Process[] procs = Process.GetProcessesByName(procName);
            List<string> kills = new List<string>();

            foreach (Process proc in procs)
            {
                if (proc.ProcessName == procName)
                {
                    proc.Kill();
                    kills.Add(procName);
                }
            }

            return kills;
        }

        public void StartProcess(string procName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = procName;
            proc.Start();

        }

        public string CloseAppWindow(string procWindowName)
        {
            IntPtr windowPtr = IntPtr.Zero;

            int counter = 0;

            // Wait for the window to open
            while (windowPtr == IntPtr.Zero)
            {
                windowPtr = FindWindowByCaption(IntPtr.Zero, procWindowName);

                Thread.Sleep(500);

                // Prevent infinite loop, in case window is not found in the set time period
                if (counter > 10)
                {
                    break;
                }
                counter += 1;
            }

            if (windowPtr != IntPtr.Zero)
            {
                // Wait for startup
                // TODO There should be a better way to determine when the window has initialized
                Thread.Sleep(2000);

                // Need to grab the window one more time after it has initialized
                windowPtr = FindWindowByCaption(IntPtr.Zero, procWindowName);

                SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                return $"Attempted to close window {procWindowName}";
            }

            return $"Could not close window {procWindowName}";
        }
    }
}
