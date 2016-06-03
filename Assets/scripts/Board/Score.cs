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
  public class ScoreQuery
  { }
  public class ScoreReply
  {
    public int Score
    { get; set; }
  }
  public class WriteScore
  { }

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
      subscriptions.Add(Pool.Subscribe<ScoreQuery>(_ => OnQuery()));
      subscriptions.Add(Pool.Subscribe<WriteScore>(_ => OnWrite()));
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

    private void OnQuery()
    {
      var reply = new ScoreReply();
      reply.Score = score;
      Pool.Dispatch(reply);
    }

    private void OnWrite()
    { Pool.Dispatch(new HighScore.Write(score)); }
  }
}
