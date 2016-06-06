using System;
using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace Board
{
  public class PieceIdentifier : MonoBehaviour
  {
    private string id;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    public string ID
    {
      get
      { return id; }
      set
      { id = value; }
    }

    public class Save
    { }
    [Serializable]
    public class SaveReply
    {
      public string ID
      { get; set; }
      public string Name
      { get; set; }
      public float[] Position
      { get; set; }
      public float[] Scale
      { get; set; }
      public float[] Rotation
      { get; set; }

      public SaveReply(string id, string n, float[] p, float[] s, float[] r)
      {
        ID = id;
        Name = n;
        Position = p;
        Scale = s;
        Rotation = r;
      }
    }

    private void Start()
    {
      id = Guid.NewGuid().ToString();
      subscriptions.Add<Save>
      (
        _ =>
        Pool.Dispatch
        (
          new SaveReply
          (
            id,
            GetComponent<Image>().sprite.name,
            new float[]
            {
              gameObject.transform.position.x,
              gameObject.transform.position.y,
              gameObject.transform.position.z
            },
            new float[]
            {
              gameObject.transform.localScale.x,
              gameObject.transform.localScale.y,
              gameObject.transform.localScale.z
            },
            new float[]
            {
              gameObject.transform.rotation.x,
              gameObject.transform.rotation.y,
              gameObject.transform.rotation.z,
              gameObject.transform.rotation.w
            }
          )
        )
      );
    }

    private void OnDisable()
    { subscriptions.Clear(); }
  }
}
