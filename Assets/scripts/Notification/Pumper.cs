using UnityEngine;

namespace Notification
{
  public class Pumper : MonoBehaviour
  {
    public int max_update_time_ms = 10;

    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    private void Update()
    {
      watch.Reset();
      watch.Start();
      while(Pool.Pump() && watch.ElapsedMilliseconds < max_update_time_ms)
      { }
      watch.Stop();
    }
  }
}
