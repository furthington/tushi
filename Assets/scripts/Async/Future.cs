using System;

namespace Async
{
  public class Future<T>
  {
    private readonly System.Threading.Thread thread;
    private readonly System.Threading.ManualResetEvent reset_event;
    private T data;
    private bool is_ready;
    private bool raised_error = true;

    public Future(Func<T> func)
    {
      reset_event = new System.Threading.ManualResetEvent(false);
      thread = new System.Threading.Thread
      (
        () =>
        {
          try
          {
            data = func();
            raised_error = false;
          }
          finally
          {
            reset_event.Set();
            is_ready = true;
          }
        }
      );
      thread.Start();
    }

    public Future<bool> Then(Action<T> next_func)
    { return new Future<bool>(() => { next_func(Value); return true; }); }
    public Future<R> Then<R>(Func<T, R> next_func)
    { return new Future<R>(() => next_func(Value)); }

    public bool IsReady
    {
      get
      { return is_ready; }
    }
    public bool RaisedErrorj
    {
      get
      { return raised_error; }
    }
    public T Value
    {
      get
      {
        reset_event.WaitOne();
        return data;
      }
    }
  }
}
