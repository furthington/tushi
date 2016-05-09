using UnityEngine;
using UnityEngine.UI;
using Notification;
using System;
using System.Collections.Generic;

namespace Save
{
  [Serializable]
  public class TileInfo
  {
    public string Name
    { get; set; }
    public Vector3 Scale
    { get; set; }
    public Quaternion Rotation
    { get; set; }

    public TileInfo(string n, Vector3 s, Quaternion r)
    {
      Name = n;
      Scale = s;
      Rotation = r;
    }
  }

  public class ReadRow
  { }
  [Serializable]
  public class ReadRowReply
  {
    public int Number
    { get; set; }
    public TileInfo[] Tiles
    { get; set; }

    public ReadRowReply(int n, TileInfo[] t)
    {
      Number = n;
      Tiles = t;
    }
  }
  public class WriteRow
  {
    public int Number
    { get; set; }
    public TileInfo[] Tiles
    { get; set; }

    public WriteRow(int n, TileInfo[] t)
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
            cur.transform.rotation
          )
        );
      }
      Pool.Dispatch(new ReadRowReply(number, states.ToArray()));
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
        Debug.Assert(i < wr.Tiles.Length, "Not enough tiles while loading");
        cur.transform.localScale = wr.Tiles[i].Scale;
        cur.transform.rotation = wr.Tiles[i].Rotation;
        cur.block = new GameObject().AddComponent<Board.Block>();
        cur.block.GetComponent<Image>()
           .sprite = Resources.Load<Sprite>(wr.Tiles[i].Name);
      }
    }
  }
}
