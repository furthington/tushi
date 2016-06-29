using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using Notification;

namespace UI.Guide
{
  public class Read
  { }

  public class ReadReply
  {
    public bool Show
    { get; set; }

    public ReadReply(bool r)
    { Show = r; }
  }

  public class Write
  {
    public bool Show
    { get; set; }

    public Write(bool r)
    { Show = r; }
  }

  [RequireComponent(typeof(CanvasGroup))]
  public class Listener : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private bool is_showing = false;
    private CanvasGroup cg;

    private void Start()
    {
      cg = GetComponent<CanvasGroup>();
      DoRead();
      SetGuide();

      subscriptions.Add<Read>(_ => Pool.Dispatch(new ReadReply(is_showing)));
      subscriptions.Add<Write>(Save);
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void SetGuide()
    { cg.alpha = is_showing ? 1.0f : 0.0f; }

    private void Save(Write w)
    {
      is_showing = w.Show;
      SetGuide();
      using(var timer = new Profile.TaskTimer("Save guide option"))
      {
        using(var writer = new StreamWriter(Path()))
        { writer.WriteLine(is_showing); }
        Logger.Log
        ("Wrote guide option to disk: {0}", is_showing ? "show" : "hide");
      }
    }

    private void DoRead()
    {
      using(var timer = new Profile.TaskTimer("Load guide option"))
      {
        if(File.Exists(Path()))
        {
          using(var reader = new StreamReader(Path()))
          { is_showing = Boolean.Parse(reader.ReadLine()); }
          Logger.Log
          ("Read guide option: {0}", is_showing ? "show" : "hide");
        }
      }
    }

    private string Path()
    { return Application.persistentDataPath + "/guide"; }
  }
}
