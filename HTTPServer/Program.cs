using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer
{
    public class Program
    {
        private static int port = 8080;
        private static string logFile = string.Empty;
        private static string docRoot = string.Empty;

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                port = 8080;
                logFile = null;
                docRoot = Directory.GetCurrentDirectory();
            }

            if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "-docRoot":
                        docRoot = args[1];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[1], out port))
                        {
                            Console.WriteLine("Invalid port: {0}", port);
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[1];
                        break;
                }
            }
            else if (args.Length == 4)
            {
                switch (args[0])
                {
                    case "-docRoot":
                        docRoot = args[1];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[1], out port))
                        {
                            Console.WriteLine("Invalid port.");
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[1];
                        break;
                }

                switch (args[2])
                {
                    case "-docRoot":
                        docRoot = args[3];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[3], out port))
                        {
                            Console.WriteLine("Invalid port.");
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[3];
                        break;
                }
            }
            else if (args.Length == 6)
            {
                switch (args[0])
                {
                    case "-docRoot":
                        docRoot = args[1];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[1], out port))
                        {
                            Console.WriteLine("Invalid port.");
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[1];
                        break;
                }

                switch (args[2])
                {
                    case "-docRoot":
                        docRoot = args[3];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[3], out port))
                        {
                            Console.WriteLine("Invalid port.");
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[3];
                        break;
                }

                switch (args[4])
                {
                    case "-docRoot":
                        docRoot = args[5];
                        break;
                    case "-p":
                        if (!Int32.TryParse(args[5], out port))
                        {
                            Console.WriteLine("Invalid port.");
                            Console.ReadKey();
                            return;
                        }

                        break;
                    case "-logFile":
                        logFile = args[5];
                        break;
                }
            }
            else
            {
                Console.WriteLine("Incorrect number of arguments.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(logFile))
            {
                HttpServer server = new HttpServer(port);
            }
            else
            {
                HttpServer server = new HttpServer("log.txt", port);
            }
        }

        private static void SetData(string field, string[] args)
        {
            switch (args[0])
            {
                case "-docRoot":
                    docRoot = args[1];
                    break;
                case "-p":
                    if (!Int32.TryParse(args[1], out port))
                    {
                        Console.WriteLine("Invalid port.");
                        Console.ReadKey();
                        return;
                    }

                    break;
                case "-logFile":
                    logFile = args[1];
                    break;
            }
        }
    }
}
