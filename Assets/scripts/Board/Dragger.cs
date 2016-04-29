using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    private GameObject currently_dragged;
    private void Start()
    {}

    public void OnBeginDrag(PointerEventData eventData)
    {
      GameObject canvas = GameObject.FindGameObjectWithTag("main_canvas");
      currently_dragged = Instantiate(gameObject);
      currently_dragged.transform.SetParent(canvas.transform);
      currently_dragged.GetComponent<RectTransform>().anchoredPosition = eventData.position;

      /* Have to manually set size probably because HorizontalLayoutGroup is messing with it? */
      currently_dragged.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;

      /* To raycast to the tiles under it */
      currently_dragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
      currently_dragged.GetComponent<RectTransform>().anchoredPosition = eventData.position;
      if(eventData.pointerCurrentRaycast.gameObject != null)
      {
        Tile t = eventData.pointerCurrentRaycast.gameObject.GetComponent<Tile>();
        if(t != null)
        {
          currently_dragged.GetComponent<RectTransform>().position = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>().position;
        }
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      currently_dragged.GetComponent<CanvasGroup>().blocksRaycasts = true;

      /* Can't really place it for now */
      Destroy(currently_dragged);
    }
  }
}
