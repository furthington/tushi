using UnityEngine;
using System.Collections.Generic;

namespace Board
{
  public class NeighbourRequest
  {
    public GameObject Requestor
    { get; set; }
    NeighbourRelationship Relationship
    { get; set; }

    public NeighbourRequest(GameObject r, NeighbourRelationship nr)
    {
      Requestor = r;
      Relationship = nr;
    }
  }
  public class NeighbourReply
  {
    public Tile Neighbour;
    public NeighbourRequest Request
    { get; set; }

    public NeighbourReply(Tile t, NeighbourRequest nr)
    {
      Neighbour = t;
      Request = nr;
    }
  }

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
    public List<Neighbour> neighbours; /* Asssign in editor */

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
  }
}
