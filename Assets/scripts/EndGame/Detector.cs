using UnityEngine;
using Notification;
using System.Collections;
using Error;

namespace EndGame
{
  public class CheckFailed
  { }
  public class CheckPassed
  { }
  public class GameLost
  {
    public int Score
    { get; set; }

    public GameLost(int s)
    { Score = s; }
  }

  public class Detector : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private const int max = 3;
    private int failed = 0;
    private int total = 0;

    private void Start()
    {
      /* TODO: Use a coroutine for this whole thing? */
      subscriptions.Add(Pool.Subscribe<Board.PiecePlaced>(_ => OnStartCheck()));
      subscriptions.Add(Pool.Subscribe<CheckFailed>(_ => OnCheckFailed()));
      subscriptions.Add(Pool.Subscribe<CheckPassed>(_ => OnCheckPassed()));
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void OnStartCheck()
    { failed = total = 0; }

    private void OnCheckFailed()
    {
      ++total;
      if(++failed == max)
      { StartCoroutine(Lost()); }
      CheckValues();
    }

    private void OnCheckPassed()
    {
      ++total;
      CheckValues();
    }

    private void CheckValues()
    {
      Assert.Invariant(failed <= max && total <= max,
                       "Too many check results");
      if(total == max)
      { Pool.Dispatch(new Save.SaveGame()); }
    }
    
    private IEnumerator Lost()
    {
      var score = 0;
      {
        var sub = Pool.Subscribe<Board.ScoreReply>(r => score = r.Score);
        Pool.Dispatch(new Board.ScoreQuery());
        yield return Notification.Async.WaitForReplies<Board.ScoreQuery>();
        Pool.Unsubscribe(sub);
      }
      Logger.Log("Game lost! Score: {0}", score);
      Pool.Dispatch(new GameLost(score));
      Pool.Dispatch(new Board.WriteScore());
    }
  }
}
