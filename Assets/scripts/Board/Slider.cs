using UnityEngine;
using System.Collections;

namespace Board
{
  public class Slider : MonoBehaviour
  {
    public Vector3 end_pos; /* Assigned by PiecePositioner. */
    public AnimationCurve curve; /* Assigned by PiecePositioner. */

    private const float slide_time = 0.5f;

    private void Awake()
    {
      Slider[] all_sliders = GetComponents<Slider>();
      /* Destroy all older sliders. */
      foreach (Slider s in all_sliders)
      {
        if (s == this)
        { continue; }
        Destroy(s);
      }
    }

    private void Start()
    { StartCoroutine(Move()); }

    private IEnumerator Move()
    {
      Vector3 start_pos = transform.position;
      float start_time = Time.time;
      float elapsed = Time.time - start_time;
      /* TODO: Replace 0.01f with a better epsilon. */
      while (elapsed < slide_time)
      {
        transform.position = Vector3.LerpUnclamped
                              (
                                start_pos,
                                end_pos,
                                curve.Evaluate(elapsed / slide_time)
                              );
        yield return null;
        elapsed = Time.time - start_time;
      }
      transform.position = end_pos;
      Destroy(this);
    }
  }
}
