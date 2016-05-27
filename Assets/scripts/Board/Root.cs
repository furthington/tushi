using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Notification;

namespace Board
{
  public class RotateNeighbours { }
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

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<RotateNeighbours>(_ => GetComponent<Neighbour>().Rotate()));
      subscriptions.Add
      (Pool.Subscribe<RotateNeighbours>(_ => FindPlacement()));
      subscriptions.Add
      (Pool.Subscribe<ActiveTileReply>(StoreActiveTile));
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
      using(var timer = new Profile.TaskTimer("Active tile request"))
      {
        active.Clear();
        /* TODO: Only do this once, not per piece. */
        Pool.Dispatch(new ActiveTileRequest(gameObject));
        yield return Notification.Async.WaitForReplies<ActiveTileRequest>
        (n => n.Requestor == gameObject);
      }

      if(active.Count == 0)
      {
        Logger.Log("Board is full...?");
        yield break; /* TODO: Notif? */
      }
      else if(active.Count > 30) /* TODO: Calculate */
      {
        Logger.Log("Active count greater than threshold");
        yield break; /* TODO: Notif? */
      }

      using(var timer = new Profile.TaskTimer("Neighbour walk"))
      {
        foreach(var rotation in GetComponent<Neighbour>().GetRotations())
        {
          foreach(var act in active)
          {
            /* TODO: Coroutine. */
            var valid = Walk(rotation, act);
            if(valid)
            {
              Logger.Log("Found valid position for piece");
              yield break; /* TODO: Notif? */
            }
          }
        }
      }
      Logger.Log("No piece found");
    }

    private void StoreActiveTile(ActiveTileReply r)
    {
      if(r.Request.Requestor != gameObject)
      { return; }
      active.Add(r.Active);
    }

    private bool WalkImpl(Neighbour neighbour, Tile tile)
    {
      if(neighbour != null)
      {
        if(tile == null || tile.block != null)
        { return false; }
        return Walk(neighbour.neighbours, tile);
      }
      return true;
    }

    private bool Walk(List<Neighbour> neighbour, Tile tile)
    {
      /* We've hit a leaf. */
      if(neighbour == null && tile == null)
      { return true; }

      if(!WalkImpl(neighbour[(int)NeighbourRelationship.Right],
                   tile.right))
      { return false; }
      if(!WalkImpl(neighbour[(int)NeighbourRelationship.TopRight],
                   tile.top_right))
      { return false; }
      if(!WalkImpl(neighbour[(int)NeighbourRelationship.TopLeft],
                   tile.top_left))
      { return false; }
      if(!WalkImpl(neighbour[(int)NeighbourRelationship.Left],
                   tile.left))
      { return false; }
      if(!WalkImpl(neighbour[(int)NeighbourRelationship.BottomLeft],
                   tile.bottom_left))
      { return false; }
      if(!WalkImpl(neighbour[(int)NeighbourRelationship.BottomRight],
                   tile.bottom_right))
      { return false; }

      return true;
    }
  }
}
