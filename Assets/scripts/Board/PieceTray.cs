using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Notification;

namespace Board
{
  public class AddNewPiece { }

  public class PieceTray : MonoBehaviour
  {
    public class Save
    { }
    [System.Serializable]
    public class SaveReply
    {
      public string[] Names
      { get; set; }
      public float[] Rotations
      { get; set; }

      public SaveReply(string[] n, float[] r)
      {
        Names = n;
        Rotations = r;
      }
    }
    public class Load
    {
      public string[] Names
      { get; set; }
      public float[] Rotations
      { get; set; }

      public Load(string[] n, float[] r)
      {
        Names = n;
        Rotations = r;
      }
    }

    public List<GameObject> PrefabsLowProb;
    public List<GameObject> PrefabsHighProb;

    public List<GameObject> PrefabsRiceLowProb;
    public List<GameObject> PrefabsRiceHighProb;

    private const float rice_prob_min = 0.1f;
    private float rice_prob = 0.0f;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      rice_prob = rice_prob_min;

      subscriptions.Add
      (
        Pool.Subscribe<AddNewPiece>
        (_ => AddPiece())
      );
      subscriptions.Add(Pool.Subscribe<Save>(_ => OnSave()));
      subscriptions.Add(Pool.Subscribe<Load>(OnLoad));

      /* Need to add pieces only after tiles are created,
       * since they are scaled based on the size of the tiles. */
      StartCoroutine(InitializePieces());
    }

    private IEnumerator InitializePieces()
    {
      yield return new WaitForEndOfFrame();
      for (int i = 0; i < 3; ++i)
      { AddPiece(); }
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    public void AddPiece()
    {
      GameObject new_child = GeneratePiece();
      new_child.transform.SetParent(transform, false);
      new_child.transform.SetAsLastSibling();
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
        return (prob <= 0.1f)
                 ? RandomFrom(PrefabsRiceLowProb)
                 : RandomFrom(PrefabsRiceHighProb);
      }
      else
      {
        /* Rice probability goes up */
        rice_prob *= 1.5f;
        return (prob <= 0.1f)
                 ? RandomFrom(PrefabsLowProb)
                 : RandomFrom(PrefabsHighProb);
      }
    }

    private GameObject RandomFrom(List<GameObject> prefabs)
    { return Instantiate(prefabs[Random.Range(0, prefabs.Count)]); }

    public void Rotate()
    {
      foreach (Transform child in transform)
      { child.Rotate(new Vector3(0, 0, -60)); }
      Pool.Dispatch(new RotateNeighbours());
    }

    private void OnSave()
    {
      var names = new List<string>();
      var rotations = new List<float>();
      foreach (Transform child in transform)
      {
        names.Add(child.name.Replace("(Clone)", ""));
        rotations.Add(child.rotation.z);
      }
      Pool.Dispatch
      (
        new SaveReply
        (
          names.ToArray(),
          rotations.ToArray()
        )
      );
    }

    private void OnLoad(Load l)
    {
      Debug.Assert(l.Names.Length == l.Rotations.Length,
                   "Different amount of names and rotations");
      for(int i = 0; i < l.Names.Length; ++i)
      {
        var prefab = Resources.Load("piece/" + l.Names[i]);
        Debug.Assert(prefab != null, "Invalid prefab for name " + l.Names[i]);
        var obj = Instantiate(prefab) as GameObject;
        obj.transform.rotation = new Quaternion(0, 0, l.Rotations[i], 1);
        obj.transform.SetParent(transform);
      }
    }
  }
}
