using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    /// <summary>
    /// The LogMessage class used to create a new message
    /// for a log.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Status:
        ///     - Error
        ///     - Normal
        /// </summary>
        public State Status { get; set; }

        /// <summary>
        /// Message created for the logger.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Class that called the logger.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Class that called the logger
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Initializes a new instance of the LogMessage class with
        /// default values.
        /// </summary>
        public LogMessage()
            : this(State.INFO, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogMessage class with
        /// specified values for Status, Message, and Method.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="method"></param>
        public LogMessage(State status, string message, string method)
            : this(status, string.Empty, message, method)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the LogMessage class with
        /// specified values for Status, Header, Message, and Method.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <param name="method"></param>
        public LogMessage(State status, string header, string message, string method)
        {
            Status = status;
            Header = header;
            Message = message;
            Method = method;
        }
    }

    /// <summary>
    /// Enumeration for the state of the log message.
    /// </summary>
    public enum State
    {
        INFO = 0,
        ERROR = 1
    }
}
