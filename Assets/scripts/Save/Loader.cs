using UnityEngine;
using Notification;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Save
{
  public class GameExists
  { }
  public class LoadGame
  { }
  public class SaveGame
  { }

  public class Loader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<ReadRowReply> replies = new List<ReadRowReply>();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<LoadGame>(_ => Load()));
      subscriptions.Add
      (Pool.Subscribe<SaveGame>(_ => Save()));
      subscriptions.Add
      (Pool.Subscribe<ReadRowReply>(SaveRow));

      if(File.Exists(Path()))
      { Pool.Dispatch(new GameExists()); }
    }

    private void Load()
    {
      /* TODO: Load the main level. */

      Debug.Assert(File.Exists(Path()), "Trying to load non-existent save");
      using(var reader = new StreamReader(Path()))
      {
        /* TODO: Parse each line for each row. */
      }
    }

    private void Save()
    {
      /* Ask each row to respond. */
      Pool.Dispatch(new ReadRow());
    }

    private void SaveRow(ReadRowReply rrr)
    {
      if(rrr.Number >= replies.Count)
      {
        replies.AddRange
        (Enumerable.Repeat<ReadRowReply>(null, rrr.Number - replies.Count));
      }
      replies[rrr.Number] = rrr;

      var filled = replies.Count(x => x != null);
      if(filled == replies.Count)
      { Write(); }
    }

    private void Write()
    {
      using(var writer = new StreamWriter(Path()))
      {
        /* TODO: Write replies to file. */
      }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/current-game"; }
  }
}
