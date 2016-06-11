using UnityEngine;
using Notification;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI.Floater
{
  public class Show
  {
    public string Txt
    { get; set; }

    public Vector3 Position
    { get; set; }

    public Show(string s, Vector3 pos)
    {
      Txt = s;
      Position = pos;
    }
  }

  public class Spawner : MonoBehaviour
  {
    public GameObject prefab; /* Assign in editor. */
    private SubscriptionStack subscriptions = new SubscriptionStack();
    private List<int> scores = new List<int>();

    private void Start()
    {
      subscriptions.Add<Show>(Create);
      subscriptions.Add<Board.AddScore>
      (
        score =>
        {
          if (scores.Count == 0)
          { Create("+" + score.Score); }
          scores.Add(score.Score);
        }
      );
      subscriptions.Add<Destroyed>
      (
        _ =>
        {
          scores.RemoveAt(0);
          if (scores.Count > 0)
          { Create("+" + scores[0]); }
        }
      );
    }

    /* Automatically sets position based on parent. */
    private void Create(string text)
    {
      GameObject floater = GameObject.Instantiate(prefab);
      floater.GetComponent<Text>().text = text;
      floater.transform.SetParent(transform, false);
    }

    /* Position set based on world position supplied by Show. */
    private void Create(Show s)
    {
      GameObject floater = GameObject.Instantiate(prefab);
      floater.GetComponent<Text>().text = s.Txt;
      floater.transform.position = s.Position;
      floater.transform.SetParent(transform);
    }

    private void OnDisable()
    { subscriptions.Clear(); }
  }
}
