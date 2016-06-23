using UnityEngine;
using Notification;

namespace Menu
{
  public class Popper : MonoBehaviour
  {
    public GameObject menu_prefab; /* Assign in editor. */

    public void Show()
    { Instantiate(menu_prefab); }
  }
}
