using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Board
{
  public class ColorPicker : MonoBehaviour, IPointerClickHandler
  {
    private Image img;

    public void Start()
    { img = GetComponent<Image>(); }

    public void OnPointerClick(PointerEventData data)
    {
      Image color_holder = GameObject.FindGameObjectWithTag("color").GetComponent<Image>();
      color_holder.color = img.color;
    }
  }
}
