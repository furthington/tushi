using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Notification;

namespace Menu
{
  public class Populator : MonoBehaviour
  {
    public Text score; /* Assign in editor. */
    public List<Animator> animators; /* Assign in editor. */

    private void Start()
    {
      score.text = GameObject.FindGameObjectWithTag("score")
                     .GetComponent<Board.Score>().GetRealScore().ToString();
      Pool.Dispatch(new UI.AddBackButtonHandler(gameObject, Close));
    }

    private void OnDisable() /* In case we don't close, but restart. */
    { Pool.Dispatch(new UI.RemoveBackButtonHandler(gameObject)); }

    public void OnRestart()
    {
      Pool.Dispatch(new EndGame.GameRestart());
      Pool.Dispatch(new Board.WriteScore());
      /*All notifications above will be pumped. */
      Scene.LoadLevelProxy ll = gameObject.AddComponent<Scene.LoadLevelProxy>();
      ll.LoadLevel(0);
    }

    public void Close()
    {
      Pool.Dispatch(new UI.RemoveBackButtonHandler(gameObject));
      foreach (Animator a in animators)
      { a.SetTrigger("exit"); }
      StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
      yield return new WaitForSeconds(0.5f);
      Destroy(gameObject);
    }
  }
}
