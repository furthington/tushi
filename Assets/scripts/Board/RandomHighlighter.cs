using UnityEngine;
using System.Collections;
using Notification;

namespace Board
{
  public class RandomHighlighter : MonoBehaviour
  {
    private bool glowing = false;
    private Tile[] tiles;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      tiles = GameObject.FindObjectsOfType<Tile>();
      StartCoroutine(Glow());

      subscriptions.Add(Pool.Subscribe<GlowStart>(_ => glowing = true));
      subscriptions.Add(Pool.Subscribe<GlowStop>(_ => glowing = false));
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private IEnumerator Glow()
    {
      while (true)
      {
        yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        if (!glowing)
        {
          tiles[Random.Range(0, tiles.Length)]
            .gameObject.AddComponent<HighlightStarter>();
        }
      }
    }
  }
}
