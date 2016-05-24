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
      using(var timer = new Profile.TaskTimer("Neighbour walk"))
      {
        foreach(var act in active)
        {
          /* TODO: Coroutine. */
          var valid = Walk(GetComponent<Neighbour>(), act);
          if(valid)
          {
            Logger.Log("Found valid position for piece");
            yield break; /* TODO: Notif? */
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
