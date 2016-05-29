using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace EndGame
{
  public class Populator : MonoBehaviour
  {
    public Text score; /* Assign in editor. */
    public Text new_high_score; /* Assign in editor. */
    private SubscriptionStack subscriptions = new SubscriptionStack();

    public void Initialize(GameLost results)
    {
      score.text = results.Score.ToString();
      subscriptions.Add
      (
        Pool.Subscribe<HighScore.ReadReply>
        (
          rr =>
          {
            if (results.Score > rr.Best)
            { new_high_score.gameObject.SetActive(true); }
            subscriptions.Clear();
          }
        )
      );
      Pool.Dispatch(new HighScore.Read());
    }
  }
}
