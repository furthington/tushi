using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Notification;

namespace UI.Mode
{
  public class Read
  { }

  public class ReadReply
  {
    public bool Right
    { get; set; }

    public ReadReply(bool r)
    { Right = r; }
  }

  public class Write
  {
    public bool Right
    { get; set; }

    public Write(bool r)
    { Right = r; }
  }

  public class Listener : MonoBehaviour
  {
    public List<GameObject> left_ui; /* Assign in editor. */
    public List<GameObject> right_ui; /* Assign in editor. */
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private bool is_right = true;

    private void Start()
    {
      DoRead();

      subscriptions.Add<Read>(_ => Pool.Dispatch(new ReadReply(is_right)));
      subscriptions.Add<Write>
      (
        mode =>
        {
          SetMode(mode.Right);
          Save();
        }
      );
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void SetMode(bool right)
    {
      is_right = right;
      foreach (GameObject obj in right_ui)
      { obj.SetActive(right); }
      foreach (GameObject obj in left_ui)
      { obj.SetActive(!right); }
    }

    private void Save()
    {
      using(var writer = new StreamWriter(Path()))
      { writer.WriteLine(is_right); }
      Logger.LogFormat("Wrote UI mode to disk");
    }

    private void DoRead()
    {
      if(File.Exists(Path()))
      {
        using(var reader = new StreamReader(Path()))
        { is_right = Boolean.Parse(reader.ReadLine()); }
        SetMode(is_right);
        Logger.Log("Read UI mode: ", is_right);
      }
    }

    private string Path() /* TODO: encrypt */
    { return Application.persistentDataPath + "/ui-mode"; }
  }
}
