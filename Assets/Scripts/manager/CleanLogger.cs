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
}