using UnityEngine;
using Notification;

namespace EndGame
{
  public class StartCheck
  { }
  public class CheckFailed
  { }
  public class CheckPassed
  { }

  public class Detector : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private const int total = 3;
    private int current = 0;

    private void Start()
    {
      subscriptions.Add(Pool.Subscribe<StartCheck>(_ => OnStartCheck()));
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
      { Lost(); }
      Debug.Assert(current >= -total, "Too many check results");
    }

    private void OnCheckPassed()
    {
      ++current;
      Debug.Assert(current <= total, "Too many check results");
    }

    private void Lost()
    {
      /* TODO: Show some UI. */
      Logger.Log("Game lost!");
    }
  }
}
