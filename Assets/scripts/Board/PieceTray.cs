using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public class PieceTray : MonoBehaviour
  {
    public List<GameObject> PrefabsLowProb;
    public List<GameObject> PrefabsHighProb;

    public List<GameObject> PrefabsRiceLowProb;
    public List<GameObject> PrefabsRiceHighProb;

    private const float rice_prob_min = 0.1f;
    private float rice_prob = 0.0f;

    private void Start()
    { rice_prob = rice_prob_min; }

    public void UsePiece(int i)
    {
      Destroy(transform.GetChild(i).gameObject);
      GameObject new_child = GeneratePiece();
      new_child.transform.SetParent(transform, false);
      new_child.transform.SetSiblingIndex(i);
    }

    private GameObject GeneratePiece()
    {
      /* Check high or low prob */
      float prob = Random.Range(0.0f, 1.0f);

      /* Check if we should generate rice */
      if (Random.Range(0.0f, 1.0f) <= rice_prob)
      {
        /* Rice probability resets */
        rice_prob = rice_prob_min;
        return (prob <= 0.1f) ? RandomFrom(PrefabsRiceLowProb) : RandomFrom(PrefabsRiceHighProb);
      }
      else
      {
        /* Rice probability goes up */
        rice_prob *= 1.5f;
        return (prob <= 0.1f) ? RandomFrom(PrefabsLowProb) : RandomFrom(PrefabsHighProb);
      }
    }

    private GameObject RandomFrom(List<GameObject> prefabs)
    { return Instantiate(prefabs[Random.Range(0, prefabs.Count)]); }

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
