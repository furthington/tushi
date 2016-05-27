using UnityEngine;
using UnityEngine.UI;

namespace Board
{
  [RequireComponent(typeof(Image))]
  public class Tinter : MonoBehaviour
  {
    private void Start()
    {
      Image img = GetComponent<Image>();
      Color clr = img.color;
      clr.r *= Random.Range(0.5f, 1.0f);
      clr.g *= Random.Range(0.5f, 1.0f);
      clr.b *= Random.Range(0.5f, 1.0f);
      img.color = clr;
    }

  }
}
