using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    public class HttpServer
    {
        /// <summary>
        /// The default port. If no other port is given, our main
        /// port is assigned to this value.
        /// </summary>
        private const int DEFAULT_PORT = 8080;

        /// <summary>
        /// Log.
        /// </summary>
        private Logger Log { get; set; }

        /// <summary>
        /// Port number the server runs on.
        /// </summary>
        private int Port { get; set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// No port / log file given.
        /// </summary>
        public HttpServer()
            : this(new Logger(), DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Assumes there is no logging taking place.
        /// </summary>
        public HttpServer(int port)
            : this(new Logger(), port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Has a given log file and port number.
        /// </summary>
        /// <param name="logFileLocation"></param>
        /// <param name="port"></param>
        public HttpServer(string logFileLocation)
            : this(new Logger(logFileLocation), DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Uses a string representation of the location of the log class.
        /// </summary>
        /// <param name="logFileLocation"></param>
        public HttpServer(string logFileLocation, int port)
            : this(new Logger(logFileLocation), port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpServer class.
        /// Uses the specified logger as it's logging method.
        /// </summary>
        /// <param name="logger"></param>
        public HttpServer(Logger logger, int port)
        {
            Log = logger;
            LogMessage message = new LogMessage(State.INFO, "New instance of logger.", "HttpServer()");
            LogInformation(message);
            Initialize();
        }
        #endregion

        /// <summary>
        /// Initializes and sets up the server.
        /// </summary>
        private void Initialize()
        {
            LogMessage message = new LogMessage(State.INFO, "Initializing the server.", "HttpServer.Initialize()");
            LogInformation(message);

            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());
            string x = reader.ReadLine();

            int i = 1;
            while (!String.IsNullOrEmpty(x))
            {
                Console.WriteLine(i.ToString() + x);
                x = reader.ReadLine();
                i++;
            }

            LogInformation(new LogMessage(State.ERROR, "HTTP/1.0 404 Not Found\r\n", "HttpServer.Initialize()"));
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
