﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static readonly int DEFAULT_PORT = 8080;
		
        public static readonly string DEFAULT_DIRECTORY = Directory.GetCurrentDirectory();

        /// <summary>
        /// The directory where requested files should come from.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Log.
        /// </summary>
        public Logger Log { get; set; }

        /// <summary>
        /// Our listener.
        /// </summary>
        private TcpListener Listener { get; set; }

        /// <summary>
        /// Port number the server runs on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Our default MIME types we support.
        /// </summary>
        private Hashtable MimeTypes { get; set; }

        private Hashtable Responses { get; set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer"/> class.
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
			RootDirectory = DEFAULT_DIRECTORY;
			Port = port;
            Log = logger;
			//LogMessage message = new LogMessage(State.INFO, "New instance of logger.", "HttpServer()");
			//LogInformation(message);
			//Initialize();
        }
        #endregion

        /// <summary>
        /// Initializes and sets up the server.
        /// </summary>
        public void Initialize()
        {
            LogMessage message = new LogMessage(State.INFO, "Initializing the server.", "HttpServer.Initialize()");
            LogInformation(message);
			Console.WriteLine("Attempting to start Server..."+
			                  "\n\tPort:\t\t"+Port+
			                  "\n\tLogfile:\t"+Log.Location+
			                  "\n\tRoot:\t\t"+RootDirectory);
            // Create our MIME types Hashtable.
            MimeTypes = new Hashtable();
            MimeTypes.Add(".html", "text/html");        // HTML files
            MimeTypes.Add(".htm", "text/html");         // HTM files
            MimeTypes.Add(".bmp", "image/bmp");         // Bitmaps
            MimeTypes.Add(".jpg", "image/jpg");         // JPEGs
            MimeTypes.Add(".pdf", "application/pdf");   // PDFs
            try
            {
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                Console.WriteLine("Server is running..." + Port);

                // Creates a new main thread. Further threading is done within the StartListen function.
                Thread thread = new Thread(new ThreadStart(StartListen));
                thread.Start();
            }
            catch (Exception e)
            {
                LogInformation(new LogMessage(State.ERROR, "Unable to start server. " + e.Message, "HttpServer.Initialize()"));
            }
        }

        /// <summary>
        /// Sends data.
        ///   - Converts our string data into byte format then
        ///     passes it onto the other SendData function.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="socket"></param>
        public void SendData(string data, ref Socket socket)
        {
            SendData(Encoding.ASCII.GetBytes(data), ref socket);
        }

        /// <summary>
        /// Sends data.
        ///   - Checks to see if we are connected to a socket.
        ///   - Verifies the bytes are present.
        ///   - Uses our socket to send the data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="socket"></param>
        public void SendData(byte[] data, ref Socket socket)
        {
            int bytes = 0;
            try
            {
                if (socket.Connected) // Check to see if we are connected.
                {
                    bytes = socket.Send(data, data.Length, 0);
                    if (bytes == -1)
                    {
                        LogInformation(new LogMessage(State.ERROR, "Cannot send over socket.", "HttpServer.SendData"));
                    }
                    else
                    {
                        LogInformation(new LogMessage(State.INFO, "Sending those bytes", "HttpServer.SendData"));
                    }
                }
                else
                {
                    LogInformation(new LogMessage(State.ERROR, "Connection not working...", "HttpServer.SendData"));
                }
            }
            catch (Exception e)
            {
                LogInformation(new LogMessage(State.ERROR, "An error occurred: " + e.Message, "HttpServer.SendData"));
            }
        }

        /// <summary>
        /// Our basic listener.
        ///   - Sets up our root directory.
        /// </summary>
        private void StartListen()
        {
            // Keep it open, brah.
            while (true)
            {
                Console.WriteLine("Doing some stuff, homies...");
                Socket socket = Listener.AcceptSocket();
                LogInformation(new LogMessage(State.INFO, "Socket type: " + socket.SocketType, "HttpServer.StartListen"));
                if (socket.Connected)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(Connection));
                    thread.Start(socket); // Can your fancy Java do this?
                }
            }
        }

        /// <summary>
        /// Handles connections.
        /// </summary>
        /// <param name="aSocket"></param>
        private void Connection(object aSocket)
        {
            Socket socket = aSocket as Socket;
            int count = 0;
            string physicalFilePath = string.Empty;
            string formattedMessage = string.Empty;
            string response = string.Empty;
            int startPos = 0;
            string request;
            string dirName;
            string requestedFile;
            string errorMessage;
            string localDir;
            string buffer = string.Empty;
            string root = this.RootDirectory;

            while (count < 20)
            {
                if (!socket.Connected)
                {
                    Console.WriteLine("Disconnected...");
                    return;
                }
            
                if (root[root.Length - 1] != Path.DirectorySeparatorChar)
                {
                    root = root + Path.DirectorySeparatorChar;
                }

                byte[] receive = new byte[1024];
                int i = -1;
                
                // Assign a timeout to make sure it doesn't take forever for the socket to connect.
                socket.ReceiveTimeout = 2000;
                try
                {                                 
                    i = socket.Receive(receive, receive.Length, 0);
                    Console.WriteLine("Receiving...");
                }
                catch (Exception e)
                {               
                    //LogInformation(new LogMessage(State.ERROR, "Socket timeout. " + e.Message, "HttpServer.Connection"));
                    //socket.Close();
                    //return;
                }

                buffer = Encoding.ASCII.GetString(receive);
                if (!String.IsNullOrEmpty(buffer))
                {
                    HttpRequest httpRequest = new HttpRequest(buffer);
                    startPos = buffer.IndexOf("HTTP", 1);
                    if (startPos >= 0)
                    {
                        Console.WriteLine("Buffer: {0}", buffer);
                        if (buffer.Substring(0, 3) != "GET")
                        {
                            LogInformation(new LogMessage(State.ERROR, "Not in GET format... " + buffer.ToString(), "HttpServer.StartListen"));
                        }

                        string httpVersion = buffer.Substring(startPos, 8);
                        request = buffer.Substring(0, startPos - 1);
                        request.Replace("\\", "/");
                        if ((request.IndexOf(".") < 1) && (!request.EndsWith("/")))
                        {
                            request = request + "/";
                        }

                        startPos = request.LastIndexOf("/") + 1;
                        requestedFile = request.Substring(startPos);
                        dirName = request.Substring(request.IndexOf("/"), request.LastIndexOf("/") - 3);
                        if (dirName.Equals("/"))
                        {
                            localDir = root;
                        }
                        else
                        {
                            localDir = string.Empty;
                        }

                        Console.WriteLine("Directory: " + localDir);

                        // Invalid directory.
                        if (localDir.Length == 0)
                        {
                            errorMessage = "<H2>Error!! You didn't request anything...<BR>Scooby Doo and the gang can't really help you out ... <BR>I don't think anyone can help you with your level of stupidity.</H2><BR>";
                            SendHeader(httpVersion, string.Empty, errorMessage.Length, "HTTP 404 Not Found", ref socket);
                            SendData(errorMessage, ref socket);
                            socket.Close();
                            break;
                        }

                        // Check to make sure we have a valid file name.
                        // Sends a 404 Not found error.
                        if (requestedFile.Length == 0)
                        {
                            if (string.IsNullOrEmpty(requestedFile))
                            {
                                errorMessage = "<H2>The page " + requestedFile + " was not found...<BR>Don't worry, Scooby Doo and the gang are on the case!</H2><BR>";
                                SendHeader(httpVersion, string.Empty, errorMessage.Length, " 404 Not Found", ref socket);
                                SendData(errorMessage, ref socket);
                                socket.Close();
                                break;
                            }
                        }

                        string mimeType = GetMimeType(requestedFile);
                        physicalFilePath = localDir + requestedFile;
                        if (File.Exists(physicalFilePath) == false)
                        {
                            errorMessage = "<H2>The page " + requestedFile + " was not found...<BR>Don't worry, Scooby Doo and the gang are on the case!</H2><BR>";
                            if (File.Exists(RootDirectory + "404.html"))
                            {
                                errorMessage = File.ReadAllText(RootDirectory + "404.html");
                            }

                            SendHeader(httpVersion, string.Empty, errorMessage.Length, " 404 Not Found", ref socket);
                            SendData(errorMessage, ref socket);
                        }
                        else
                        {
                            int toBytes = 0;
                            response = string.Empty;
                            FileStream stream = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            FileInfo fi = new FileInfo(physicalFilePath);
                            DateTime lastWriteTime = fi.LastWriteTime;
                            BinaryReader reader = new BinaryReader(stream);
                            byte[] bytes = new byte[stream.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                Console.WriteLine("Reading some bytes...");
                                response = response + Encoding.ASCII.GetString(bytes, 0, read);
                                toBytes = toBytes + read;
                            }

                            reader.Close();
                            stream.Close();
                            errorMessage = "200 OK";
                            SendHeader(httpVersion, mimeType, toBytes, " 200 OK", ref socket);
                            SendData(bytes, ref socket);
                            //socket.Close();
                        }

                        startPos = buffer.IndexOf("Connection");
                        if (buffer.Contains("Close") || buffer.Contains("close"))
                        {
                            socket.Close();
                            Console.WriteLine("Closing the connection, because the browser told us so.");
                        }
                        else
                        {
                            Console.WriteLine("Keeping the connection open, because das browser told me so.");
                        }
                    }
                    else
                    {
                    }
                }

                Debug.WriteLine("Thread {0} is going to sleep...{1}", Thread.CurrentThread.ManagedThreadId, count);
                Thread.Sleep(1000);
                count++;
            }        
        }

        /// <summary>
        /// Sends the header to our socket.
        /// Calls the more complex SendHeader function which includes a lastWriteTime.
        /// </summary>
        /// <param name="httpVersion">HTTP version we are targetting.</param>
        /// <param name="mimeHeader">Header for the MIME format.</param>
        /// <param name="toBytes">Bytes ... as an int</param>
        /// <param name="statusCode">Code to respond to the server: 404, 304, etc.</param>
        /// <param name="socket">The socket used to send the response.</param>
        private void SendHeader(string httpVersion, string mimeHeader, int toBytes, string statusCode, ref Socket socket)
        {
            SendHeader(httpVersion, mimeHeader, toBytes, statusCode, ref socket, DateTime.Now);
        }

        /// <summary>
        /// Sends the header to our socket.
        /// </summary>
        /// <param name="httpVersion">HTTP Version we are using</param>
        /// <param name="mimeHeader">Mime header</param>
        /// <param name="toBytes"></param>
        /// <param name="statusCode">Status code we are sending</param>
        /// <param name="socket">Socket to the receiver</param>
        private void SendHeader(string httpVersion, string mimeHeader, int toBytes, string statusCode, ref Socket socket, DateTime lastWriteTime)
        {
            string buffer = string.Empty;
            if (mimeHeader.Length == 0)
            {
                mimeHeader = "text/html";
            }

            buffer = buffer + httpVersion + statusCode + "\r\n";
            buffer = buffer + "Server: WQN1010\r\n";
            buffer = buffer + "Content-Type: " + mimeHeader + "\r\n";
            buffer = buffer + "Accept-Ranges: bytes\r\n";
            DateTime time = DateTime.Now;
            string format = "ddd, d MMM yyyy HH:mm:ss";
            Console.WriteLine(time.ToString(format));
            buffer = buffer + "Date: " + time.ToString(format) + "\r\n";
            buffer = buffer + "Last-Modified: " + lastWriteTime.ToString(format) + "\r\n";
            buffer = buffer + "Content-Length: " + toBytes + "\r\n\r\n";
            byte[] data = Encoding.ASCII.GetBytes(buffer);
            SendData(data, ref socket);

            // Set up and write a log message about the header we just sent.
            LogMessage message = new LogMessage();
            message.Message = "Sending header...";
            message.Status = State.INFO;
            message.Header = buffer;
            message.Method = "HttpServer.SendHeader";
            LogInformation(message);
        }

        /// <summary>
        /// Gets the type of MIME we are using for the request.
        ///   - Searches our Hashtable for the requested file's extension.
        /// </summary>
        /// <param name="requestedFile"></param>
        /// <returns></returns>
        private string GetMimeType(string requestedFile)
        {
            string fileExt = string.Empty;
            requestedFile = requestedFile.ToLower();
            int startPos = requestedFile.IndexOf(".");
            fileExt = requestedFile.Substring(startPos);
            if (MimeTypes.Contains(fileExt))
            {
                return MimeTypes[fileExt].ToString();
            }
            else
            {
                LogMessage message = new LogMessage();
                message.Status = State.ERROR;
                message.Message = "Unknown MIME type, or not supported.";
                LogInformation(message);
            }

            return string.Empty;
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