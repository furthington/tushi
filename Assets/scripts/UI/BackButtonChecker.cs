using System;
using System.Collections.Generic;
using UnityEngine;
using Notification;

namespace UI
{
  public struct AddBackButtonHandler
  {
    public GameObject Obj;
    public Action Handler;

    public AddBackButtonHandler(GameObject obj, Action handler)
    {
      Obj = obj;
      Handler = handler;
    }
  }
  public struct RemoveBackButtonHandler
  {
    public GameObject Obj;

    public RemoveBackButtonHandler(GameObject obj)
    { Obj = obj; }
  }

  public struct TaggedAction
  {
    public GameObject Obj;
    public Action Handler;

    public TaggedAction(GameObject obj, Action handler)
    {
      Obj = obj;
      Handler = handler;
    }
  }

  public class BackButtonChecker : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<TaggedAction> stack = new List<TaggedAction>();

    /* Manual checking for pressed versus holding. */
    private bool escaped_last_frame;

    private void Start()
    {
      subscriptions.Add<AddBackButtonHandler>
      (
        abbh =>
        {
          stack.Add(new TaggedAction(abbh.Obj, abbh.Handler));
          Logger.Log("Pushing handler onto stack; id = " + stack.Count);
        }
      );
      subscriptions.Add<RemoveBackButtonHandler>
      (
        rbbh =>
        {
          stack.RemoveAll(e => e.Obj == rbbh.Obj);
          Logger.Log("Popping! Size: " + stack.Count);
        }
      );
    }

    private void Update()
    {
      // XXX: We might want this later
      //if(Application.platform != RuntimePlatform.Android)
      //{ return; }

      if
      (
        Input.GetKey(KeyCode.Escape) &&
        !escaped_last_frame
      )
      { escaped_last_frame = true; }
      else if
      (
        !Input.GetKey(KeyCode.Escape) &&
        escaped_last_frame
      )
      {
        /* Detect key up. */
        if(stack.Count > 0)
        {
          Logger.Log("Running! Top: " + stack.Count);
          stack[stack.Count - 1].Handler();
        }
        escaped_last_frame = false;
      }
    }

    private void OnDisable()
    { subscriptions.Clear(); }
  }
}
