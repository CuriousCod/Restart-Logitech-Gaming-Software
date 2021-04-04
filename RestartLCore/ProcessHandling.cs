using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestartLCore
{
    public partial class MainWindow : Window
    {
        public void RestartProcess(string proc, string procLocation)
        {
            KillProcess(proc);

            AppendLog($"Starting process {procLocation}");
            StartProcess(procLocation);
        }

        public void KillProcess(string procName)
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

        public void StartProcess(string procName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = procName;
            proc.Start();
        }
    }
}
