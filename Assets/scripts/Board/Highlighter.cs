using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Notification;

namespace Board
{
  public class GlowStart { }
  public class GlowStop { }
  public class GlowOnce { }
  [RequireComponent (typeof(Image))]
  public class Highlighter : MonoBehaviour
  {
    public AnimationCurve curve; /* To set in editor. Can't make it static! */
    private float start_time;
    private Image img;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      img = GetComponent<Image>();

      subscriptions.Add(Pool.Subscribe<GlowOnce>(_ => Glow()));
      subscriptions.Add(Pool.Subscribe<GlowStart>(_ => StartGlow()));
      subscriptions.Add(Pool.Subscribe<GlowStart>(_ => StopGlow()));
    }

    private void StartGlow()
    {
      StopAllCoroutines();
      start_time = Time.time;
      start_time += Random.Range(0.0f, 0.1f);
      StartCoroutine(GlowAux());
    }

    private void StopGlow()
    {

    }

    private void Glow()
    {

    }

    private IEnumerator GlowAux()
    {
      while (true)
      {
        Color clr = img.color;
        clr.a = curve.Evaluate(Time.time - start_time);
        img.color = clr;
        yield return null;
      }
    }

    private void OnDisable()
    { subscriptions.Clear(); }

  }
}
