using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace HTTPServer
{
    /// <summary>
    /// Emulates an HttpRequest.
    /// </summary>
    public class HttpRequest
    {
        public string Accept { get; set; }

        public string Connection { get; set; }
        
        public string IfModifiedSince { get; set; }

        public string HttpVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class.
        /// </summary>
        /// <param name="header">The request header we want to parse.</param>
        public HttpRequest(string header)
        {
            string[] parts = header.Split('\r', '\n');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Contains(':') && parts[i].Split(':')[0].Equals("If-Modified-Since"))
                {
                    IfModifiedSince = parts[i].Split(':')[1].Trim();
                }
                else if (parts[i].Contains(':') && parts[i].Split(':')[0].Equals("Accept"))
                {
                    Accept = parts[i].Split(':')[1].Trim();
                }
                else if (parts[i].Contains(':') && parts[i].Split(':')[0].Equals("Connection"))
                {
                    Connection = parts[i].Split(':')[1].Trim();
                    Debug.WriteLine("Boom! Connection: {0}", Connection);
                }
            }
        }
    }
}
