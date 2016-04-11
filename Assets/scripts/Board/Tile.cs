using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Tile : MonoBehaviour, IPointerClickHandler
  {
    public Tile top;
    public Tile top_right;
    public Tile right;
    public Tile bottom_right;
    public Tile bottom;
    public Tile bottom_left;
    public Tile left;
    public Tile top_left;

    private Image img;

    public void Start()
    { img = GetComponent<Image>(); }

    public void OnPointerClick(PointerEventData data)
    {
      Image color_holder = GameObject.FindGameObjectWithTag("color").GetComponent<Image>();
      img.color = color_holder.color;
    }
  }
}
