using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Notification;

namespace Board
{
  public class GlowStart { }
  public class GlowStop { }
  [RequireComponent(typeof(Image))]
  [RequireComponent (typeof(Tile))]
  public class Highlighter : MonoBehaviour
  {
    public AnimationCurve curve; /* To set in editor. Can't make it static! */
    public AnimationCurve curve_short; /* To set in editor. Can't make it static! */

    private float start_time;
    private float total_time;
    private Image img;
    private Tile tile;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      img = GetComponent<Image>();
      tile = GetComponent<Tile>();
      total_time = curve_short.keys[curve_short.length - 1].time;

      subscriptions.Add<GlowStart>(_ => StartGlow());
      subscriptions.Add<GlowStop>(_ => StopGlow());
    }

    private void StartGlow()
    {
      StopAllCoroutines();

      HighlightStarter h = GetComponent<HighlightStarter>();
      if (h != null)
      { Destroy(h); }

      if (tile.block != null)
      { return; }

      start_time = Time.time;
      start_time -= Random.Range(0.0f, 0.25f);
      StartCoroutine(StartGlowAux());
    }

    private void StopGlow()
    {
      StopAllCoroutines();
      StartCoroutine(StopGlowAux());
    }

    public void Glow()
    {
      if (tile.block != null)
      { return; }

      start_time = Time.time;
      start_time -= Random.Range(0.0f, 0.1f);
      StartCoroutine(GlowAux());
    }

    private IEnumerator StartGlowAux()
    {
      while (true)
      {
        Color clr = img.color;
        clr.a = curve.Evaluate(Time.time - start_time);
        img.color = clr;
        yield return null;
      }
    }

    private IEnumerator StopGlowAux()
    {
      while (img.color.a > 0.0f)
      {
        Color clr = img.color;
        clr.a = Mathf.Max(0.0f, clr.a - 0.015f);
        img.color = clr;
        yield return null;
      }
    }

    private IEnumerator GlowAux()
    {
      float elapsed = Time.time - start_time;
      while(elapsed < total_time)
      {
        Color clr = img.color;
        clr.a = curve_short.Evaluate(elapsed);
        img.color = clr;
        yield return null;
        elapsed = Time.time - start_time;
      }
    }

    private void OnDisable()
    { subscriptions.Clear(); }

  }
}
