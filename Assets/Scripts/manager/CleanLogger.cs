using System.Text;
using Engine;
using UnityEngine;

namespace manager
{
    public static class CleanLogger
    {
        public static void Log(string msg)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, msg);
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
        }
    }
}