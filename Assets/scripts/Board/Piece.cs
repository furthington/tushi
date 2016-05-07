using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
      Destroy(gameObject);
      foreach (Image img in block_images)
      { img.enabled = true; }
    }
  }
}
