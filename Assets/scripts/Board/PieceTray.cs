using UnityEngine;using System.Collections.Generic;namespace Board{  public class PieceTray : MonoBehaviour  {    public List<GameObject> PrefabsLowProb;    public List<GameObject> PrefabsHighProb;    public void Regenerate()    {      foreach (Transform child in transform)      { Destroy(child.gameObject); }      for(int i = 0; i < 3; ++i)      {
        if(Random.Range(0.0f, 1.0f) <= 0.2f)
        { Instantiate(PrefabsLowProb[Random.Range(0, PrefabsLowProb.Count)]).transform.SetParent(transform, false); }
        else
        { Instantiate(PrefabsHighProb[Random.Range(0, PrefabsHighProb.Count)]).transform.SetParent(transform, false); }
      }    }    public void Rotate()    {      foreach (Transform child in transform)      { child.Rotate(new Vector3(0, 0, -60)); }    }  }}