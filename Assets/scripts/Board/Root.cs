using UnityEngine;
using Notification;

namespace Board
{
  public class RotateNeighbours { }
  public class PrintDebug { }

  [RequireComponent (typeof(Neighbour))]
  public class Root : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      // TODO:
      // listen for PiecePlaced
      // start a coroutine
      //   dispatch ActiveTileRequest(this)
      //   yield WaitForNotification<Post<ActiveTileRequest>>(n => n.obj == this)
      //   if fewer active than in piece
      //     break
      //   else if more than check threshold
      //     break
      //   for each rotation
      //     subscribe active tiles
      //     for each valid neighbour
      //       dispatch NeighbourRequest(this, neighbour_relationship)
      //       yield WaitForNotification<Post<NeighbourRequest>>(n => n.obj == this)
      //       if no responses
      //         break
      //       responses become new active tiles; resubscribe them
      //       reset state
      //     if active tiles remain
      //       valid position found
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
