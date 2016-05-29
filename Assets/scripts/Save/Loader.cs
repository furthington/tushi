using UnityEngine;
using Notification;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Save
{
  public class GameExists
  { }
  public class LoadGame
  { }
  public class SaveGame
  { }

  [Serializable]
  public class Data
  {
    public SaveRowReply[] rows;
    public int score;
    /* TODO: Save the current piece tray. */
    /* TODO: Save the whole pieces. */
  }

  public class Loader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<SaveRowReply> replies = new List<SaveRowReply>();
    Data data_to_save = null;

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<LoadGame>(_ => Load()));
      subscriptions.Add
      (Pool.Subscribe<SaveGame>(_ => Save()));
      subscriptions.Add
      (Pool.Subscribe<SaveRowReply>(OnSaveRow));
      subscriptions.Add
      (Pool.Subscribe<Board.SaveScoreReply>(OnSaveScore));

      if(File.Exists(Path()))
      { Pool.Dispatch(new GameExists()); }
    }

    private void Load()
    {
      Debug.Assert(File.Exists(Path()), "Trying to load non-existent save");
      using(var timer = new Profile.TaskTimer("Read saved game"))
      {
        Data data = null;

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.Open))
        { data = (Data)bf.Deserialize(file); }

        var load_score = new Board.LoadScore();
        load_score.Score = data.score;
        Pool.Dispatch(load_score);

        foreach(var row in data.rows)
        { Pool.Dispatch(new LoadRow(row.Number, row.Tiles)); }
      }
    }

    private void Save()
    {
      /* Ask each row to respond. */
      replies.Clear();
      data_to_save = new Data();
      Pool.Dispatch(new Board.SaveScore());
      Pool.Dispatch(new SaveRow());
      /* XXX: SaveRow should be the last notif sent here. */
    }

    private void OnSaveRow(SaveRowReply rrr)
    {
      if(rrr.Number >= replies.Count)
      {
        replies.AddRange
        (
          Enumerable.Repeat<SaveRowReply>
          (null, (rrr.Number - replies.Count) + 1)
        );
      }
      replies[rrr.Number] = rrr;

      var filled = replies.Count(x => x != null);
      if(filled == replies.Count)
      { Write(); }
    }

    private void OnSaveScore(Board.SaveScoreReply reply)
    { data_to_save.score = reply.Score; }

    private void Write()
    {
      var filled = replies.Count(x => x != null);
      Debug.Assert(filled == replies.Count, "Cannot write incomplete data");

      using(var timer = new Profile.TaskTimer("Write save game"))
      {
        data_to_save.rows = replies.ToArray();

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.OpenOrCreate))
        { bf.Serialize(file, data_to_save); }
      }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
