using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public enum NeighbourRelationship
  {
    Right,
    TopRight,
    TopLeft,
    Left,
    BottomLeft,
    BottomRight
  }

  public class Neighbour : MonoBehaviour
  {
    public List<Neighbour> neighbours;

    private void Start()
    { Debug.Assert(neighbours.Count == 6, name + " needs exactly 6 neighbours!"); }

    public void Rotate()
    { neighbours = Rotate(neighbours); }

    public List<List<Neighbour>> GetRotations()
    {
      var ret = new List<List<Neighbour>>();

      /* Iteratively add rotations based on the last. */
      ret.Add(neighbours);
      while(ret.Count < 6)
      { ret.Add(Rotate(ret[ret.Count - 1])); }

      Debug.Assert(ret.Count == 6, "Invalid neighbour rotation count");
      return ret;
    }

    private List<Neighbour> Rotate(List<Neighbour> ns)
    {
      /* Deep copy. */
      var clone = ns.ConvertAll
      (
        x =>
        {
          if(x == null)
          { return x; }
          else
          { return Object.Instantiate(x); }
        }
      );

      clone.Add(neighbours[0]);
      clone.RemoveAt(0);
      foreach (Neighbour n in clone)
      {
        if (n != null)
        { n.Rotate(); }
      }
      return clone;
    }
  }
}
