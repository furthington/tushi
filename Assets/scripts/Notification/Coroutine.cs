using UnityEngine;
using System;
using System.Collections;

namespace Notification
{
  public static class Coroutine
  {
    public static IEnumerator WaitFor<T>(Predicate<T> filter)
    {
      bool found = false;
      var sub = Pool.Subscribe<T>(n => found = filter(n));
      while(!found)
      { yield return new WaitForEndOfFrame(); }
      Pool.Unsubscribe(sub);
    }
  }
}
