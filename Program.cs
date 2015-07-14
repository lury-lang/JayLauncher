using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace JayLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine("You must specify 4 file paths: option grammar_jay skeleton_cs output_target");
                Environment.Exit(1);
            }

            Start(args);
        }

        static void Start(string[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                CreateNoWindow = false,
                ErrorDialog = false,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "jay.exe"),
                Arguments = args[0] + " " + args[1],
               
            };

            string str = File.ReadAllText(args[2]);
            StringBuilder output = new StringBuilder();
            var p = Process.Start(info);

            p.OutputDataReceived += (s, e) => output.AppendLine(e.Data);
            p.BeginOutputReadLine();

            p.StandardInput.Write(str);
            p.StandardInput.Dispose();

            p.WaitForExit();
            p.Dispose();

            File.WriteAllText(args[3], output.ToString(), Encoding.UTF8);
        }
    }
}
