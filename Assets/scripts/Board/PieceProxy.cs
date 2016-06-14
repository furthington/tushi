using UnityEngine;
using UnityEngine.EventSystems;
using Error;

namespace Board
{
  public class PieceProxy : MonoBehaviour,
  IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
  {
    private int index;
    private GameObject tray;
    private Dragger dragger = null;

    private void Start()
    {
      index = transform.GetSiblingIndex();
      tray = GameObject.FindGameObjectWithTag("tray");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      Transform child = tray.transform.GetChild(index);
      Assert.Invariant(child != null, "Piece tray has no child of index " + index);
      dragger = child.GetComponent<Dragger>();
      Assert.Invariant(dragger != null, "Piece tray's child " + index + " has no dragger!");
      dragger.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (dragger != null)
      { dragger.OnDrag(eventData); }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (dragger != null)
      { dragger.OnEndDrag(eventData); }
    }

    public void OnPointerClick(PointerEventData data)
    {
      Transform child = tray.transform.GetChild(index);
      Assert.Invariant(child != null, "Piece tray has no child of index " + index);
      Rotater r = child.GetComponent<Rotater>();
      Assert.Invariant(r != null, "Piece tray's child " + index + " has no rotater!");
      r.OnPointerClick(data);
    }
  }
}
