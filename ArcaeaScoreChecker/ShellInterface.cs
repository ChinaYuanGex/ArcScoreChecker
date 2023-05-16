using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ArcaeaScoreChecker
{
    public class ShellInterface
    {
        public static bool IsRooted = false;
        public static bool CheckIfRooted() {
            try
            {
                Process p = Process.Start("su");
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    return false;
                }
                else {
                    IsRooted = true;
                    return true; 
                }

            }
            catch
            {
                return false;
            }
        }
        public static string RootExecuteWithReturn(string cmd, string args = "")
        {
            Process p = new Process();
            p.StartInfo.FileName = "su";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            p.StandardInput.Write($"{cmd} {args}\n");
            p.StandardInput.Flush();

            p.StandardInput.Write("exit\n");
            p.StandardInput.Flush();

            p.WaitForExit(10000);
            if (!p.HasExited) return null;

            return p.StandardOutput.ReadToEnd();
        }
        public static void RootExecute(string cmd,string args = "") {
            RootExecuteWithReturn(cmd, args);
        }
        public static string ExecuteWithReturn(string cmd,string args = "") {
            Process p = new Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd();
        }
        public static void Execute(string cmd,string args = "") { 
            ExecuteWithReturn(cmd,args);
        }
        public static string Whoami() {
            Process p = new Process();
            p.StartInfo.FileName = "whoami";
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            return output;
        }
    }
}
