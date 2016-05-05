using UnityEngine;
using UnityEngine.UI;

namespace Board
{
  public struct AddScore
  {
    public int Score
    { get; set; }
  }

  [RequireComponent(typeof(Text))]
  public class Score : MonoBehaviour
  {
    private Text text;
    private int score = 0;
    private Notification.SubscriptionStack subscriptions = new Notification.SubscriptionStack();

    private void Awake()
    {
      text = GetComponent<Text>();
      UpdateScore();
    }

    private void Start()
    {
      subscriptions.Add
      (
        Notification.Pool.Subscribe<AddScore>
        ( s =>
          {
            /* TODO: Make score count up? */
            score += s.Score;
            UpdateScore();
          }
        )
      );
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void UpdateScore()
    { text.text = score.ToString(); }
  }
}
