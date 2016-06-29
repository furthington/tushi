using UnityEngine;
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

      subscriptions.Add<Board.PiecePlaced>
      (
        _ =>
        {
          /* TODO: Optimize?
            If guides are not showing, unsubscribe from PiecePlaced entirely? */
          if (show)
          { UpdateGuide(); }
        }
      );
      subscriptions.Add<Write>(w => show = w.Show);

      /* TODO: Unsubscribe from ReadReply right after responding? */
      subscriptions.Add<ReadReply>
      (
        rr =>
        {
          show = rr.Show;
          UpdateGuide();
        }
      );
      Pool.Dispatch(new Read());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void UpdateGuide()
    {
      Color c = img.color;
      c.a = (tile.block == null) ? 0.4f : 1.0f;
      img.color = c;
    }
  }
}
