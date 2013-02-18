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
			// make the server
			HttpServer server = new HttpServer();
			
			// parse the args
			FlagParser parser = new FlagParser();
			try
			{
				parser.ParseFlags(ref server, args);
			} 
			catch (Exception e)
			{
				Console.WriteLine("ERROR\n\t"+e.Message);
			}
			
			// start the server
			server.Initialize();
        }

        
    }
}
