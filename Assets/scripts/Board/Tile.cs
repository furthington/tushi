using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Tile : MonoBehaviour, IPointerClickHandler
  {
    public List<Tile> SurroundingTiles;
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
