using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TeamDeploy.Config;

namespace TeamDeploy
{
    class Program
    {
        static int Main(string[] args)
        {
            var Source = "";
            var SourceMachine = "";
            var Dest = "";
            var DestMachine = "";
            var Ignores = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-source":
                        Source = args[++i];
                        break;
                    case "-sourcemachine":
                        SourceMachine = args[++i];
                        break;
                    case "-dest":
                        Dest = args[++i];
                        break;
                    case "-destmachine":
                        DestMachine = args[++i];
                        break;
                    case "-ignore":
                        Ignores.Add(args[++i]);
                        break;
                    default:
                        break;
                }
            }
                
            string ignores = "";
            string source = "";
            string dest = "";

            foreach (var ignore in Ignores)
            {
                ignores += string.Format("-skip:objectName=filePath,absolutePath={0} ", ignore);
            }

            if (Source.EndsWith(".zip"))
                source = string.Format("-source:package=\"{0}\",encryptPassword=[{1}]", FormatPath(Source), ConfigurationManager.AppSettings["encryptPassword"]);
            else
                source = string.Format("-source:contentPath=\"{0}\"", FormatPath(Source));

            if (!string.IsNullOrEmpty(SourceMachine))
            {
                source += GetComputerCredentials(SourceMachine);
            }

            if (Dest.EndsWith(".zip"))
                dest = string.Format("-dest:package=\"{0}\",encryptPassword=[{1}]", FormatPath(Dest), ConfigurationManager.AppSettings["encryptPassword"]);
            else
                dest = string.Format("-dest:contentPath=\"{0}\"", FormatPath(Dest));

            if (!string.IsNullOrEmpty(DestMachine))
            {
                dest += GetComputerCredentials(DestMachine);
            }

            using (var process = new Process())
            {

                process.StartInfo.FileName = ConfigurationManager.AppSettings["msdeployExecutable"];
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = string.Format("-verb:sync {0} {1} {2}", source, dest, ignores);
                process.Start();

                var sr = process.StandardOutput;
                Console.Write(sr.ReadToEnd());
                return process.ExitCode;
            }
        }

        private static string FormatPath(string path)
        {
            //adds date-support to path's - "e:\directory\date(yyyyMMdd)_file.zip" = "e:\directory\20110512_file.zip"
            var matches = Regex.Matches(path, @"date\(((.|\n)+?)\)", RegexOptions.IgnoreCase);

            int offset = 0;
            foreach (Match match in matches)
            {
                var matchText = DateTime.Now.ToString(match.Groups[1].Value);

                int startLength = match.Groups[0].Index + match.Groups[0].Length + offset;
                path = path.Substring(0, match.Groups[0].Index + offset) +
                    matchText +
                    path.Substring(startLength, path.Length - startLength);

                offset += matchText.Length - match.Groups[0].Value.Length;
            }

            return path;
        }

        private static MachineConfiguration machineConfig = (MachineConfiguration) ConfigurationManager.GetSection("machineConfiguration");

        private static string GetComputerCredentials(string SourceMachine)
        {
            MachineElement m = null;

            foreach (MachineElement machine in machineConfig.Machines)
            {
                if (machine.ComputerName == SourceMachine)
                {
                    m = machine;
                }
            }

            if (m == null) throw new InvalidOperationException(string.Format("{0} was not found in the configuration", SourceMachine));

            if (string.IsNullOrEmpty(m.Username))
                return string.Format(",computername={0}", SourceMachine);

            return string.Format(",computername={0},username=\"{1}\",password=\"{2}\"", SourceMachine, m.Username, m.Password);
        }
    }
}
