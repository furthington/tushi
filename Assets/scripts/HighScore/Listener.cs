using UnityEngine;
using System;
using System.IO;
using Notification;

namespace HighScore
{
  public class Read
  { }
  public class ReadReply
  {
    public int Last
    { get; set; }
    public int Best
    { get; set; }
  }
  public class Write
  {
    public int Score
    { get; set; }

    public Write(int s)
    { Score = s; }
  }

  public class Listener : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private int? last = null;
    private int? all_time = null;

    private void Start()
    {
      subscriptions.Add<Read>(_ => DoRead());
      subscriptions.Add<Write>(DoWrite);
      Logger.LogFormat("High score path: {0}", Path());
      DoRead();
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void DoRead()
    {
      using(var timer = new Profile.TaskTimer("Read high scores"))
      {
        if((last == null || all_time == null) && File.Exists(Path()))
        {
          using(var reader = new StreamReader(Path()))
          {
            last = Int32.Parse(reader.ReadLine());
            all_time = Int32.Parse(reader.ReadLine());
          }
        }

        var ret = new ReadReply();
        ret.Last = last ?? 0;
        ret.Best = all_time ?? 0;
        Logger.LogFormat("Read scores; last: {0} best: {1}", last, all_time);
        Pool.Dispatch(ret);
      }
    }

    private void DoWrite(Write whs)
    {
      using(var timer = new Profile.TaskTimer("Write high scores"))
      {
        using(var writer = new StreamWriter(Path()))
        {
          writer.WriteLine(whs.Score);
          var old_max = all_time ?? 0;
          var max = Math.Max(old_max, whs.Score);
          if(max > old_max)
          { Logger.Log("New high score ({0} > {1})", max, old_max); }
          writer.WriteLine(max);
        }
        Logger.LogFormat("Wrote high scores to disk");
      }
    }

    private string Path()
    { return Application.persistentDataPath + "/high-scores"; }
  }
}
