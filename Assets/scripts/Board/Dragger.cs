using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    private GameObject currently_dragged;
    private List<Block> blocks = new List<Block>();

    public void OnBeginDrag(PointerEventData eventData)
    {
      GameObject canvas = GameObject.FindGameObjectWithTag("main_canvas");
      currently_dragged = Instantiate(gameObject);
      currently_dragged.transform.SetParent(canvas.transform);
      currently_dragged.GetComponent<RectTransform>().anchoredPosition = eventData.position;
      currently_dragged.GetComponentsInChildren<Block>(blocks);

      /* Have to manually set size probably because HorizontalLayoutGroup is messing with it? */
      currently_dragged.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;

      /* To raycast to the tiles under it */
      currently_dragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
      currently_dragged.GetComponent<RectTransform>().anchoredPosition = eventData.position;

      int valid = 0;
      foreach(Block b in blocks)
      {
        b.OnDrag();
        if(b.IsInValidPosition())
        { ++valid; }
      }
      Debug.Log("=====");

      /* If they are all valid, snap to position.
       * This snapping assumes that you are dragging a block right under the cursor. */
      if(valid == blocks.Count)
      { currently_dragged.GetComponent<RectTransform>().position = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>().position; }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      int valid = 0;
      foreach (Block b in blocks)
      {
        if (b.IsInValidPosition())
        { ++valid; }
      }

      if(valid == blocks.Count)
      {
        foreach(Block b in blocks)
        { b.PlaceInTile(); }
      }

      Destroy(currently_dragged);
    }
  }
}
