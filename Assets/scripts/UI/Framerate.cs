using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
  public class Framerate : MonoBehaviour
  {
    public int target_frame_rate;
    private Text txt = null;

    void Start()
    {
      Application.targetFrameRate = target_frame_rate;
      txt = GetComponent<Text>();
    }

    void Update()
    {
      if (txt == null)
      { return; }
      string str = "Target: " + Application.targetFrameRate;
      str += "\n" + "FPS: " + (1.0f / Time.deltaTime).ToString();
      txt.text = str;
    }
  }
}
