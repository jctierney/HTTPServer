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
        public static void Main(string[] args)
        {
            HttpServer server = new HttpServer("log.txt", 8080);
        }
    }
}
