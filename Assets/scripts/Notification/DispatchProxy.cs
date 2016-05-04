using UnityEngine;
using UnityEngine.EventSystems;

namespace Notification
{
  /* Works with buttons; inherit to create a concrete type and add it to a button. */
  public class DispatchProxy<T> : MonoBehaviour, IPointerClickHandler where T : new()
  {
    public void OnPointerClick(PointerEventData data)
    { Pool.Dispatch(new T()); }
  }
}
