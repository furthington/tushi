using UnityEngine;
using Notification;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Error;

namespace Save
{
  public class LoadGame
  { }
  public class SaveGame
  { }
  public class NewGame
  { }

  [Serializable]
  public class Data
  {
    public SaveRowReply[] rows;
    public int score;
    public Board.PieceTray.SaveReply piece_tray;
    /* TODO: Save the whole pieces. */
  }

  public class Loader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<SaveRowReply> replies = new List<SaveRowReply>();
    Data data_to_save = null;

    private void Start()
    {
      subscriptions.Add<LoadGame>(_ => Load());
      subscriptions.Add<SaveGame>(_ => Save());
      subscriptions.Add<SaveRowReply>(OnSaveRow);
      subscriptions.Add<Board.SaveScoreReply>(OnSaveScore);
      subscriptions.Add<Board.PieceTray.SaveReply>(OnSavePieceTray);
      subscriptions.Add<EndGame.GameLost>(_ => OnGameLost());

      if(File.Exists(Path()))
      { Pool.Dispatch(new LoadGame()); }
      else
      { Pool.Dispatch(new NewGame()); }
    }

    private void Load()
    {
      Assert.Invariant(File.Exists(Path()),
                       "Trying to load non-existent save");
      using(var timer = new Profile.TaskTimer("Read saved game"))
      {
        Data data = null;

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.Open))
        { data = (Data)bf.Deserialize(file); }

        var load_score = new Board.LoadScore();
        load_score.Score = data.score;
        Pool.Dispatch(load_score);

        Pool.Dispatch
        (
          new Board.PieceTray.Load
          (
            data.piece_tray.Names,
            data.piece_tray.Rotations
          )
        );

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
      Pool.Dispatch(new Board.PieceTray.Save());
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

    private void OnSavePieceTray(Board.PieceTray.SaveReply reply)
    { data_to_save.piece_tray = reply; }

    private void Write()
    {
      var filled = replies.Count(x => x != null);
      Assert.Invariant(filled == replies.Count,
                       "Cannot write incomplete data");

      using(var timer = new Profile.TaskTimer("Write save game"))
      {
        data_to_save.rows = replies.ToArray();

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.OpenOrCreate))
        { bf.Serialize(file, data_to_save); }
      }
      Logger.Log("Game saved");
    }

    private void OnGameLost()
    { File.Delete(Path()); }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
