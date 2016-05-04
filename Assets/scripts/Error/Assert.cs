using UnityEngine;

// TODO: Update asserts
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
  }
}
