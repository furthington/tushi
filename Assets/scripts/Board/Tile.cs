using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Tile : MonoBehaviour, IPointerClickHandler
  {
    public Tile top_right;
    public Tile right;
    public Tile bottom_right;
    public Tile bottom_left;
    public Tile left;
    public Tile top_left;
    public List<Tile> flat_lines; /* null-separated */
    public List<List<Tile>> lines;

    public Block block = null;

    public void Start()
    { Debug.Log(flat_lines.Count.ToString()); }

    public void OnPointerClick(PointerEventData data)
    {
      Debug.Log("DING!");
      if(block != null)
      {
        block.Remove();
        block = null;
      }
    }
  }
}
