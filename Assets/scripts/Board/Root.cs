using UnityEngine;
using Notification;

namespace Board
{
  public class RotateNeighbours { }
  public class PrintDebug { }

  public class Root : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<RotateNeighbours>(_ => GetComponent<Neighbour>().Rotate()));
      subscriptions.Add
      (
        Pool.Subscribe<PrintDebug>
        (_ =>
          {
            Logger.Log("Printing from root:");
            GetComponent<Neighbour>().PrintDebug();
          }
        )
      );
    }

    public void ClearSubscriptions()
    { subscriptions.Clear(); }

    private void OnDisable()
    { subscriptions.Clear(); }
  }
}
