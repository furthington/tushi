using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class ScalerToTray : MonoBehaviour
  {
    private Vector2 original_size = new Vector2();
    private void Awake()
    { original_size = GetComponent<RectTransform>().sizeDelta; }
    private void Start()
    {
      float scale_amount = Screen.width / Mathf.Max(original_size.x, original_size.y) * 0.3f;
      //Debug.Log("scale amount " + scale_amount);
      Vector3 scale = transform.localScale;
      scale.x = scale.y = scale_amount;
      transform.localScale = scale;

      /* Only scale once! Prevents clones from scaling again and again */
      Destroy(this);
    }
  }
}
