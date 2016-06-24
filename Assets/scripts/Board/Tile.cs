using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Profile;
using Notification;

namespace Board
{
  public class Tile : MonoBehaviour
  {
    public Tile top_right;
    public Tile right;
    public Tile bottom_right;
    public Tile bottom_left;
    public Tile left;
    public Tile top_left;
    public List<Tile> line0;
    public List<Tile> line1;
    public List<Tile> line2;
    public List<Tile>[] lines;

    public Block block = null;

    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      lines = new List<Tile>[]{ line0, line1, line2 };
      subscriptions.Add<ActiveTileRequest>(ActiveReply);
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void Emplace(Block b)
    {
      block = b;
      foreach(var line in lines)
      {
        if(IsComplete(line))
        {
          StartCoroutine(Clear(line));
          var s = new AddScore();
          s.Score = line.Count * 6;
          Pool.Dispatch(s);
        }
      }
    }

    private static bool IsComplete(List<Tile> line)
    {
      var count = line.Aggregate
      (
        new { rice = 0, empty = 0 },
        (acc, e) =>
        {
          if(IsEmpty(e))
          { return new { acc.rice, empty = acc.empty + 1 }; }
          else if(IsRice(e))
          { return new { rice = acc.rice + 1, acc.empty }; }
          return acc;
        }
      );
      return count.rice > 0 // One or more rice
             && count.empty == 0 // Every tile filled
             && count.rice < line.Count; // One more more non-rice
    }

    private static bool IsEmpty(Tile t)
    { return t.block == null; }

    private static bool IsRice(Tile t)
    { return t.block.name == "rice"; }

    private static IEnumerator Clear(List<Tile> line)
    {
      int i = 0;
      foreach(var tile in line)
      { tile.block.DeferredRemove(i++); }

      yield return new WaitForEndOfFrame();
      foreach (var tile in line)
      { tile.block = null; }
    }

    private void ActiveReply(ActiveTileRequest r)
    {
      if(block == null)
      { Pool.Dispatch(new ActiveTileReply(this, r)); }
    }
  }
}
