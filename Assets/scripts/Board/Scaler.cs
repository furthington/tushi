using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Board
{
  public class Scaler : MonoBehaviour
  {
    private void Start()
    {
      RectTransform tile_rect = GameObject.FindObjectOfType<Tile>().GetComponent<RectTransform>();
      //Debug.Log("tile size x " + tile_rect.sizeDelta.x + ", my size x " + GetComponent<RectTransform>().sizeDelta.x);
      float scale_amount = tile_rect.sizeDelta.x / GetComponent<RectTransform>().sizeDelta.x;
      //Debug.Log("scale amount " + scale_amount);
      Vector3 scale = transform.parent.localScale;
      scale *= scale_amount;
      transform.parent.localScale = scale;

      /* Only scale once! Prevents clones from scaling again and again */
      Destroy(this);
    }
  }
}
