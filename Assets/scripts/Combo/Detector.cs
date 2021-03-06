﻿using UnityEngine;
using Notification;

namespace Combo
{
  public class Triggered
  {
    public int Lines
    { get; set; }

    public Triggered(int l)
    { Lines = l; }
  }

  public class Detector : MonoBehaviour
  {
    SubscriptionStack subscriptions = new SubscriptionStack();
    private int lines_cleared = 0;
    private void Start()
    {
      subscriptions.Add<Board.LineCompleted>(_ => ++lines_cleared);
      subscriptions.Add<Board.PiecePlaced>(_ => Tally());
    }

    private void OnDisable()
    { subscriptions.Clear(); }

    private void Tally()
    {
      if (lines_cleared > 1)
      {
        Logger.Log("Combo!!!");
        var s = new Board.AddScore();
        s.Score = lines_cleared * lines_cleared * 25;
        Pool.Dispatch(s);
        Pool.Dispatch(new Triggered(lines_cleared));
      }
      lines_cleared = 0;
    }
  }
}
