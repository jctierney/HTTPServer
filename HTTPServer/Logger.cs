using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    /// <summary>
    /// Class to handle the log file.
    /// 
    /// Format:
    ///  - Status: ERROR/MESSAGE
    ///  - Class: Class the called the message.
    ///  - Message: Simple string message
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The location of the file represented as a string.
        /// </summary>
        public string Location { get; set; }

        private readonly object lockObj = new object();
        
        /// <summary>
        /// Determines whether or not we should write to the log file.
        /// </summary>
        private bool WriteToLog { get; set; }

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        public Logger()
        {
            Location = null;
            WriteToLog = false;
        }

        /// <summary>
        /// Initializes a new instance of the Logger class with
        /// a location of the log file save.
        /// </summary>
        /// <param name="location"></param>
        public Logger(string location)
        {
            Location = location;
            WriteToLog = true;
            Console.WriteLine("Logger(string location)");
        }

        /// <summary>
        /// Appends a new message to the log file.
        /// </summary>
        /// <param name="message">The LogMessage</param>
        public void AppendToLogFile(LogMessage message)
        {
            if (WriteToLog)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append(DateTime.Now.ToString());
                sb.Append(",");
                sb.Append(message.Status);
                sb.Append(",");
                sb.Append("Header: ");
                sb.Append(message.Header);
                sb.Append(",");
                sb.Append(message.Message);
                sb.Append(",");
                sb.Append(message.Method);
                sb.AppendLine();
                sb.AppendLine();

                //do
                //{
                //    Thread.Sleep(1000);
                //} while (CheckIfLogIsBeingUsed(Location));

                // Append our message to the log.
                lock (lockObj)
                {                    
                    File.AppendAllText(Location, sb.ToString());
                    Thread.Sleep(100);
                }
            }                        
        }

        /// <summary>
        /// Checks to see if the log file is currently in use.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool CheckIfLogIsBeingUsed(string filename)
        {
            try
            {
                File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exc)
            {
                return true;
            }

            return false;
        }
    }
}
