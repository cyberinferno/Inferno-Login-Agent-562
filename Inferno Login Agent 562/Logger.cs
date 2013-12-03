using System;
using System.IO;

namespace Inferno_Login_Agent_562
{
    public static class Logger
    {
        /// <summary>
        /// Writes specified string into log file
        /// </summary>
        public static void WriteLog(string log)
        {
            try
            {
                using (var sw = new StreamWriter(Config.LogName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt") + " : " + log);
                }
            }
            catch {}
        }
    }
}
