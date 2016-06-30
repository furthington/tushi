using UnityEngine;
using Notification;
using Error;
using System.Collections;

namespace Board
{
  public class PiecePositioner : MonoBehaviour
  {
    public Transform piece_tray; /* Assign in editor. */
    public AnimationCurve curve; /* Assign in editor. */

    private Vector3[] positions = new Vector3[4];
    private SubscriptionStack subscriptions = new SubscriptionStack();

    void Start()
    {
      for (int i = 0; i < 3; ++i)
      {
        positions[i] = transform.GetChild(i).position;
      }
      positions[3] = positions[2];
      positions[3].x += positions[2].x - positions[1].x;

      subscriptions.Add<AddNewPiece>(Pool.PostSubscribe<AddNewPiece>(_ => StartCoroutine(DelayedPosition())));
      subscriptions.Add<Save.NewGame>(Pool.PostSubscribe<Save.NewGame>(_ => ResetPositions()));
      subscriptions.Add<PieceTray.Load>(Pool.PostSubscribe<PieceTray.Load>(_ => ResetPositions()));
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private IEnumerator DelayedPosition()
    {
      yield return new WaitForEndOfFrame();
      Assert.Invariant(piece_tray.childCount == 3, "Piece Tray has " + piece_tray.childCount + " children instead of 3!");
      piece_tray.GetChild(2).position = positions[3];
      for(int i = 0; i < 3; ++i)
      {
        if ((piece_tray.GetChild(i).position - positions[i]).sqrMagnitude > Mathf.Epsilon)
        {
          Slider s = piece_tray.GetChild(i).gameObject.AddComponent<Slider>();
          s.curve = curve;
          s.end_pos = positions[i];
        }
      }
    }

    private void ResetPositions()
    {
      Assert.Invariant(piece_tray.childCount == 3, "Piece Tray has " + piece_tray.childCount + " children instead of 3!");
      for (int i = 0; i < 3; ++i)
      { piece_tray.GetChild(i).position = positions[i]; }
    }

  }
}
