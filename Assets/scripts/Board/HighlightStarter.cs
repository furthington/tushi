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

      /* Shuffle tiles. TODO: Make generic shuffling? */
      for (int i = tiles.Count - 1; i > 0; --i)
      {
        int k = Random.Range(0, i + 1);
        Tile temp = tiles[k];
        tiles[k] = tiles[i];
        tiles[i] = temp;
      }
      //tiles.OrderBy(x => System.Guid.NewGuid());

      /* SHUFFLE CHECK */
      /*List<Tile> tiles_temp = new List<Tile>()
      {
        tile.bottom_left, tile.bottom_right, tile.right,
        tile.top_right, tile.top_left, tile.left
      };
      bool different = false;
      for (int i = 0; i < tiles.Count; ++i)
      {
        if (tiles[i] != tiles_temp[i])
        { different = true; break; }
      }
      if (different)
      { Logger.Log("DIFFERENT"); }*/

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
