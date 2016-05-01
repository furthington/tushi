﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Block : MonoBehaviour
  {
    private Tile currently_over = null;
    private GameObject canvas;

    private void Awake()
    { canvas = GameObject.FindGameObjectWithTag("main_canvas"); }

    public bool IsInValidPosition()
    { return (currently_over != null && currently_over.block == null) ; }

    public void PlaceInTile()
    {
      Debug.Assert(currently_over != null, "Trying to place block in invalid tile!");
      transform.SetParent(currently_over.transform);
      currently_over.block = this;

      /*TODO: remove when we no longer click tiles to do anything. */
      CanvasGroup cg = gameObject.AddComponent<CanvasGroup>();
      cg.blocksRaycasts = false;
      cg.interactable = false;
    }

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
  }
}