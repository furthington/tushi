using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace UI.Guide
{
  public class Toggler : MonoBehaviour
  {
    public Button show; /* Assign in editor. */
    public Button hide; /* Assign in editor. */

    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add<ReadReply>
      (
        r =>
        {
          hide.gameObject.SetActive(r.Show);
          show.gameObject.SetActive(!r.Show);
          subscriptions.Clear();
        }
      );
      Pool.Dispatch(new Read());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void SetGuide(bool r)
    { Pool.Dispatch(new Write(r)); }
  }
}
