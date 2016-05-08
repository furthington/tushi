using UnityEngine;
using Notification;
using System.IO;
using System.Collections.Generic;

namespace Save
{
  public struct GameExists
  { }

  public class Loader : MonoBehaviour
  {
    private void Start()
    {
      if(File.Exists(Path()))
      { Pool.Dispatch(new GameExists()); }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
