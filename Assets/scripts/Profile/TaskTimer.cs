using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Profile
{
  public class TaskTimer : IDisposable
  {
    private string name = "generic";
    private Stopwatch watch;

    public TaskTimer(string name)
    {
#if ENABLE_LOG
      this.name = name;
      watch = new Stopwatch();
      watch.Start();
#endif
    }

    public TaskTimer()
    {
#if ENABLE_LOG
      var trace = new StackTrace();
      name = trace.GetFrame(1).GetMethod().Name;
      watch = new Stopwatch();
      watch.Start();
#endif
    }

    public void Dispose()
    {
#if ENABLE_LOG
      watch.Stop();
      Logger.Log
      ("TaskTimer (" + name + "): " + watch.ElapsedMilliseconds + "ms");
#endif
    }
  }
}