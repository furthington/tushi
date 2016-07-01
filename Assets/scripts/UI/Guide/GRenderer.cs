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
    bool show = false;
    private void Start()
    {
      img = GetComponent<Image>();

      /* TODO: Optimize?
        If guides are not showing, unsubscribe from PiecePlaced entirely? */
      subscriptions.Add<Board.PiecePlaced>
      (
        _ =>
        {
          if (show)
          {
            StopAllCoroutines();
            StartCoroutine(DelayedUpdate());
          }
        }
      );
      subscriptions.Add<Write>
      (
        w =>
        {
          show = w.Show;
          if (show)
          { UpdateGuide(); }
        }
      );

      StartCoroutine(Initialize());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private IEnumerator Initialize()
    {
      /* TODO: unsub may not be called if object is destroyed during yield. */
      var sub = Pool.Subscribe<ReadReply>
                (
                  w =>
                  {
                    show = w.Show;
                    UpdateGuide();
                  }
                );
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
