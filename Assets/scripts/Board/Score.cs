using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace Board
{
  public class AddScore
  {
    public int Score
    { get; set; }
  }
  public class LoadScore
  {
    public int Score
    { get; set; }
  }
  public class SaveScore
  { }
  public class SaveScoreReply
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
      UpdateText();
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
            UpdateText();
          }
        )
      );
      subscriptions.Add(Pool.Subscribe<LoadScore>(OnLoadScore));
      subscriptions.Add(Pool.Subscribe<SaveScore>(_ => OnSaveScore()));
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void UpdateText()
    { text.text = score.ToString(); }

    private void OnLoadScore(LoadScore ls)
    {
      score = ls.Score;
      UpdateText();
    }

    private void OnSaveScore()
    {
      var reply = new SaveScoreReply();
      reply.Score = score;
      Pool.Dispatch(reply);
    }
  }
}
