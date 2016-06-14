using UnityEngine;
using UnityEngine.EventSystems;

namespace Board
{
  public class Rotater : MonoBehaviour, IPointerClickHandler
  {
    public void OnPointerClick(PointerEventData data)
    { Rotate(); }

    public void Rotate()
    {
      transform.Rotate(new Vector3(0, 0, -60));
      Notification.Pool.Dispatch(new Save.SaveGame());
    }
  }
}
