using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
			TcpListener listener = new TcpListener(IPAddress.Any, 8080);
			listener.Start();
			TcpClient client = listener.AcceptTcpClient();
			StreamReader reader = new StreamReader(client.GetStream());
			StreamWriter writer = new StreamWriter(client.GetStream());
			String x = reader.ReadLine();
			int i = 1;
			while(!String.IsNullOrEmpty(x)) {
				Console.WriteLine(i.ToString()+ x);
				x = reader.ReadLine();
				i++;

			}

			writer.WriteLine("HTTP/1.0 404 Not Found\r\n");

        }
    }
}
