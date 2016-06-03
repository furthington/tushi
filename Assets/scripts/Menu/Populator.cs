using UnityEngine;
using UnityEngine.UI;
using Notification;

namespace Menu
{
  public class Populator : MonoBehaviour
  {
    public Text score; /* Assign in editor. */

    private void Start()
    {
      score.text = GameObject.FindGameObjectWithTag("score")
                     .GetComponent<Text>().text;
    }
    public void Close()
    { Destroy(gameObject); }
  }
}
