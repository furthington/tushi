using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
  public class LoadLevelProxy : MonoBehaviour
  {
    public void LoadLevel(Object scene)
    {
      PumpNotifications();
      SceneManager.LoadScene(scene.name);
    }

    public void LoadLevel(int index)
    {
      PumpNotifications();
      SceneManager.LoadScene(index);
    }

    public void LoadLevel(string name)
    {
      PumpNotifications();
      SceneManager.LoadScene(name);
    }

    private void PumpNotifications()
    { /*Notification.Pool.PumpAll();*/ }
  }
}
