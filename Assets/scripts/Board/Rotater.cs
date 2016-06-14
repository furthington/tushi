using UnityEngine;
using UnityEngine.EventSystems;
using Notification;

namespace Board
{
  public class RotatePieces
  { }

  public class Rotater : MonoBehaviour, IPointerClickHandler
  {
    SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    { subscriptions.Add<RotatePieces>(_ => Rotate()); }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void OnPointerClick(PointerEventData data)
    { Rotate(); }

    public void Rotate()
    {
      transform.Rotate(new Vector3(0, 0, -60));
      Notification.Pool.Dispatch(new Save.SaveGame());
    }
  }
}
