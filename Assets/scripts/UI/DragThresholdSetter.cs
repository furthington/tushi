using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
  public class DragThresholdSetter : MonoBehaviour
  {
    void Start()
    { EventSystem.current.pixelDragThreshold = 10; }

  }
}
