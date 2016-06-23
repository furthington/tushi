using UnityEngine;
using Notification;

namespace Menu
{
  [RequireComponent(typeof(Popper))]
  public class Listener : MonoBehaviour
  {
    private void Start()
    { Pool.Dispatch(new UI.AddBackButtonHandler(gameObject, Show)); }

    private void OnDisable()
    { Pool.Dispatch(new UI.RemoveBackButtonHandler(gameObject)); }

    private void Show()
    { GetComponent<Popper>().Show(); }
  }
}
