using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Board
{
  [RequireComponent(typeof(Tile))]
  [RequireComponent(typeof(Highlighter))]
  public class HighlightStarter : MonoBehaviour
  {
    private void Awake()
    {
      if (GetComponents<HighlightStarter>()[0] != this)
      { Destroy(this); }
    }

    private void Start()
    { StartCoroutine(Glow()); }

    private IEnumerator Glow()
    {
      Tile tile = GetComponent<Tile>();
      GetComponent<Highlighter>().Glow();

      List<Tile> tiles = new List<Tile>()
      {
        tile.bottom_left, tile.bottom_right, tile.right,
        tile.top_right, tile.top_left, tile.left
      };

      /* Shuffle tiles. */
      tiles = tiles.OrderBy(x => System.Guid.NewGuid()).ToList();

      for (int i = 0; i < tiles.Count; ++i)
      {
        if (tiles[i] == null || tiles[i].GetComponent<HighlightStarter>() != null)
        { continue; }
        yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
        tiles[i].gameObject.AddComponent<HighlightStarter>();
      }

      yield return new WaitForSeconds(5.0f);
      StopAllCoroutines();
      Destroy(this);
    }
  }
}
