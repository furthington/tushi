using UnityEngine;
using Notification;
using System.Collections.Generic;

namespace Save
{
  public class TileInfo
  {
    public string Name
    { get; set; }
    public Vector3 Scale
    { get; set; }
    public float Rotation
    { get; set; }

    public TileInfo(string n, Vector3 s, float r)
    {
      Name = n;
      Scale = s;
      Rotation = r;
    }
  }

  public class ReadRow
  { }
  public class ReadRowReply
  {
    public int Number
    { get; set; }
    public List<TileInfo> Tiles
    { get; set; }

    public ReadRowReply(int n, List<TileInfo> t)
    {
      Number = n;
      Tiles = t;
    }
  }
  public class WriteRow
  {
    public int Number
    { get; set; }
    public List<TileInfo> Tiles
    { get; set; }

    public WriteRow(int n, List<TileInfo> t)
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
      subscriptions.Add
      (Pool.Subscribe<WriteRow>(Write));
    }

    private void Read()
    {
      var states = new List<TileInfo>();
      for(var cur = GetComponent<Board.Tile>();
          cur != null;
          cur = cur.right)
      {
        states.Add
        (
          new TileInfo
          (
            cur.name,
            cur.transform.localScale,
            cur.transform.rotation.z
          )
        );
      }
      Pool.Dispatch(new ReadRowReply(number, states));
    }

    private void Write(WriteRow wr)
    {
      if(wr.Number != number)
      { return; }

      var i = 0;
      for(var cur = GetComponent<Board.Tile>();
          cur != null;
          cur = cur.right, ++i)
      {
        Debug.Assert(i < wr.Tiles.Count, "Not enough tiles while loading");
        // TODO: Assign wr.Tiles[i] to cur
      }
    }
  }
}
