using System;
using System.IO;
using System.Collections.Generic;

namespace HTTPServer
{
    /// <summary>
    /// Flag callback.
    /// Function type called by <see cref="HTTPServer.CLArguments"/> class to modify a
    /// given instance of a <see cref="HTTPServer.HttpServer"/>.
    /// </summary>
    /// <param name="server">The Server to modify during execution</param>
    /// <param name="value">Value passed at the command line following the flag that
    /// this callback is registered to.</param>
    public delegate void FlagCallback(ref HttpServer server, string value);

    /// <summary>
    /// Flag parser.
    /// Modifies an HttpServer based on callbacks bound to command line flags
    /// </summary>
    public class FlagParser
    {
        /// <summary>
        /// Command Line Flag List
        /// </summary>
        private Dictionary<string, CLFlag> Flags { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HTTPServer.CLArguments"/> class
        /// from command line arguments
        /// </summary>
        /// <param name="args">Arguments.</param>
        public FlagParser()
        {
            Flags = new Dictionary<string, CLFlag>();
            InitializeFlags();
        }

        /// <summary>
        /// Parses the flags.
        /// </summary>
        /// <param name="server">Reference to Server to be modified by args.</param>
        /// <param name="args">arg list from Main.</param>
        public void ParseFlags(ref HttpServer server, string[] args)
        {
            Dictionary<string, string> usedFlags = new Dictionary<string, string>();

            Queue<string> queue = new Queue<string>(args);
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (Flags.ContainsKey(current))
                {
                    // Every flag expects a parameter, if there is no strings left to be the parameter
                    // of if the next string is a flag, throw an exception.                    
                    if (queue.Count == 0 || Flags.ContainsKey(queue.Peek()))
                    {
                        throw new MalformedFlagException("Flag: \"" + current + "\" expects an argument, recieved none");
                    }
                    else
                    {
                        string next = queue.Dequeue();
                        Flags[current].Callback(ref server, next);
                        if (usedFlags.ContainsKey(current))
                        {
                            Console.WriteLine("WARNING! flag: \"" + current + "\" declared multiple times, " +
                                               "happily overriding previous value of " + usedFlags[current] + " with " + next + ".");
                            usedFlags.Remove(current);
                        }

                        usedFlags.Add(current, next);
                    }
                }
                else
                {
                    throw new InvalidFlagException(current);
                }
            }
        }

        /// <summary>
        /// Create and register new flags
        /// </summary>
        private void InitializeFlags()
        {
            // register the port flag
            registerFlag("p", delegate(ref HttpServer server, string value)
            {
                int port = HttpServer.DEFAULT_PORT;
                bool successfulParse = Int32.TryParse(value, out port);
                if (successfulParse)
                {
                    server.Port = port;
                }
                else
                {
                    throw new MalformedFlagException("\"-p\" expects numeric port, recieved: " + value + ".");
                }
            });

            // register the document root flag
            registerFlag("docroot", delegate(ref HttpServer server, string value)
            {
                if (value != null &&
                   Directory.Exists(value))
                {
                    server.RootDirectory = value;
                }
                else
                {
                   throw new MalformedFlagException("\"-docroot\" expects a valid directory, recieved: " + value + ".");
                }
            });

            // register the logfile flag
            registerFlag("logfile", delegate(ref HttpServer server, string value)
            {
                // logfile must not to a filename inside a existing directory, file does not 
                // need to exist.
                if (value != null &&
                    Directory.Exists(Path.GetDirectoryName(value)) &&
                    Path.GetFileName(value) != "")
                {
                    server.Log = new Logger(value);
                }
                else if (value != null)
                {
                    server.Log = new Logger(value);
                }
                else
                {
                    throw new MalformedFlagException("\"-logfile\" expects a path to a file in an existing directory, " +
                                                     "received: " + value);
                }
            });

        }

        /// <summary>
        /// Registers a new flag.
        /// </summary>
        private void registerFlag(string flag, FlagCallback callback)
        {
            if (Flags.ContainsKey(flag))
            {
                Console.Error.WriteLine("Flag '" + flag + "' already exists, you're overriding it");
            }
            if (flag[0] == '-')
            {
                Console.Error.WriteLine("Attempting to register flag: " + flag + "with explicit preceding dash");
            }

            Flags.Add("-" + flag, new CLFlag(flag, callback));
        }

    }

    /// <summary>
    /// Command Line Flag action
    /// </summary>
    public class CLFlag
    {
        /// <summary>
        /// Gets the flag.
        /// </summary>
        /// <value>The flag.</value>
        public string Flag
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>The callback.</value>
        public FlagCallback Callback
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HTTPServer.CLFlag"/> class.
        /// </summary>
        /// <param name="flag">Flag.</param>
        /// <param name="callback">Callback.</param>
        public CLFlag(string flag, FlagCallback callback)
        {
            Flag = flag;
            Callback = callback;
        }
    }

    /// <summary>
    /// Invalid flag exception.
    /// </summary>
    public class InvalidFlagException : System.Exception
    {
        public InvalidFlagException(string flag)
            : base("Flag: " + flag + " is not a valid flag.")
        {
        }
    }

    /// <summary>
    /// Malformed flag exception.
    /// </summary>
    public class MalformedFlagException : System.Exception
    {
        public MalformedFlagException(string message)
            : base(message)
        {
        }
    }
}