using System;
using System.Text;
using Engine;
using UnityEngine;

namespace manager
{
    public static class CleanLogger
    {
        public static void Log(string msg)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", msg);
        }

        public static void Error(Exception e)
        {
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "{0}\n{1}", e.Message, e.StackTrace);
        }

        public static void Error(string msg)
        {
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "{0}", msg);
        }
    }

    public class AppendLogger : IEngineLogger
    {
        private readonly StringBuilder _builder = new();

        public void Log(string msg)
        {
            _builder.AppendLine(msg);
        }

        public void PrintLogs()
        {
            CleanLogger.Log(_builder.ToString());
            _builder.Clear();
        }
    }
}