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

  public class Root : MonoBehaviour
  {
    public string neighbour_json; /* Assign in editor. */
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<Tile> active = new List<Tile>();
    private List<List<List<int?>>> neighbour_rotations;
    private const int threshold = 40; /* TODO: Configure */

    /* This is awake so that new pieces added to the tray
       can participate in endgame checks. */
    private void Awake()
    {
      neighbour_rotations = NeighbourParser.GetRotations(neighbour_json);

      subscriptions.Add
      (Pool.Subscribe<PiecePlaced>(_ => FindPlacement()));
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
        Pool.Dispatch(new EndGame.CheckFailed());
        yield break;
      }
      else if(active.Count > threshold)
      {
        Logger.Log("Active count greater than threshold");
        Pool.Dispatch(new EndGame.CheckPassed());
        yield break;
      }

      using(var timer = new Profile.TaskTimer("Neighbour walk"))
      {
        foreach(var rotation in neighbour_rotations)
        {
          foreach(var act in active)
          {
            /* TODO: Coroutine? */
            var valid = Walk(rotation, 0, act);
            if(valid)
            {
              Logger.Log("Found valid position for piece");
              Pool.Dispatch(new EndGame.CheckPassed());
              yield break;
            }
          }
        }
      }
      Logger.Log("No piece found");
      Pool.Dispatch(new EndGame.CheckFailed());
    }

    private void StoreActiveTile(ActiveTileReply r)
    {
      if(r.Request.Requestor != gameObject)
      { return; }
      active.Add(r.Active);
    }

    private bool WalkImpl(List<List<int?>> rotation,
                          int? next, Tile tile)
    {
      if(next != null)
      {
        if(tile == null || tile.block != null)
        { return false; }
        return Walk(rotation, (int)next, tile);
      }
      return true;
    }

    private bool Walk(List<List<int?>> rotation, int line, Tile tile)
    {
      /* We've hit a leaf. */
      if(tile == null)
      { return true; }

      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.Right],
                   tile.right))
      { return false; }
      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.TopRight],
                   tile.top_right))
      { return false; }
      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.TopLeft],
                   tile.top_left))
      { return false; }
      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.Left],
                   tile.left))
      { return false; }
      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.BottomLeft],
                   tile.bottom_left))
      { return false; }
      if(!WalkImpl(rotation,
                   rotation[line][(int)NeighbourRelationship.BottomRight],
                   tile.bottom_right))
      { return false; }

      return true;
    }
  }
}
