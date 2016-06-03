using UnityEngine;

namespace Error
{
  public static class Assert
  {
    public static void Fail(string message)
    {
      Logger.LogError(message);
      if(Application.isEditor)
      { Debug.Break(); }
      else
      { Application.Quit(); }
    }

    public static void Invariant(bool test, string message)
    {
      if(!test)
      { Fail(message); }
    }
  }
}
