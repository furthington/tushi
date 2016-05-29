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
    /* TODO: Save the current piece tray. */
    /* TODO: Save the current score. */
    /* TODO: Save the whole pieces. */
    /* TODO: Save the score. */
  }

  public class Loader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<SaveRowReply> replies = new List<SaveRowReply>();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<LoadGame>(_ => Load()));
      subscriptions.Add
      (Pool.Subscribe<SaveGame>(_ => Save()));
      subscriptions.Add
      (Pool.Subscribe<SaveRowReply>(OnSaveRow));

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

        foreach(var row in data.rows)
        { Pool.Dispatch(new LoadRow(row.Number, row.Tiles)); }
      }
    }

    public void Save() // TODO private
    {
      /* Ask each row to respond. */
      replies.Clear();
      Pool.Dispatch(new SaveRow());
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

    private void Write()
    {
      var filled = replies.Count(x => x != null);
      Debug.Assert(filled == replies.Count, "Cannot write incomplete data");

      using(var timer = new Profile.TaskTimer("Write save game"))
      {
        var data = new Data();
        data.rows = replies.ToArray();

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.OpenOrCreate))
        { bf.Serialize(file, data); }
      }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
