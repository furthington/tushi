using UnityEngine;
using UnityEngine.EventSystems;
using Notification;

namespace Board
{
  public class RotatePieces
  { }

  [RequireComponent (typeof(Animator))]
  public class Rotater : MonoBehaviour, IPointerClickHandler
  {
    SubscriptionStack subscriptions = new SubscriptionStack();
    Animator animator;
    bool animating = false;
    float target_rotation;

    private void Start()
    {
      subscriptions.Add<RotatePieces>(_ => Rotate());
      animator = GetComponent<Animator>();
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void OnPointerClick(PointerEventData data)
    { Rotate(); }

    public void Rotate()
    {
      if (animating)
      { EndRotate(); }
      target_rotation = transform.eulerAngles.z - 60.0f;
      animator.SetTrigger("play");
      animating = true;
    }

    public void EndRotate()
    {
      animator.ResetTrigger("play");
      transform.rotation = Quaternion.Euler(0.0f, 0.0f, target_rotation);
      /*TODO: don't save 3 times when rotating all 3 pieces. */
      Notification.Pool.Dispatch(new Save.SaveGame());
      animating = false;
    }
  }
}
