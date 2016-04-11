using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public class PieceTray : MonoBehaviour
  {
    public List<GameObject> Prefabs;

    public void Regenerate()
    {
      foreach (Transform child in transform)
      { Destroy(child.gameObject); }

      for(int i = 0; i < 3; ++i)
      { Instantiate(Prefabs[Random.Range(0, Prefabs.Count)]).transform.SetParent(transform, false); }
    }

    public void Rotate()
    {
      foreach (Transform child in transform)
      { child.Rotate(new Vector3(0, 0, -60)); }
    }
  }
}
