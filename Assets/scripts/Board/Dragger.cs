using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Notification;

namespace Board
{
  public class PiecePlaced
  { }

  [RequireComponent (typeof(CanvasGroup))]
  public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    public GameObject piece_prefab;
    private GameObject currently_dragged;
    private GameObject canvas;
    private float drag_offset;
    private List<Block> blocks = new List<Block>();
    private List<Image> guides = new List<Image>();
    private List<Image> hide = new List<Image>();

    private void Awake()
    {
      canvas = GameObject.FindGameObjectWithTag("main_canvas");
      drag_offset = Screen.height * 0.1f;
      foreach (Transform child in transform)
      {
        if (child.gameObject.tag == "guide")
        { guides.Add(child.GetComponent<Image>()); }
        else if (child.gameObject.tag == "hide")
        { hide.Add(child.GetComponent<Image>()); }
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      currently_dragged = Instantiate(piece_prefab);
      currently_dragged.transform.SetParent(canvas.transform);
      currently_dragged.transform.position = eventData.position;
      currently_dragged.transform.rotation = transform.rotation;
      currently_dragged.GetComponentsInChildren<Block>(blocks);
      currently_dragged.GetComponentInChildren<ScalerToTile>(true).gameObject.SetActive(true);

      /* Have to manually set size probably because HorizontalLayoutGroup is messing with it? */
      currently_dragged.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;

      /* To raycast to the tiles under it */
      currently_dragged.GetComponent<CanvasGroup>().blocksRaycasts = false;

      foreach (Image img in hide)
      { img.enabled = false; }
      foreach (Image img in guides)
      { img.enabled = true; }

      Pool.Dispatch(new GlowStart());
    }

    public void OnDrag(PointerEventData eventData)
    {
      Vector3 new_pos = eventData.position;
      new_pos.y += drag_offset;
      currently_dragged.transform.position = new_pos;

      /* Manual raycast since piece is no longer dragging under cursor. */
      PointerEventData p = new PointerEventData(EventSystem.current);
      p.position = canvas.transform.worldToLocalMatrix * new_pos;
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(p, results);

      if (results.Count == 0)
      {
        foreach (Block b in blocks)
        { b.InvalidatePosition(); }
        return;
      }

      RaycastResult r = results[0];
      if (r.gameObject != null && r.gameObject.GetComponent<Tile>() != null)
      {
        Vector3 snap_correction = (canvas.transform.worldToLocalMatrix * r.gameObject.transform.position)
                                    - new Vector4(new_pos.x, new_pos.y);

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
        { currently_dragged.transform.position = r.gameObject.GetComponent<RectTransform>().position; }
      }
      else
      {
        foreach (Block b in blocks)
        { b.InvalidatePosition(); }
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      Pool.Dispatch(new GlowStop());

      foreach (Image img in hide)
      { img.enabled = true; }
      foreach (Image img in guides)
      { img.enabled = false; }

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
          p.OnPlacement();
        }

        Destroy(gameObject);
        Pool.Dispatch(new AddNewPiece());
        Pool.Dispatch(new PiecePlaced());
      }

      Destroy(currently_dragged);
    }
  }
}
