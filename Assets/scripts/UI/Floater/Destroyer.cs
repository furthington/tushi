using UnityEngine;
using System.Collections;

namespace UI.Floater
{
  public class Destroyed
  { }

  public class Destroyer : MonoBehaviour
  {
    public void DestroyMe()
    {
      Notification.Pool.Dispatch(new Destroyed());
      Destroy(gameObject);
    }
  }
}
