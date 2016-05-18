using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Board
{
  public class Piece : MonoBehaviour
  {
    public List<Image> block_images; /* Assign in editor */

    public void BreakDown()
    { StartCoroutine(DeferredRemove()); }

    private IEnumerator DeferredRemove()
    {
      yield return new WaitForEndOfFrame();
      foreach (Image img in block_images)
      { img.enabled = true; }
      Destroy(gameObject);
    }
  }
}
