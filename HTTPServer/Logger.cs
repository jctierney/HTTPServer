using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

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
        
        /// <summary>
        /// Determines whether or not we should write to the log file.
        /// </summary>
        private bool WriteToLog { get; set; }
        
        private Thread writeThread;
        
        private ConcurrentQueue<LogMessage> messageQueue;

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
			messageQueue = new ConcurrentQueue<LogMessage>();
            writeThread = new Thread(logThreadCallback);
			if( ! File.Exists(Location)) {
				File.Create(Location);
			}
            writeThread.Start();
        }

        /// <summary>
        /// Appends a new message to the log file.
        /// </summary>
        /// <param name="message">The LogMessage</param>
        public void AppendToLogFile(LogMessage message)
        {
			messageQueue.Enqueue(message);
        }
        
        public void logThreadCallback()
        {
			while (true) 
			{
				LogMessage message = null;
				if (messageQueue.Any() && messageQueue.TryDequeue(out message)) {
					if (!WriteToLog) continue;
					
					
					if (WriteToLog)
					{
						StringBuilder sb = new StringBuilder();
						sb.AppendLine();
						sb.Append(DateTime.Now.ToString());
						sb.Append(",");
						sb.Append(message.Status);
						sb.Append(",");
					sb.Append("Header: \t");
						sb.Append(message.Header);
						sb.Append(",");
						sb.Append(message.Message);
						sb.Append(",");
						sb.Append(message.Method);
						// Append our message to the log.
						File.AppendAllText(Location, sb.ToString());
					}
				} else {
					Thread.Sleep(250);
				}
			}
        }
    }
}
