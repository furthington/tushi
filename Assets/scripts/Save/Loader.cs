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
    public Board.PieceIdentifier.SaveReply[] pieces;
    public int score;
    public Board.PieceTray.SaveReply piece_tray;
    /* TODO: Save the whole pieces. */
  }

  public class Loader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<SaveRowReply> row_replies = new List<SaveRowReply>();
    private List<Board.PieceIdentifier.SaveReply> piece_replies = new List<Board.PieceIdentifier.SaveReply>();
    private Data data_to_save = null;
    private int total_rows = 9;

    private void Start()
    {
      subscriptions.Add<LoadGame>(_ => Load());
      subscriptions.Add<SaveGame>(_ => Save());
      subscriptions.Add<SaveRowReply>(OnSaveRow);
      subscriptions.Add<Board.SaveScoreReply>(OnSaveScore);
      subscriptions.Add<Board.PieceTray.SaveReply>(OnSavePieceTray);
      subscriptions.Add<Board.PieceIdentifier.SaveReply>(OnSavePieceIdentifier);
      subscriptions.Add<EndGame.GameLost>(_ => OnGameLost());

      if(File.Exists(Path()))
      { Pool.Dispatch(new LoadGame()); }
      else
      { Pool.Dispatch(new NewGame()); }
    }

    private void OnDisable()
    { subscriptions.Clear(); }

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

        foreach(var piece in data.pieces)
        { Pool.Dispatch(new Board.PieceLoader.Load(piece)); }
      }
    }

    public void Save()
    {
      /* Multiple requests to save shouldn't do anything. */
      if(data_to_save != null)
      { return; }

      /* Ask each row to respond. */
      row_replies.Clear();
      piece_replies.Clear();
      data_to_save = new Data();
      Pool.Dispatch(new Board.SaveScore());
      Pool.Dispatch(new Board.PieceTray.Save());
      Pool.Dispatch(new Board.PieceIdentifier.Save());
      Pool.Dispatch(new SaveRow());
      /* XXX: SaveRow should be the last notif sent here. */
    }

    private void OnSaveRow(SaveRowReply rrr)
    {
      if(rrr.Number >= row_replies.Count)
      {
        row_replies.AddRange
        (
          Enumerable.Repeat<SaveRowReply>
          (null, (rrr.Number - row_replies.Count) + 1)
        );
      }
      row_replies[rrr.Number] = rrr;

      var filled = row_replies.Count(x => x != null);
      if(filled == total_rows)
      { Write(); }
    }

    private void OnSaveScore(Board.SaveScoreReply reply)
    { data_to_save.score = reply.Score; }

    private void OnSavePieceTray(Board.PieceTray.SaveReply reply)
    { data_to_save.piece_tray = reply; }

    private void OnSavePieceIdentifier(Board.PieceIdentifier.SaveReply reply)
    { piece_replies.Add(reply); }

    private void Write()
    {
      var filled = row_replies.Count(x => x != null);
      Assert.Invariant(filled == row_replies.Count,
                       "Cannot write incomplete data");

      using(var timer = new Profile.TaskTimer("Write save game"))
      {
        data_to_save.rows = row_replies.ToArray();
        data_to_save.pieces = piece_replies.ToArray();

        var bf = new BinaryFormatter();
        using(var file = File.Open(Path(), FileMode.OpenOrCreate))
        { bf.Serialize(file, data_to_save); }
      }
      Logger.Log("Game saved");
      data_to_save = null;
    }

    private void OnGameLost()
    { File.Delete(Path()); }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
