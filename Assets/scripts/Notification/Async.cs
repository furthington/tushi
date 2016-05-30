using UnityEngine;
using System;
using System.Threading;
using System.Collections;

namespace Notification
{
  public class RepliesComplete<T>
  {
    public int ID
    { get; set; }

    public RepliesComplete(int id)
    { ID = id; }
  };

  public static class Async
  {
    /* XXX: Thread-safe. */
    private static int counter = 0;

    public static IEnumerator WaitFor<T>()
    { yield return WaitFor<T>(_ => true); }

    public static IEnumerator WaitFor<T>(Predicate<T> filter)
    {
      var found = false;
      var sub = Pool.PostSubscribe<T>(n =>
      {
        if(filter(n))
        { found = true; }
      });
      while(!found)
      { yield return new WaitForEndOfFrame(); }
      Pool.Unsubscribe(sub);
    }

    public static IEnumerator WaitForReplies<T>()
    { yield return WaitForReplies<T>(_ => true); }

    public static IEnumerator WaitForReplies<T>(Predicate<T> filter)
    {
      //Logger.Log("Waiting for {0}", typeof(T).Name);
      yield return WaitFor<T>(filter);

      //Logger.Log("Waiting for replies for {0}", typeof(T).Name);
      var id = Interlocked.Increment(ref counter);
      Pool.Dispatch(new RepliesComplete<T>(id));
      yield return WaitFor<RepliesComplete<T>>(n => n.ID == id);
      //Logger.Log("Done waiting on {0}", typeof(T).Name);
    }
  }
}
