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
            string exepath = GetExecutionPath();

            CheckFilePath(exepath, args);
            CheckModification(args);

            ProcessStartInfo info = new ProcessStartInfo()
            {
                CreateNoWindow = false,
                ErrorDialog = false,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = exepath,
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

        static void CheckFilePath(string exepath, string[] args)
        {
            if (!File.Exists(exepath))
            {
                Console.Error.Write("jay is not found: {0}", exepath);
                Environment.Exit(1);
            }

            if (!File.Exists(args[1]))
            {
                Console.Error.Write("grammar_jay file is not found: {0}", args[1]);
                Environment.Exit(1);
            }

            if (!File.Exists(args[2]))
            {
                Console.Error.Write("skeleton_cs file is not found: {0}", args[2]);
                Environment.Exit(1);
            }
        }

        static void CheckModification(string[] args)
        {
            var jaytime = File.GetLastWriteTime(args[1]);
            var skeletontime = File.GetLastWriteTime(args[2]);
            var outputtime = File.GetLastWriteTime(args[3]);

            if (outputtime > jaytime && outputtime > skeletontime)
            {
                Console.Write("Everything is up-to-date.");
                Environment.Exit(0);
            }
        }

        static string GetExecutionPath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return Path.Combine(GetAssemblyLocation(), "jay", "windows", "jay.exe");
            }
            else
            {
                var jayPath = Path.Combine(GetAssemblyLocation(), "jay", "unix", "jay");

                if (!File.Exists(jayPath))
                {
                    ProcessStartInfo info = new ProcessStartInfo()
                    {
                        CreateNoWindow = false,
                        ErrorDialog = false,
                        FileName = "make",
                        WorkingDirectory = Path.Combine(GetAssemblyLocation(), "jay", "unix")
                    };

                    using (var p = Process.Start(info))
                    {
                        p.WaitForExit();

                        if (p.ExitCode != 0)
                        {
                            Console.Error.Write("make exited abnormally! : {0}", p.ExitCode);
                            Environment.Exit(1);
                        }
                    }
                }

                return jayPath;
            }
        }

        static string GetAssemblyLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
