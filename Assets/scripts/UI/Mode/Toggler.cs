using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace UI.Mode
{
  public class Toggler : MonoBehaviour
  {
    public Button left; /* Assign in editor. */
    public Button right; /* Assign in editor. */

    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add<ReadReply>
      (
        r =>
        {
          right.gameObject.SetActive(r.Right);
          left.gameObject.SetActive(!r.Right);
          subscriptions.Clear();
        }
      );
      Pool.Dispatch(new Read());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void SetMode(bool r)
    { Pool.Dispatch(new Write(r)); }
  }
}
