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
      subscriptions.Add<AddScore>
      (
       s =>
       {
         /* TODO: Make score count up? */
         score += s.Score;
         UpdateText();
       }
      );
      subscriptions.Add<LoadScore>(OnLoadScore);
      subscriptions.Add<SaveScore>(_ => OnSaveScore());
      subscriptions.Add<ScoreQuery>(_ => OnQuery());
      subscriptions.Add<WriteScore>(_ => OnWrite());
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
