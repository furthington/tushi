using UnityEngine;
using Notification;

namespace EndGame
{
  public class Popper : MonoBehaviour
  {
    public GameObject game_over_prefab; /* Assign in editor. */
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      subscriptions.Add
      (
        Pool.Subscribe<GameLost>
        (
          rr =>
          Instantiate(game_over_prefab)
            .GetComponentInChildren<Populator>()
            .Initialize(rr)
        )
      );
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    /* For cheating */
    public void ShowGameOver()
    {
      GameLost fake = new GameLost(12380);
      Instantiate(game_over_prefab)
        .GetComponentInChildren<Populator>()
        .Initialize(fake);
    }
  }
}
