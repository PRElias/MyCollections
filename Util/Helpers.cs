using System;
using System.IO;
using System.Reflection;

namespace MyCollections.Util
{
    public class Helper
    {
        public static string RemoveSpecialCharacters(string input)
        {
            var invalids = System.IO.Path.GetInvalidFileNameChars();
            return String.Join("", input.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.').Replace(" ", "");
        }

        public static void ExecuteCommand(string comm)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            string directory = Path.Combine(Directory.GetCurrentDirectory());
            startInfo.WorkingDirectory = directory + @"\docs\games";
            startInfo.Arguments = "/C " + comm;
            process.StartInfo = startInfo;
            process.Start();
            process.Close();
        }

        public static void Commit()
        {
            ExecuteCommand("git add .");
            ExecuteCommand("git commit -m AtualizaçãoJogos");
            ExecuteCommand("git push");
        }
    }
}
