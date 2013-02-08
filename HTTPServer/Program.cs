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
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            int port = 8080;
            string logFile;

            Console.WriteLine("What is the port: ");
            if (!Int32.TryParse(Console.ReadLine(), out port))
            {
                port = 8080;
            }

            Console.WriteLine("What is the log file: ");
            logFile = Console.ReadLine();
            if (string.IsNullOrEmpty(logFile))
            {
                HttpServer server = new HttpServer(8080);
            }
            else
            {
                HttpServer server = new HttpServer("log.txt", 8080);
            }
        }
    }
}
