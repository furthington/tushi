using UnityEngine;

namespace Profile
{
  /* Times from Awake to first Update. */
  public class StartupTimer : MonoBehaviour
  {
    private TaskTimer timer;

    private void Awake()
    { timer = new TaskTimer("Startup"); }

    private void Update()
    {
      if(timer != null)
      {
        timer.Dispose();
        Destroy(gameObject);
      }
    }
  }
}
