using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public class Neighbour : MonoBehaviour
  {
    public List<Neighbour> neighbours; /* Asssign in editor */

    /* start from the right and go counter-clockwise:
       0: right
       1: top-right
       2: top-left
       3: left
       4: bottom-left
       5: bottom-right
    */

    private void Start()
    { Debug.Assert(neighbours.Count == 6, name + " needs exactly 6 neighbours!"); }

    public void Rotate()
    {
      neighbours.Add(neighbours[0]);
      neighbours.RemoveAt(0);
      foreach (Neighbour n in neighbours)
      {
        if (n != null)
        { n.Rotate(); }
      }
    }

    public void PrintDebug()
    {
      string[] dir = new string[6] { "right", "top right", "top left", "left", "bottom left", "bottom right" };
      int i = 0;
      foreach (Neighbour n in neighbours)
      {
        if (n != null)
        { Logger.Log("Neighbour to my " + dir[i]); }
        ++i;
      }
      Logger.Log("====");
      foreach (Neighbour n in neighbours)
      {
        if (n != null)
        { n.PrintDebug(); }
      }
    }
  }
}
