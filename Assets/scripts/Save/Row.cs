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
    public float[] Scale
    { get; set; }
    public float[] Rotation
    { get; set; }

    public TileInfo(string n, float[] s, float[] r)
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
        if(cur.block != null)
        {
          states.Add
          (
            new TileInfo
            (
              cur.block.GetComponent<Image>().sprite.name,
              new float[]
              {
                cur.block.transform.localScale.x,
                cur.block.transform.localScale.y,
                cur.block.transform.localScale.z
              },
              new float[]
              {
                cur.block.transform.rotation.x,
                cur.block.transform.rotation.y,
                cur.block.transform.rotation.z,
                cur.block.transform.rotation.w
              }
            )
          );
        }
        else
        { states.Add(new TileInfo("", null, null)); }
      }
      Pool.Dispatch(new ReadRowReply(number, states.ToArray()));
    }

    private void Write(WriteRow wr)
    {
      if(wr.Number != number)
      { return; }

      /* TODO: Cache this? */
      var canvas = GameObject.FindGameObjectWithTag("main_canvas");

      var i = 0;
      for(var cur = GetComponent<Board.Tile>();
          cur != null;
          cur = cur.right, ++i)
      {
        Debug.Assert(i < wr.Tiles.Length, "Not enough tiles while loading");
        if(wr.Tiles[i].Name == "")
        { continue; }

        cur.block = new GameObject().AddComponent<Board.Block>();
        cur.block.transform.position = cur.transform.position;
        cur.block.transform.localScale = new Vector3
        (
          wr.Tiles[i].Scale[0],
          wr.Tiles[i].Scale[1],
          wr.Tiles[i].Scale[2]
        );
        cur.block.transform.rotation = new Quaternion
        (
          wr.Tiles[i].Rotation[0],
          wr.Tiles[i].Rotation[1],
          wr.Tiles[i].Rotation[2],
          wr.Tiles[i].Rotation[3]
        );

        cur.block.gameObject.AddComponent<Image>();
        cur.block.transform.SetParent(canvas.transform);
        cur.block.GetComponent<Image>()
           .sprite = Resources.Load<Sprite>(wr.Tiles[i].Name);
        cur.block.GetComponent<Image>().SetNativeSize();
        Logger.LogFormat("Tile ({0},{1}) {2}", number, i, wr.Tiles[i].Name);
      }
    }
  }
}
