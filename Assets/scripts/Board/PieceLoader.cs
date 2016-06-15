using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace Board
{
  public class PieceLoader : MonoBehaviour
  {
    private SubscriptionStack subscriptions = new SubscriptionStack();

    public class Load
    {
      public PieceIdentifier.SaveReply Item
      { get; set; }

      public Load(PieceIdentifier.SaveReply i)
      { Item = i; }
    }

    public class Loaded
    {
      public string ID
      { get; set; }
      public Piece Item
      { get; set; }

      public Loaded(string id, Piece item)
      {
        ID = id;
        Item = item;
      }
    }

    private void Start()
    { subscriptions.Add<Load>(OnLoadPiece); }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void OnLoadPiece(Load l)
    {
      using(var timer = new Profile.TaskTimer("Load piece"))
      {
        /* TODO: Cache this? */
        var canvas = GameObject.FindGameObjectWithTag("main_canvas");

        /* TODO: Copied from Row loading */
        var obj = new GameObject().AddComponent<Piece>();
        obj.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
        obj.gameObject.AddComponent<PieceIdentifier>().ID = l.Item.ID;
        obj.transform.position = new Vector3
        (
          l.Item.Position[0],
          l.Item.Position[1],
          l.Item.Position[2]
        );
        obj.transform.localScale = new Vector3
        (
          l.Item.Scale[0],
          l.Item.Scale[1],
          l.Item.Scale[2]
        );
        obj.transform.rotation = new Quaternion
        (
          l.Item.Rotation[0],
          l.Item.Rotation[1],
          l.Item.Rotation[2],
          l.Item.Rotation[3]
        );

        obj.gameObject.AddComponent<Image>();
        obj.transform.SetParent(canvas.transform);
        obj.GetComponent<Image>()
           .sprite = Resources.Load<Sprite>(l.Item.Name);
        obj.GetComponent<Image>().SetNativeSize();

        Pool.Dispatch(new Loaded(l.Item.ID, obj));
      }
    }
  }
}
