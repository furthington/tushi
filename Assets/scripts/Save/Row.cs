using UnityEngine;
using UnityEngine.UI;
using Notification;
using System;
using System.Collections.Generic;
using Error;

namespace Save
{
  [Serializable]
  public class TileInfo
  {
    public string Name
    { get; set; }
    public string SpriteName
    { get; set; }
    public float[] Scale
    { get; set; }
    public float[] Rotation
    { get; set; }
    public string PieceIdentifier
    { get; set; }

    public TileInfo(string n, string sn, float[] s, float[] r, string p)
    {
      Name = n;
      SpriteName = sn;
      Scale = s;
      Rotation = r;
      PieceIdentifier = p;
    }
  }

  public class SaveRow
  { }
  [Serializable]
  public class SaveRowReply
  {
    public int Number
    { get; set; }
    public TileInfo[] Tiles
    { get; set; }

    public SaveRowReply(int n, TileInfo[] t)
    {
      Number = n;
      Tiles = t;
    }
  }
  public class LoadRow
  {
    public int Number
    { get; set; }
    public TileInfo[] Tiles
    { get; set; }

    public LoadRow(int n, TileInfo[] t)
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
    private GameObject canvas;

    private void Start()
    {
      canvas = GameObject.FindGameObjectWithTag("main_canvas");

      subscriptions.Add<SaveRow>(_ => OnSave());
      subscriptions.Add<LoadRow>(OnLoad);
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void OnSave()
    {
      using(var timer = new Profile.TaskTimer("Save row"))
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
                cur.block.name,
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
                },
                (cur.block.piece != null)
                  ? cur.block.piece.GetComponent<Board.PieceIdentifier>().ID
                  : ""
              )
            );
          }
          else
          { states.Add(new TileInfo("", "", null, null, "")); }
        }
        Pool.Dispatch(new SaveRowReply(number, states.ToArray()));
      }
    }

    private void OnLoad(LoadRow wr)
    {
      if(wr.Number != number)
      { return; }

      using(var timer = new Profile.TaskTimer("Load row"))
      {
        var i = 0;
        for(var cur = GetComponent<Board.Tile>();
            cur != null;
            cur = cur.right, ++i)
        {
          Assert.Invariant(i < wr.Tiles.Length,
                           "Not enough tiles while loading");
          if(wr.Tiles[i].Name == "")
          { continue; }

          cur.block = new GameObject().AddComponent<Board.Block>();
          cur.block.piece_id = wr.Tiles[i].PieceIdentifier;
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
             .sprite = Resources.Load<Sprite>(wr.Tiles[i].SpriteName);
          cur.block.GetComponent<Image>().SetNativeSize();
          cur.block.name = wr.Tiles[i].Name;
        }
      }
    }
  }
}
