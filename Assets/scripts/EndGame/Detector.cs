using UnityEngine;
using Notification;
using System.Collections;

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
    private const int total = 3;
    private int current = 0;

    private void Start()
    {
      /* TODO: Use a coroutine for this whole thing. */
      subscriptions.Add(Pool.Subscribe<Board.PiecePlaced>(_ => OnStartCheck()));
      subscriptions.Add(Pool.Subscribe<CheckFailed>(_ => OnCheckFailed()));
      subscriptions.Add(Pool.Subscribe<CheckPassed>(_ => OnCheckPassed()));
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void OnStartCheck()
    { current = 0; }

    private void OnCheckFailed()
    {
      if(--current == -total)
      { StartCoroutine(Lost()); }
      Debug.Assert(current >= -total, "Too many check results");
    }

    private void OnCheckPassed()
    {
      ++current;
      Debug.Assert(current <= total, "Too many check results");
    }

    private IEnumerator Lost()
    {
      /* TODO: Show some UI. */
      var score = 0;
      {
        var sub = Pool.Subscribe<Board.ScoreReply>(r => score = r.Score);
        Pool.Dispatch(new Board.ScoreQuery());
        yield return Notification.Async.WaitForReplies<Board.ScoreQuery>();
        Pool.Unsubscribe(sub);
      }
      Logger.Log("Game lost! Score: {0}", score);
      Pool.Dispatch(new GameLost(score));
    }
  }
}
