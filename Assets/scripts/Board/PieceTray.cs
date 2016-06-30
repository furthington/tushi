using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Notification;
using Save;
using Error;

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
      public float[][] Rotations
      { get; set; }

      public SaveReply(string[] n, float[][] r)
      {
        Names = n;
        Rotations = r;
      }
    }
    public class Load
    {
      public string[] Names
      { get; set; }
      public float[][] Rotations
      { get; set; }

      public Load(string[] n, float[][] r)
      {
        Names = n;
        Rotations = r;
      }
    }

    public List<GameObject> PrefabsLowProb;
    public List<GameObject> PrefabsHighProb;

    public List<GameObject> PrefabsRiceLowProb;
    public List<GameObject> PrefabsRiceHighProb;

    private const float rice_prob_min = 0.143f;
    private float rice_prob = 0.0f;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    private void Start()
    {
      rice_prob = rice_prob_min;

      subscriptions.Add<AddNewPiece>(_ => AddPiece());
      subscriptions.Add<Save>(_ => OnSave());
      subscriptions.Add<Load>(OnLoad);
      subscriptions.Add<NewGame>(_ => InitializePieces());
    }

    private void InitializePieces()
    {
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
      /* Start it off screen to avoid flickering on screen before positioning kicks in. */
      new_child.transform.position = new Vector3(-4000.0f, 0.0f);
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
        return (prob <= 0.2f)
                 ? RandomFrom(PrefabsRiceLowProb)
                 : RandomFrom(PrefabsRiceHighProb);
      }
      else
      {
        /* Rice probability goes up */
        rice_prob *= 1.5f;
        return (prob <= 0.2f)
                 ? RandomFrom(PrefabsLowProb)
                 : RandomFrom(PrefabsHighProb);
      }
    }

    private GameObject RandomFrom(List<GameObject> prefabs)
    { return Instantiate(prefabs[Random.Range(0, prefabs.Count)]); }

    /* Editor needs this to be non-static. */
    public void Rotate()
    { Pool.Dispatch(new RotatePieces()); }

    private void OnSave()
    {
      using(var timer = new Profile.TaskTimer("Save piece tray"))
      {
        var names = new List<string>();
        var rotations = new List<float[]>();
        foreach (Transform child in transform)
        {
          names.Add(child.name.Replace("(Clone)", ""));
          rotations.Add
          (
            new float[]
            {
              child.rotation.x,
              child.rotation.y,
              child.rotation.z,
              child.rotation.w
            }
          );
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
    }

    private void OnLoad(Load l)
    {
      Assert.Invariant(l.Names.Length == l.Rotations.Length,
                       "Different amount of names and rotations");

      using(var timer = new Profile.TaskTimer("Load piece tray"))
      {
        for(int i = 0; i < l.Names.Length; ++i)
        {
          var prefab = Resources.Load("piece_tray/" + l.Names[i]);
          Assert.Invariant(prefab != null,
                           "Invalid prefab for name " + l.Names[i]);
          var obj = Instantiate(prefab) as GameObject;
          obj.transform.rotation = new Quaternion
          (
            l.Rotations[i][0],
            l.Rotations[i][1],
            l.Rotations[i][2],
            l.Rotations[i][3]
          );
          obj.transform.SetParent(transform);
        }
      }
    }
  }
}
