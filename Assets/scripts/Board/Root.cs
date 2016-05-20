using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Notification;

namespace Board
{
  public class RotateNeighbours { }
  public class PrintDebug { }
  public class ActiveTileRequest
  {
    public GameObject Requestor
    { get; set; }

    public ActiveTileRequest(GameObject r)
    { Requestor = r; }
  }
  public class ActiveTileReply
  {
    public Tile Active
    { get; set; }
    public ActiveTileRequest Request
    { get; set; }

    public ActiveTileReply(Tile t, ActiveTileRequest r)
    {
      Active = t;
      Request = r;
    }
  }

  [RequireComponent (typeof(Neighbour))]
  public class Root : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<Tile> active = new List<Tile>();
    private List<Tile> new_active = new List<Tile>();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<RotateNeighbours>(_ => GetComponent<Neighbour>().Rotate()));
      subscriptions.Add
      (Pool.Subscribe<RotateNeighbours>(_ => FindPlacement()));
      subscriptions.Add
      (Pool.Subscribe<ActiveTileReply>(StoreActiveTile));
      subscriptions.Add
      (Pool.Subscribe<NeighbourReply>(StoreNeighbor));
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

    private void FindPlacement()
    { StartCoroutine(FindPlacementAsync()); }

    private IEnumerator FindPlacementAsync()
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
      active.Clear();
      Pool.Dispatch(new ActiveTileRequest(gameObject));
      yield return Notification.Coroutine.WaitForReplies<ActiveTileRequest>
      (n => n.Requestor == gameObject);

      if(active.IsEmpty())
      { return; } // TODO: Notif?
      else if(active.Count > 30) // TODO: Calculate
      { return; }

      // TODO: For each rotation
      var active_subs = new SubscriptionStack();
      foreach(var t in active)
      {
        active_subs.Add
        (Pool.Subscribe<NeighbourRequest>(n => t.ReportNeighbour(n)));
      }

      var neighbours = GetComponent<Neighbour>();
      for(int i = 0; i < neighbours.neighbours.Count; ++i)
      {
        // dispatch NeighbourRequest(this, neighbour_relationship)
        // yield WaitForNotification<Post<NeighbourRequest>>(n => n.obj == this)
        // if no responses
        //   break
        // responses become new active tiles; resubscribe them
        // reset state
        var n = neighbours.neighbours[i];
        if(n == null)
        { continue; }

        new_active.Clear();
        Pool.Dispatch
        (new NeighbourRequest(gameObject, (NeighbourRelationship)i));
        yield return Notification.Coroutine.WaitForReplies<NeighbourRequest>
        (n => n.Requestor == gameObject);

        if(new_active.IsEmpty())
        { continue; }

        active = new_active;
        new_active.Clear();
      }
    }

    private void StoreActiveTile(ActiveTileReply r)
    {
      if(r.Request.Requestor != gameObject)
      { return; }
      active.Add(r.Active);
    }

    private void StoreNeighbor(NeighbourReply r)
    {
      if(r.Request.Requestor != gameObject)
      { return; }
      new_active.Add(r.Neighbour);
    }
  }
}
