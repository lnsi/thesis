using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Displex
{
    public class Logger
    {
        private static string userInitials = "TE";
        private static string sessionNr = "00";
        private static string filename = "log00.csv";

        private static StreamWriter sw;
        private static StringBuilder builder;

        static Logger() {

              // load the logging path

            string relativepath = String.Concat(@"..\..\Logs\",filename);
            string filepath = Path.Combine(Environment.CurrentDirectory, relativepath);

              // if the file doesn't exist, create it

            if (!File.Exists(filepath))
            {
                FileStream fs = new FileStream(filepath,
                        FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
            }
              // open up the streamwriter for writing..

              sw = File.AppendText(filepath);
              Console.WriteLine("Logger ready...");
        }

        public static void Log(string command, string action)
        {
            builder = new StringBuilder();
            builder.Append(sessionNr);
            builder.Append(",");
            builder.Append(userInitials);
            builder.Append(",");
            builder.Append(command);
            builder.Append(",");
            builder.Append(action);
            builder.Append(",");
            builder.Append(DateTime.Now.ToString("HH:mm:ss"));

            lock(sw)
            {
                sw.WriteLine(builder.ToString());
                sw.Flush();
            }
        }
    }
}
