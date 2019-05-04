using System;
using System.IO;
using System.Text;

namespace HydraX.Library
{
    public class HydraLogger : IDisposable
    {
        /// <summary>
        /// Message Types for logging
        /// </summary>
        public enum MessageType
        {
            INFO,
            WARNING,
            ERROR,
        }

        /// <summary>
        /// Log File Name
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Current Log Name
        /// </summary>
        private string LogName { get; set; }

        /// <summary>
        /// Active Stream
        /// </summary>
        private StringBuilder Buffer = new StringBuilder();

        /// <summary>
        /// Initiate Logger
        /// </summary>
        /// <param name="logName">Log Name</param>
        /// <param name="fileName">Log File Name</param>
        public HydraLogger(string logName, string fileName)
        {
            // Set properties
            LogFile = fileName;
            LogName = logName;

            // Write Initial 
            Log(LogName, MessageType.INFO);

            // Close
            Flush();
        }

        /// <summary>
        /// Writes a message to the log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public void Log(object message, MessageType messageType)
        {
            // Write to file
            lock (Buffer)
            {
                Buffer.AppendLine(string.Format("{0} [ {1} ] {2}", DateTime.Now.ToString("dd-MM-yyyy - HH:mm:ss"), messageType, message));
            }
        }

        /// <summary>
        /// Closes current Streamwriter
        /// </summary>
        public void Flush()
        {
            // Dump buffer
            using (var output = new StreamWriter(LogFile, true))
                output.Write(Buffer.ToString());
            // Reset
            Buffer = new StringBuilder();
        }

        /// <summary>
        /// Disposes of the Logger and its Streamwriter
        /// </summary>
        public void Dispose()
        {
            // Close stream
            Flush();
        }
    }
}
