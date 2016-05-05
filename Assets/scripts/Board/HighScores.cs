using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Notification;

namespace Board
{
  public struct ReadHighScores
  { }
  public struct ReadHighScoresReply
  {
    public int Last
    { get; set; }
    public int AllTime
    { get; set; }
  }

  public class HighScores : MonoBehaviour
  {
    private string file = Application.persistentDataPath
                          + "/high-scores";
    private SubscriptionStack subscriptions = new SubscriptionStack();

    public void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<ReadHighScores>(_ => Read()));
    }

    private void Read()
    {
      int last = 0;
      int all_time = 0;
      /* TODO: Use a future here. */
      using(var reader = new StreamReader(file))
      {
        last = Int32.Parse(reader.ReadLine());
        all_time = Int32.Parse(reader.ReadLine());
      }

      var ret = new ReadHighScoresReply();
      ret.Last = last;
      ret.AllTime = all_time;
      Logger.LogFormat("last: {0} all time: {1}", last, all_time);
      Pool.Dispatch(ret);
    }
  }
}
