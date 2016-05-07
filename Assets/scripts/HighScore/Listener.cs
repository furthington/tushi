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
    public int Best
    { get; set; }
  }
  public struct Write
  {
    public int Score
    { get; set; }

    public Write(int s)
    { Score = s; }
  }

  public class Listener : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    /* TODO: Only hit disk once. */
    private int last = 0;
    private int all_time = 0;

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<Read>(_ => DoRead()));
      subscriptions.Add
      (Pool.Subscribe<Write>(DoWrite));
      Logger.LogFormat("High score path: {0}", Path());
    }

    private void DoRead()
    {
      using(var timer = new Profile.TaskTimer("Read high scores"))
      {
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
        ret.Best = all_time;
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
          writer.WriteLine(Math.Max(all_time, whs.Score));
        }
        Logger.LogFormat("wrote high scores to disk");
      }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/high-scores"; }
  }
}
