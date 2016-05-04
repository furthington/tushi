using System;
using System.Collections.Generic;

namespace Notification
{
  public static class Pool
  {
    private static List<Action> queue = new List<Action>();

    public static Subscription<T> Subscribe<T>(Action<T> action)
    {
      lock(queue)
      { return Dispatcher<T>.Subscribe(action); }
    }

    public static Subscription<T> PreSubscribe<T>(Action<T> action)
    {
      lock(queue)
      { return Dispatcher<T>.PreSubscribe(action); }
    }

    public static Subscription<T> PostSubscribe<T>(Action<T> action)
    {
      lock(queue)
      { return Dispatcher<T>.PostSubscribe(action); }
    }

    public static Subscription<T> Intercept<T>(Predicate<T> action)
    {
      lock(queue)
      { return Dispatcher<T>.Intercept(action); }
    }

    public static void Unsubscribe<T>(Subscription<T> id)
    {
      lock(queue)
      { Dispatcher<T>.Unsubscribe(id); }
    }

    public static void Dispatch<T>(T data)
    {
      lock(queue)
      { queue.Add(() => Dispatcher<T>.Dispatch(data)); }

      /* XXX: Enable for synchronous mode. */
      //Logger.Log("Dispatching " + data);
      //PumpAll();
    }

    public static Chain DispatchChain<T>(T data)
    {
      var chain = new Chain(() => Dispatcher<T>.Dispatch(data));
      lock(queue)
      { queue.Add(chain.Call); }
      return chain;
    }
    public static void DispatchSync<T>(T data)
    {
      lock(queue)
      { Dispatcher<T>.Dispatch(data); }
    }

    public static bool Pump()
    {
      bool handled = false;
      Action action = () => {}; /* Does nothing, by default. */
      lock(queue)
      {
        if(queue.Count > 0)
        {
          action = queue[0];
          queue.RemoveAt(0);
          handled = true;
        }
      }
      action();
      return handled;
    }

    public static void PumpAll()
    { while(Pump()); }
  }
}
