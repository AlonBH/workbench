using System;
using System.Diagnostics;
using Caliburn.Micro;

namespace Workbench.Loggers
{
    internal class DebugLogger : ILog
    {
        private readonly Type _type;
    
        public DebugLogger(Type type)
        {
            _type = type;
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARN");
        }

        private static string CreateLogMessage(string format, params object[] args)
        {
            return $"[{DateTime.Now.ToString("o")}] {string.Format(format, args)}";
        }
    }
}
