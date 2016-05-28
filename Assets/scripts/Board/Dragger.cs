using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  [RequireComponent (typeof(CanvasGroup))]
  public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    private GameObject currently_dragged;
    private List<Block> blocks = new List<Block>();
    private GameObject canvas;

    private void Awake()
    { canvas = GameObject.FindGameObjectWithTag("main_canvas"); }

    public void OnBeginDrag(PointerEventData eventData)
    {
      currently_dragged = Instantiate(gameObject);
      currently_dragged.transform.SetParent(canvas.transform);
      currently_dragged.transform.position = eventData.position;
      currently_dragged.GetComponentsInChildren<Block>(blocks);
      currently_dragged.GetComponentInChildren<ScalerToTile>(true).gameObject.SetActive(true);

      /* Have to manually set size probably because HorizontalLayoutGroup is messing with it? */
      currently_dragged.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;

      /* To raycast to the tiles under it */
      currently_dragged.GetComponent<CanvasGroup>().blocksRaycasts = false;

      GetComponent<CanvasGroup>().alpha = 0.0f;

      Notification.Pool.Dispatch(new GlowStart());
    }

    public void OnDrag(PointerEventData eventData)
    {
      currently_dragged.transform.position = eventData.position;
      RaycastResult r = eventData.pointerCurrentRaycast;
      Vector3 snap_correction = new Vector3();
      if (r.gameObject != null && r.gameObject.GetComponent<Tile>() != null)
      {
        snap_correction = (canvas.transform.worldToLocalMatrix * r.gameObject.transform.position) - new Vector4(eventData.position.x, eventData.position.y);

        int valid = 0;
        foreach (Block b in blocks)
        {
          b.OnDrag(snap_correction);
          if (b.IsInValidPosition())
          { ++valid; }
        }

        /* If they are all valid, snap to position.
         * This snapping assumes that you are dragging a block right under the cursor. */
        if (valid == blocks.Count)
        { currently_dragged.GetComponent<RectTransform>().position = r.gameObject.GetComponent<RectTransform>().position; }
      }
      else
      {
        foreach (Block b in blocks)
        { b.InvalidatePosition(); }
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      Notification.Pool.Dispatch(new GlowStop());

      int valid = 0;
      foreach (Block b in blocks)
      {
        if (b.IsInValidPosition())
        { ++valid; }
        else
        { break; }
      }

      if(valid == blocks.Count)
      {
        /* Placed pieces do not rotate anymore. */
        currently_dragged.GetComponentInChildren<Root>().ClearSubscriptions();

        foreach(Block b in blocks)
        { b.PlaceInTile(); }

        Piece p = currently_dragged.GetComponentInChildren<Piece>();
        if (p != null)
        {
          p.transform.SetParent(canvas.transform);
          p.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        Notification.Pool.Dispatch(new AddNewPiece());
        Destroy(gameObject);
      }
      else
      {
        GetComponent<CanvasGroup>().alpha = 1.0f;
      }

      Destroy(currently_dragged);
    }
  }
}
