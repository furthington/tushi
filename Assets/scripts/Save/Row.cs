using UnityEngine;
using Notification;
using System.Collections.Generic;

namespace Save
{
  public struct ReadRow
  { }
  public struct ReadRowReply
  {
    public int Number
    { get; set; }
    public List<string> Tiles
    { get; set; }

    public ReadRowReply(int n, List<string> t)
    {
      Number = n;
      Tiles = t;
    }
  }

  [RequireComponent (typeof(Board.Tile))]
  public class Row : MonoBehaviour
  {
    public int number;

    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<ReadRow>(_ => Read()));
    }

    private void Read()
    {
      var states = new List<string>();
      for(var cur = GetComponent<Board.Tile>();
          cur != null;
          cur = cur.right)
      { states.Add(cur.name); } // TODO: Add the correct thing here
      Pool.Dispatch(new ReadRowReply(number, states));
    }

    //private void Write()
    //{
    //  for(var cur = GetComponent<Board.Tile>();
    //      cur != null;
    //      cur = cur.right)
    //  { }
    //}
  }
}
