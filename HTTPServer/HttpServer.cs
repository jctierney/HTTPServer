using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HTTPServer
{
    public class HttpServer
    {
        /// <summary>
        /// Log.
        /// </summary>
        private Logger Log { get; set; }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Assumes there is no logging taking place.
        /// </summary>
        public HttpServer()
        {
            Log = new Logger();
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Uses a string representation of the location of the log class.
        /// </summary>
        /// <param name="logFileLocation"></param>
        public HttpServer(string logFileLocation)
            : this(new Logger(logFileLocation))
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Uses the specified logger as it's logging method.
        /// </summary>
        /// <param name="logger"></param>
        public HttpServer(Logger logger)
        {
            Log = logger;
            LogMessage message = new LogMessage(State.INFO, "New instance of logger.", "HttpServer()");
            LogInformation(message);
            
        }

        /// <summary>
        /// Logs the specified information to the log file.
        /// </summary>
        private void LogInformation(LogMessage message)
        {
            Log.AppendToLogFile(message);
        }
    }
}
