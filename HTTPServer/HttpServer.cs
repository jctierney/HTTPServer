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
        /// The directory where requested files should come from.
        /// </summary>
        private string Directory { get; set; }

        /// <summary>
        /// Log.
        /// </summary>
        private Logger Log { get; set; }

        /// <summary>
        /// Our listener.
        /// </summary>
        private TcpListener Listener { get; set; }

        /// <summary>
        /// Port number the server runs on.
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// Our default MIME types we support.
        /// </summary>
        private Hashtable MimeTypes { get; set; }

        private Hashtable Responses { get; set; }

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
            Port = port;
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

            // Create our MIME types Hashtable.
            MimeTypes = new Hashtable();
            MimeTypes.Add(".html", "text/html"); // HTML files
            MimeTypes.Add(".htm", "text/html"); // HTM files
            MimeTypes.Add(".bmp", "image/bmp"); // Bitmaps
            MimeTypes.Add(".jpg", "image/jpg"); // JPEG            
            try
            {
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                Console.WriteLine("Server is running..." + Port);

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
        ///   - TODO: currently defaults to C:\\www\\ directory.
        ///     Should make this non-static.
        /// </summary>
        private void StartListen()
        {
            int startPos = 0;
            string request;
            string dirName;
            string requestedFile;
            string errorMessage;
            string localDir;
            string root = "C:\\www\\"; // Default root directory.
            string physicalFilePath = string.Empty;
            string formattedMessage = string.Empty;
            string response = string.Empty;

            // Keep it open, brah.
            while (true)
            {
                Console.WriteLine("Doing some stuff, homies...");
                Socket socket = Listener.AcceptSocket();
                LogInformation(new LogMessage(State.INFO, "Socket type: " + socket.SocketType, "HttpServer.StartListen"));
                if (socket.Connected)
                {
                    byte[] receive = new byte[1024];
                    int i = -1;
                    try
                    {
                        i = socket.Receive(receive, receive.Length, 0);
                    }
                    catch (Exception e)
                    {
                        LogInformation(new LogMessage(State.ERROR, "Socket error. " + e.Message, "HttpServer.StartListen"));
                    }

                    string buffer = Encoding.ASCII.GetString(receive);
                    if (buffer.Substring(0, 3) != "GET")
                    {
                        LogInformation(new LogMessage(State.ERROR, "Not in GET format... " + buffer.ToString(), "HttpServer.StartListen"));
                        socket.Close();
                        continue;
                    }

                    startPos = buffer.IndexOf("HTTP", 1);
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

                    if (localDir.Length == 0)
                    {
                        errorMessage = "<H2>Error!! Requested directory does not exist...</H2><BR>";
                        SendHeader(httpVersion, string.Empty, errorMessage.Length, "HTTP 404 Not Found", ref socket);
                        SendData(errorMessage, ref socket);
                        socket.Close();
                        continue;
                    }

                    // Check to make sure we have a valid file name.
                    if (requestedFile.Length == 0)
                    {
                        if (string.IsNullOrEmpty(requestedFile))
                        {
                            errorMessage = "<H2>Error, no default file name specified.</H2>";
                            SendHeader(httpVersion, string.Empty, errorMessage.Length, " 404 Not Found", ref socket);
                            SendData(errorMessage, ref socket);
                            socket.Close();
                            continue;
                        }
                    }

                    string mimeType = GetMimeType(requestedFile);
                    physicalFilePath = localDir + requestedFile;
                    if (File.Exists(physicalFilePath) == false)
                    {
                        errorMessage = "<H2>404 Error! File does not exist...</H2>";
                        SendHeader(httpVersion, string.Empty, errorMessage.Length, " 404 Not Found", ref socket);
                        SendData(errorMessage, ref socket);
                    }
                    else
                    {
                        int toBytes = 0;
                        response = string.Empty;
                        FileStream stream = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        BinaryReader reader = new BinaryReader(stream);
                        byte[] bytes = new byte[stream.Length];
                        int read;
                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            response = response + Encoding.ASCII.GetString(bytes, 0, read);
                            toBytes = toBytes + read;
                        }

                        reader.Close();
                        stream.Close();
                        SendHeader(httpVersion, mimeType, toBytes, " 200 OK", ref socket);
                        SendData(bytes, ref socket);
                        socket.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Sends the header to our TCPSocket.
        /// </summary>
        /// <param name="httpVersion">HTTP Version we are using</param>
        /// <param name="mimeHeader">Mime header</param>
        /// <param name="toBytes"></param>
        /// <param name="statusCode">Status code we are sending</param>
        /// <param name="socket">Socket to the receiver</param>
        private void SendHeader(string httpVersion, string mimeHeader, int toBytes, string statusCode, ref Socket socket)
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
            buffer = buffer + "Content-Length: " + toBytes + "\r\n\r\n";
            byte[] data = Encoding.ASCII.GetBytes(buffer);
            SendData(data, ref socket);
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
                Console.WriteLine("Invalid MIME type: " + fileExt);
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