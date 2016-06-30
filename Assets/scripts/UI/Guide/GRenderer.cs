using UnityEngine;
using System.Collections;
using Notification;
using UnityEngine.UI;

namespace UI.Guide
{
  /* Can't name it as Renderer, conflicts with Unity stuff. */
  [RequireComponent(typeof(Image))]
  public class GRenderer : MonoBehaviour
  {
    public Board.Tile tile; /* Assign in editor. */
    private Image img;
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private Subscription<Board.PiecePlaced> subscription = null;

    private void Start()
    {
      img = GetComponent<Image>();
      subscriptions.Add<Write>
      (
        w =>
        {
          if (w.Show)
          {
            Subscribe();
            UpdateGuide();
          }
          else
          { Unsubscribe(); }
        }
      );
      StartCoroutine(Initialize());
    }

    private void Subscribe()
    {
      /* TODO: Fix subscription. */
      if (subscription != null)
      { return; }
      subscription = Pool.Subscribe<Board.PiecePlaced>
      (
        _ =>
        {
          StopAllCoroutines();
          StartCoroutine(DelayedUpdate());
        }
      );
    }

    private void Unsubscribe()
    {
      if (subscription == null)
      { return; }
      Pool.Unsubscribe(subscription);
      subscription = null;
    }

    private void OnDisable()
    {
      subscriptions.Clear();
      Unsubscribe();
    }

    private IEnumerator Initialize()
    {
      /* TODO: unsub may not be called if object is destroyed during yield. */
      var sub = Pool.Subscribe<ReadReply>(_ => UpdateGuide());
      Pool.Dispatch(new Read());
      yield return Notification.Async.WaitForReplies<Read>();
      Pool.Unsubscribe(sub);
    }

    private IEnumerator DelayedUpdate()
    {
      yield return Notification.Async.WaitForReplies<Board.PiecePlaced>();
      UpdateGuide();
    }

    private void UpdateGuide()
    {
      Color c = img.color;
      c.a = (tile.block == null) ? 0.4f : 1.0f;
      img.color = c;
    }
  }
}
