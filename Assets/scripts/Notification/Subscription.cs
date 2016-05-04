using System;
using System.Collections.Generic;

namespace Notification
{
  public struct Subscription<T>
  {
    public int ID
    { get; set; }

    public Subscription(int id)
    { ID = id; }

    public static bool operator ==(Subscription<T> a, Subscription<T> b)
    { return a.ID == b.ID; }
    public static bool operator !=(Subscription<T> a, Subscription<T> b)
    { return a.ID != b.ID; }

    public bool Equals(Subscription<T> s)
    { return s.ID == ID; }
    public override bool Equals(object o)
    { return Equals((Subscription<T>)o); }

    public override int GetHashCode()
    { return ID; }
  }

  public struct Subscribed<A, T>
  {
    public readonly Subscription<T> subscription;
    public readonly A action;

    public Subscribed(Subscription<T> s, A a)
    {
      subscription = s;
      action = a;
    }
  }

  public class SubscriptionStack
  {
    private readonly List<Action> unsubscribe = new List<Action>();

    public void Add<T>(Subscription<T> sub)
    { unsubscribe.Add(() => Pool.Unsubscribe(sub)); }

    public void Clear()
    {
      unsubscribe.ForEach(a => a());
      unsubscribe.Clear();
    }
  }
}
