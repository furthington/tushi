using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Notification;

namespace Help
{
  public class Populator : MonoBehaviour
  {
    /* Assign all in editor. */
    public Image current;
    public Image next;
    public AnimationCurve curve;
    public List<Sprite> sprites;
    public float transit_time;
    public float wait_time;
    public List<Animator> animators;

    /* TODO: Start, Close, and DelayDestroy are basically the same thing from Menu.Populator. Refactor? */
    private void Start()
    {
      Pool.Dispatch(new UI.AddBackButtonHandler(gameObject, Close));
      StartCoroutine(Scroll());
    }

    private IEnumerator Scroll()
    {
      /* Image size is still invalid at start, give it one frame to be set. */
      yield return new WaitForEndOfFrame();
      float height = current.rectTransform.rect.height;
      Vector2 origin = current.rectTransform.anchoredPosition;
      int current_index = Random.Range(0, sprites.Count);
      int next_index = (Random.Range(1, sprites.Count) + current_index) % sprites.Count;
      current.sprite = sprites[current_index];
      next.sprite = sprites[next_index];
      while (true)
      {
        /* Reset image positions. */
        Vector2 pos = origin;
        pos.y += height;
        next.rectTransform.anchoredPosition = pos;
        current.rectTransform.anchoredPosition = origin;
        yield return new WaitForSeconds(wait_time);

        /* Play transition. */
        float start_time = Time.time;
        float elapsed = 0.0f;
        while (elapsed < transit_time)
        {
          pos.y = origin.y - curve.Evaluate(elapsed / transit_time) * height;
          current.rectTransform.anchoredPosition = pos;
          pos.y += height;
          next.rectTransform.anchoredPosition = pos;
          yield return null;
          elapsed = Time.time - start_time;
        }

        /* Roll next sprite. */
        current.sprite = next.sprite;
        next_index = (Random.Range(1, sprites.Count) + next_index) % sprites.Count;
        next.sprite = sprites[next_index];
      }
    }

    public void Close()
    {
      Pool.Dispatch(new UI.RemoveBackButtonHandler(gameObject));
      foreach (Animator a in animators)
      { a.SetTrigger("exit"); }
      StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
      yield return new WaitForSeconds(0.5f);
      Destroy(gameObject);
    }
  }
}
