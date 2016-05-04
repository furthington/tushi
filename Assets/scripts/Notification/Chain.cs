using System;
using System.Collections.Generic;

namespace Notification
{
  public class Chain
  {
    private readonly List<Action> actions = new List<Action>();

    public Chain(Action action)
    { actions.Add(action); }

    public Chain Then(Action action)
    {
      actions.Add(action);
      return this;
    }

    public void Call()
    {
      foreach(var a in actions)
      { a(); }
    }
  }
}
