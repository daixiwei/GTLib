namespace com.gt.units
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 
        /// </summary>
        private static LogLevel loggingLevel = LogLevel.INFO|LogLevel.ERROR|LogLevel.WARN;
        /// <summary>
        /// 
        /// </summary>
        private string loggerName = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerName"></param>
        public Logger(string loggerName)
        {
            this.loggerName = loggerName;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classz"></param>
        public Logger( Type classz)
        {
            //this.classz = classz;
            if (classz != null)
            {
                loggerName = classz.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void Debug(params string[] messages)
        {
            this.Log(LogLevel.DEBUG, string.Join(" ", messages));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void Error(params string[] messages)
        {
            this.Log(LogLevel.ERROR, string.Join(" ", messages));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void Info(params string[] messages)
        {
            this.Log(LogLevel.INFO, string.Join(" ", messages));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        protected virtual void Log(LogLevel level, string message)
        {
            if ((level & loggingLevel)!=0)
            {
                GTLib.GameManager.Print(level, TypeName, message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void Warn(params string[] messages)
        {
            this.Log(LogLevel.WARN, string.Join(" ", messages));
        }

        /// <summary>
        /// 
        /// </summary>
        public static LogLevel LoggingLevel
        {
            get
            {
                return loggingLevel;
            }
            set
            {
                loggingLevel = value;
            }
        }

        /// <value>
        /// The name of the type.
        /// </value>
        private string TypeName
        {
            get
            {
                return loggerName;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The debug
        /// </summary>
        DEBUG = 2,
        /// <summary>
        /// The error
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// The info
        /// </summary>
        INFO = 8,
        /// <summary>
        /// The warn
        /// </summary>
        WARN = 16,
        /// <summary>
        /// The All
        /// </summary>
        ALL = 0xFFFFFF
    }
}

