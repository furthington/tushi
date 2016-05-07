using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Notification;

namespace HighScore
{
  public struct Read
  { }
  public struct ReadReply
  {
    public int Last
    { get; set; }
    public int AllTime
    { get; set; }
  }
  public struct Write
  {
    public int Score
    { get; set; }
  }

  public class Listener : MonoBehaviour
  {
    private string file = Application.persistentDataPath
                          + "/high-scores"; // TODO: encrypt
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private int last = 0;
    private int all_time = 0;

    public void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<Read>(_ => Read()));
      subscriptions.Add
      (Pool.Subscribe<Write>(Write));
    }

    private void Read()
    {
      /* TODO: Use a future here. */
      using(var reader = new StreamReader(file))
      {
        last = Int32.Parse(reader.ReadLine());
        all_time = Int32.Parse(reader.ReadLine());
      }

      var ret = new ReadReply();
      ret.Last = last;
      ret.AllTime = all_time;
      Logger.LogFormat("last: {0} all time: {1}", last, all_time);
      Pool.Dispatch(ret);
    }

    private void Write(Write whs)
    {
      using(var writer = new StreamWriter(file))
      {
        writer.WriteLine(whs.Score);
        writer.WriteLine(Math.Max(all_time, whs.Score));
      }
      Logger.LogFormat("wrote high scores to disk");
    }
  }
}
