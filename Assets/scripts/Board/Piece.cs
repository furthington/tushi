using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Error;

namespace Board
{
  public class Piece : MonoBehaviour
  {
    public List<Image> block_images = new List<Image>(); /* Assign in editor */

    public void OnPlacement()
    {
      Assert.Invariant
      (
        GetComponent<PieceIdentifier>() == null,
        "Piece already placed"
      );
      gameObject.AddComponent<PieceIdentifier>();
    }

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
