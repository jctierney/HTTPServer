using System;
using System.Collections.Generic;

namespace HTTPServer
{
  
	public delegate void FlagCallback(ref HttpServer server, string value);

	public class CLArguments
	{
		/// <summary>
		/// Command Line Flag List
		/// </summary>
		private Dictionary<string, CLFlag> Flags;


		/// <summary>
		/// Create and register new flags
		/// </summary>
		private void InitializeFlags()
		{
	
			// register the port flag
			registerFlag("-p", delegate(ref HttpServer server, string value) {
				int port = HttpServer.DEFAULT_PORT;
				Int32.TryParse(value, out port);
				server.Port = port;
			});
			  
			// register the document root flag
			registerFlag("-docroot", delegate(ref HttpServer server, string value) {
				if(value != null && new Uri(value).IsWellFormedOriginalString())
				{
					server.Directory = value;
				}
				else 
				{
					// THROW EXCEPTION

				}
			});


					
						
			
		}

		/// <summary>
		/// Registers a new flag.
		/// </summary>
		private void registerFlag(string flag, FlagCallback callback) {
			if (Flags.ContainsKey(flag))
			{
				Console.Error.WriteLine("Flag '" + flag + "' already exists, you're overriding it");
			}
			else
			{
				Flags.Add(flag, new CLFlag(flag, callback));
			}
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HTTPServer.CLArguments"/> class
		/// from command line arguments
		/// </summary>
		/// <param name="args">Arguments.</param>
		public CLArguments(string[] args)
		{
				Flags = new Dictionary<string, CLFlag>();
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
}

