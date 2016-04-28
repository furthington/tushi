using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public class PieceTray : MonoBehaviour
  {
    public List<GameObject> PrefabsLowProb;
    public List<GameObject> PrefabsHighProb;

    public void UsePiece(int i)
    {
      Destroy(transform.GetChild(i).gameObject);

      GameObject new_child;
      if(Random.Range(0.0f, 1.0f) <= 0.1f)
      { new_child = Instantiate(PrefabsLowProb[Random.Range(0, PrefabsLowProb.Count)]); }
      else
      { new_child = Instantiate(PrefabsHighProb[Random.Range(0, PrefabsHighProb.Count)]); }

      new_child.transform.SetParent(transform, false);
      new_child.transform.SetSiblingIndex(i);
    }

    public void Rotate()
    {
      foreach (Transform child in transform)
      { child.Rotate(new Vector3(0, 0, -60)); }
    }

    public void Mirror()
    {
      foreach (Transform child in transform)
      {
        Vector3 scale = child.localScale;
        scale.x *= -1;
        child.localScale = scale;
      }
    }
  }
}
