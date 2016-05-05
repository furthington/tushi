using UnityEngine;
using UnityEngine.UI;
using Notification;

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
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Awake()
    {
      text = GetComponent<Text>();
      UpdateScore();
    }

    private void Start()
    {
      subscriptions.Add
      (
        Pool.Subscribe<AddScore>
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
