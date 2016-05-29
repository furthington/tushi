using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Profile
{
  public class TaskTimer : IDisposable
  {
    private const string Symbol = "ENABLE_TIMER";
    private string name;
    private Stopwatch watch;

    public TaskTimer(string name)
    { Start(name); }

    public TaskTimer()
    { Start(); }

    public void Dispose()
    { Stop(); }

    [System.Diagnostics.Conditional(Symbol)]
    private void Start()
    {
      var trace = new StackTrace();
      Start(trace.GetFrame(1).GetMethod().Name);
    }

    [System.Diagnostics.Conditional(Symbol)]
    private void Start(string n)
    {
      name = n;
      watch = new Stopwatch();
      watch.Start();
    }

    [System.Diagnostics.Conditional(Symbol)]
    private void Stop()
    {
      watch.Stop();
      Logger.Log
      ("TaskTimer (" + name + "): " + watch.ElapsedMilliseconds + "ms");
    }
  }
}
