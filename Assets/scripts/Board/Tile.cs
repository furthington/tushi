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
    public List<Tile> line0;
    public List<Tile> line1;
    public List<Tile> line2;

    public Block block = null;

    public void Start()
    {
      Debug.Log("line0: " + line0.Count.ToString());
      Debug.Log("line1: " + line0.Count.ToString());
      Debug.Log("line2: " + line0.Count.ToString());
    }

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
