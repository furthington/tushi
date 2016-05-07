using UnityEngine;
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
    private SubscriptionStack subscriptions = new SubscriptionStack();
    /* TODO: Only hit disk once. */
    private int last = 0;
    private int all_time = 0;

    private void Awake()
    { Read(); }

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<Read>(_ => Read()));
      subscriptions.Add
      (Pool.Subscribe<Write>(Write));
    }

    private void Read()
    {
      /* TODO: Use a future here. */
      if(File.Exists(Path()))
      {
        using(var reader = new StreamReader(Path()))
        {
          last = Int32.Parse(reader.ReadLine());
          all_time = Int32.Parse(reader.ReadLine());
        }
      }

      var ret = new ReadReply();
      ret.Last = last;
      ret.AllTime = all_time;
      Logger.LogFormat("last: {0} all time: {1}", last, all_time);
      Pool.Dispatch(ret);
    }

    private void Write(Write whs)
    {
      using(var writer = new StreamWriter(Path()))
      {
        writer.WriteLine(whs.Score);
        writer.WriteLine(Math.Max(all_time, whs.Score));
      }
      Logger.LogFormat("wrote high scores to disk");
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/high-scores"; }
  }
}
