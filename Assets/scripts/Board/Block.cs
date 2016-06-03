﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Board
{
  public class Block : MonoBehaviour
  {
    public Piece piece; /* Assign in editor */
    private Tile currently_over = null;
    private GameObject canvas;

    private void Awake()
    { canvas = GameObject.FindGameObjectWithTag("main_canvas"); }

    public bool IsInValidPosition()
    { return (currently_over != null && currently_over.block == null) ; }

    public void PlaceInTile()
    {
      Debug.Assert
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

    public void Remove()
    { Destroy(gameObject); }

    public void DeferredRemove()
    {
      /* Destroy the big piece and show the little blocks. */
      if (!GetComponent<Image>().enabled)
      { piece.BreakDown(); }

      StartCoroutine(DeferredRemoveImpl());
    }

    private IEnumerator DeferredRemoveImpl()
    {
      /* Wait for this frame to finish. */
      yield return new WaitForEndOfFrame();
      Remove();
    }
  }
}
