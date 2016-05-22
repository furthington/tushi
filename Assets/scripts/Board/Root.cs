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
    {
      StopAllCoroutines();
      StartCoroutine(FindPlacementAsync());
    }

    private IEnumerator FindPlacementAsync()
    {
      Logger.Log("Finding placement");
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
      using(var timer = new Profile.TaskTimer("Active tile request"))
      {
        active.Clear();
        Logger.Log("Sending out request");
        Pool.Dispatch(new ActiveTileRequest(gameObject));
        Logger.Log("Waiting for replies");
        yield return Notification.Async.WaitForReplies<ActiveTileRequest>
        (n => { Logger.Log("Active filter"); return n.Requestor == gameObject; });
        Logger.Log("Back with all replies");
      }

      if(active.Count == 0) // TODO: Notif?
      {
        Logger.Log("Board is full...?");
        yield break;
      }
      else if(active.Count > 30) // TODO: Calculate
      {
        Logger.Log("Active count greater than threshold");
        yield break;
      }

      foreach(var t in active)
      { t.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 255, 0, 255); }
      yield return new WaitForSeconds(3);

      foreach(var t in active)
      { t.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0); }

      // TODO: For each rotation
      bool found = false;
      using(var timer = new Profile.TaskTimer("Neighbour walk"))
      {
        foreach(var act in active)
        {
          /* TODO: Coroutine. */
          var valid = Walk(GetComponent<Neighbour>(), act);
          if(valid)
          {
            Logger.Log("Found valid position for piece");
            found = true;
            break; /* TODO: Notif? */
          }
        }
      }
      if(!found)
      { Logger.Log("No piece found"); } 
    }

    private void StoreActiveTile(ActiveTileReply r)
    {
      if(r.Request.Requestor != gameObject)
      {
        Logger.Log("Got somone else's active reply");
        return; }
      Logger.Log("Got active reply");
      active.Add(r.Active);
    }

    private void StoreNeighbor(NeighbourReply r)
    {
      if(r.Request.Requestor != gameObject)
      { return; }
      new_active.Add(r.Neighbour);
    }

    private bool WalkImpl(Neighbour neighbour, Tile tile)
    {
      if(neighbour != null)
      {
        if(tile == null || tile.block != null)
        { return false; }
        return Walk(neighbour, tile);
      }
      return true;
    }

    private bool Walk(Neighbour neighbour, Tile tile)
    {
      /* We've hit a leaf. */
      if(neighbour == null && tile == null)
      { return true; }

      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.Right],
                   tile.right))
      { return false; }
      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.TopRight],
                   tile.top_right))
      { return false; }
      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.TopLeft],
                   tile.top_left))
      { return false; }
      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.Left],
                   tile.left))
      { return false; }
      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.BottomLeft],
                   tile.bottom_left))
      { return false; }
      if(!WalkImpl(neighbour.neighbours[(int)NeighbourRelationship.BottomRight],
                   tile.bottom_right))
      { return false; }

      return true;
    }
  }
}
