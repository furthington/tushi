using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace UI
{
  public enum HighScoreState
  {
    Last,
    AllTime
  }

  [RequireComponent (typeof(Text))]
  public class HighScoreReader : MonoBehaviour
  {
    public HighScoreState state = HighScoreState.Last;

    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add
      (Pool.Subscribe<HighScore.ReadReply>(Read));
      Pool.Dispatch(new HighScore.Read());
    }

    private void Read(HighScore.ReadReply rr)
    {
      switch(state)
      {
        case HighScoreState.Last:
          GetComponent<Text>().text = "Last Score: " + rr.Last.ToString();
          break;
        case HighScoreState.AllTime:
          GetComponent<Text>().text = "High Score: " + rr.AllTime.ToString();
          break;
      }
    }
  }
}
