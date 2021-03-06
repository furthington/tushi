using UnityEngine;

// http://forum.unity3d.com/threads/strip-release-build-from-all-debug-log-calls.353600/#post-2292960
public sealed class Logger
{
  public const string Symbol = "ENABLE_LOG";

  [System.Diagnostics.Conditional(Symbol)]
  public static void Log(object message)
  { Debug.Log(message); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void Log(string message, params object[] args)
  { Debug.LogFormat(message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogFormat(string message, params object[] args)
  { Debug.LogFormat(message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogFormat(Object context, string message, params object[] args)
  { Debug.LogFormat(context, message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogWarning(object message)
  { Debug.LogWarning(message); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogWarning(object message, Object context)
  { Debug.LogWarning(message, context); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogWarningFormat(string message, params object[] args)
  { Debug.LogWarningFormat(message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogWarningFormat(Object context, string message, params object[] args)
  { Debug.LogWarningFormat(context, message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogError(object message)
  { Debug.LogError(message); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogError(object message, Object context)
  { Debug.LogError(message, context); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogErrorFormat(string message, params object[] args)
  { Debug.LogErrorFormat(message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogErrorFormat(Object context, string message, params object[] args)
  { Debug.LogErrorFormat(context, message, args); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogException(System.Exception exception)
  { Debug.LogException(exception); }

  [System.Diagnostics.Conditional(Symbol)]
  public static void LogException(System.Exception exception, Object context)
  { Debug.LogException(exception, context); }
}
