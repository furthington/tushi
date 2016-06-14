using UnityEngine;
using Notification;

namespace Menu
{
  public class Popper : MonoBehaviour
  {
    public GameObject menu_prefab; /* Assign in editor. */

    private void Start()
    { Pool.Dispatch(new UI.AddBackButtonHandler(gameObject, Show)); }

    public void Show()
    { Instantiate(menu_prefab); }
  }
}
