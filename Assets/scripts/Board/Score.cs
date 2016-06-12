using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
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
    private int displayed_score = 0;
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<int> scores = new List<int>();

    private void Awake()
    { text = GetComponent<Text>(); }

    private void Start()
    {
      subscriptions.Add<AddScore>
      (
       s =>
       {
         score += s.Score;
         if (scores.Count == 0)
         { StartCoroutine(UpdateText(s.Score)); }
         scores.Add(s.Score);
       }
      );
      subscriptions.Add<LoadScore>(OnLoadScore);
      subscriptions.Add<SaveScore>(_ => OnSaveScore());
      subscriptions.Add<ScoreQuery>(_ => OnQuery());
      subscriptions.Add<WriteScore>(_ => OnWrite());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private IEnumerator UpdateText(int amt)
    {
      int final_score = displayed_score + amt;
      while (displayed_score < final_score)
      {
        ++displayed_score;
        text.text = displayed_score.ToString();
        yield return new WaitForEndOfFrame();
      }
      scores.RemoveAt(0);
      if (scores.Count > 0)
      { StartCoroutine(UpdateText(scores[0])); }
    }

    private void OnLoadScore(LoadScore ls)
    {
      displayed_score = score = ls.Score;
      text.text = score.ToString();
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
