using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Notification;
using Error;

namespace Board
{
  public class Block : MonoBehaviour
  {
    public Piece piece; /* Assign in editor */
    public string piece_id; /* Loaded from saved game */
    private Tile currently_over = null;
    private GameObject canvas;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Awake()
    {
      canvas = GameObject.FindGameObjectWithTag("main_canvas");
      subscriptions.Add<PieceLoader.Loaded>(OnPieceLoaded);
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public bool IsInValidPosition()
    { return (currently_over != null && currently_over.block == null) ; }

    public void PlaceInTile()
    {
      Assert.Invariant
      (
        currently_over != null,
        "Trying to place block in invalid tile!"
      );
      transform.SetParent(canvas.transform);
      currently_over.Emplace(this);
    }

    public void InvalidatePosition()
    { currently_over = null; }

    public void OnDrag(Vector3 snap_correction)
    {
      /* Raycast from object's position */
      PointerEventData p = new PointerEventData(EventSystem.current);
      Vector3 pos = canvas.transform.worldToLocalMatrix * transform.position;
      pos += snap_correction;
      p.position = pos;
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(p, results);
      foreach(RaycastResult result in results)
      {
        if(result.gameObject != null)
        {
          currently_over = result.gameObject.GetComponent<Tile>();
          if(currently_over != null)
          { return; }
        }
      }
      currently_over = null;
    }

    public void DeferredRemove(int delay)
    {
      /* Destroy the big piece and show the little blocks. */
      if (!GetComponent<Image>().enabled)
      { piece.BreakDown(); }

      /* DeferredRemove could be called multiple times on one tile. */
      StopAllCoroutines();

      StartCoroutine(DeferredRemoveImpl(delay));
    }

    private IEnumerator DeferredRemoveImpl(int delay)
    {
      if(delay > 0)
      { yield return new WaitForSeconds(0.05f * delay); }

      float scale = transform.localScale.x;
      float delta = scale * 0.15f;
      Vector3 new_scale = transform.localScale;

      while(scale > 0.0f)
      {
        scale -= delta;
        if (scale < 0.0f)
        { break; }
        new_scale.x = new_scale.y = scale;
        transform.localScale = new_scale;
        yield return null;
      }

      Destroy(gameObject);
    }

    private void OnPieceLoaded(PieceLoader.Loaded l)
    {
      if(l.ID != piece_id)
      { return; }
      piece = l.Item;
      var img = GetComponent<Image>();
      img.enabled = false;
      piece.block_images.Add(img);
    }
  }
}
