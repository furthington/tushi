using UnityEngine;
using UnityEngine.UI;
using Notification;
using System.Collections;

namespace EndGame
{
  public class Populator : MonoBehaviour
  {
    public Text score; /* Assign in editor. */
    public Text new_high_score; /* Assign in editor. */
    private int current_score = 0;
    private int final_score = 0;
    private SubscriptionStack subscriptions = new SubscriptionStack();

    public void Initialize(GameLost results)
    {
      final_score = results.Score;
      score.text = "0";
      StartCoroutine(CountScore());
      subscriptions.Add<HighScore.ReadReply>
      (
        rr =>
        {
          if (results.Score > rr.Best)
          { new_high_score.gameObject.SetActive(true); }
          subscriptions.Clear();
        }
      );
      Pool.Dispatch(new HighScore.Read());
      Pool.Dispatch(new UI.AddBackButtonHandler(gameObject, Restart));
    }

    public void Restart()
    { gameObject.AddComponent<Scene.LoadLevelProxy>().LoadLevel(0); }

    private void OnDisable()
    { Pool.Dispatch(new UI.RemoveBackButtonHandler(gameObject)); }

    private IEnumerator CountScore()
    {
      float start_time = Time.time;
      while (current_score < final_score)
      {
        float elapsed = Time.time - start_time;
        current_score = Mathf.Min((int)(elapsed / 3.0f * final_score), final_score);
        score.text = current_score.ToString();
        yield return new WaitForEndOfFrame();
      }
    }
  }
}
