using UnityEngine;

public static class Logger
{
  public static void Log(string msg)
  {
#if UNITY_EDITOR
    Debug.Log(msg);
#endif
  }

  public static void LogError(string msg)
  {
#if UNITY_EDITOR
    Debug.LogError(msg);
#endif
  }

  public static void LogFormat(string format, params object[] args)
  {
#if UNITY_EDITOR
    Debug.LogFormat(format, args);
#endif
  }
}
