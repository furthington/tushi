using UnityEngine;
using UnityEngine.UI;

namespace Board
{
  [RequireComponent(typeof(Text))]
  public class Score : MonoBehaviour
  {
    private Text text;

    private void Awake()
    {
      text = GetComponent<Text>();
      text.text = "0";
    }
  }
}
