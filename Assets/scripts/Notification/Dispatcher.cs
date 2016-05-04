using System;
using System.Collections.Generic;

namespace Notification
{
  public static class Dispatcher<T>
  {
    private static int subscription_counter = 0;
    private static List<Subscribed<Action<T>, T>> pre = new List<Subscribed<Action<T>, T>>();
    private static List<Subscribed<Action<T>, T>> listeners = new List<Subscribed<Action<T>, T>>();
    private static List<Subscribed<Action<T>, T>> post = new List<Subscribed<Action<T>, T>>();
    private static List<Subscribed<Predicate<T>, T>> intercepters = new List<Subscribed<Predicate<T>, T>>();

    public static Subscription<T> Subscribe(Action<T> action)
    {
      listeners.Add
      (
        new Subscribed<Action<T>, T>
        (new Subscription<T>(++subscription_counter), action)
      );
      return new Subscription<T>(subscription_counter);
    }

    public static Subscription<T> PreSubscribe(Action<T> action)
    {
      pre.Add
      (
        new Subscribed<Action<T>, T>
        (new Subscription<T>(++subscription_counter), action)
      );
      return new Subscription<T>(subscription_counter);
    }

    public static Subscription<T> PostSubscribe(Action<T> action)
    {
      post.Add
      (
        new Subscribed<Action<T>, T>
        (new Subscription<T>(++subscription_counter), action)
      );
      return new Subscription<T>(subscription_counter);
    }

    public static Subscription<T> Intercept(Predicate<T> action)
    {
      intercepters.Add
      (
        new Subscribed<Predicate<T>, T>
        (new Subscription<T>(++subscription_counter), action)
      );
      return new Subscription<T>(subscription_counter);
    }

    public static void Unsubscribe(Subscription<T> id)
    {
      listeners.RemoveAll(t => t.subscription == id);
      pre.RemoveAll(t => t.subscription == id);
      intercepters.RemoveAll(t => t.subscription == id);
      post.RemoveAll(t => t.subscription == id);
    }

    public static void Dispatch(T data)
    {
      var intercept_shallow = new List<Subscribed<Predicate<T>, T>>(intercepters);
      foreach(var intercepter in intercept_shallow)
      {
        if(intercepter.action.Invoke(data))
        { return; }
      }

      var pre_shallow = new List<Subscribed<Action<T>, T>>(pre);
      foreach(var subscriber in pre_shallow)
      { subscriber.action.Invoke(data); }

      var shallow = new List<Subscribed<Action<T>, T>>(listeners);
      foreach(var subscriber in shallow)
      { subscriber.action.Invoke(data); }

      var post_shallow = new List<Subscribed<Action<T>, T>>(post);
      foreach(var subscriber in post_shallow)
      { subscriber.action.Invoke(data); }
    }
  }
}
