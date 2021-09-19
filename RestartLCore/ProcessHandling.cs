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
            IntPtr firstWindowPtr = IntPtr.Zero;
            IntPtr secondWindowPtr = IntPtr.Zero; // The ptr changes when the window opens up

            int counter = 0;

            Task<string> results = Task.Run(async () =>
            {
                // Wait for the window to open
                while (firstWindowPtr == IntPtr.Zero)
                {
                    firstWindowPtr = FindWindowByCaption(IntPtr.Zero, procWindowName);

                    await Task.Delay(200);

                    // Prevent infinite loop, in case window is not found in the set time period
                    if (counter > 50)
                    {
                        break;
                    }
                    counter += 1;
                }

                if (firstWindowPtr != IntPtr.Zero)
                {
                    counter = 0;
                    IntPtr tempPtr = IntPtr.Zero;

                    // Grab the second window ptr
                    while (secondWindowPtr == IntPtr.Zero)
                    {
                        await Task.Delay(200);

                        tempPtr = FindWindowByCaption(IntPtr.Zero, procWindowName);

                        if (tempPtr != firstWindowPtr) 
                            secondWindowPtr = tempPtr;

                        // Prevent infinite loop, in case window is not found in the set time period
                        if (counter > 50)
                        {
                            break;
                        }
                        counter += 1;
                    }

                    if (secondWindowPtr != IntPtr.Zero) { 
                        SendMessage(secondWindowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        return $"Closed window {procWindowName}";
                    }
                    else
                        return $"Could not close window {procWindowName}";
                }
                else
                    return $"Could not close window {procWindowName}";
            });

            return results.Result;
        }
    }
}
