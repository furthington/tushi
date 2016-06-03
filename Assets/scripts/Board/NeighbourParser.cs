using UnityEngine;
using System;
using System.Collections.Generic;
using Error;

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

  public static class NeighbourParser
  {
    public static List<List<List<int?>>> GetRotations(string data)
    {
      using(var timer = new Profile.TaskTimer("Build neighbour rotations"))
      {
        var ret = new List<List<List<int?>>>();
        var first = Parse(data);
        ret.Add(first);
        while(ret.Count < 6)
        { ret.Add(Rotate(ret[ret.Count - 1])); }
        return ret;
      }
    }

    private static List<List<int?>> Parse(string data)
    {
      var ret = new List<List<int?>>();
      var lines = data.Split('|');
      foreach(var line in lines)
      {
        var ret_line = new List<int?>();
        var elements = line.Split(' ');
        foreach(var el in elements)
        { ret_line.Add(ParseInt(el)); }
        Assert.Invariant(ret_line.Count == 6, "Invalid neighbour data");
        ret.Add(ret_line);
      }
      Assert.Invariant(ret.Count >= 1, "Empty neighbour data");
      return ret;
    }

    private static int? ParseInt(string data)
    {
      int i = 0;
      return int.TryParse(data, out i) ? (int?)i : null;
    }

    private static List<List<int?>> Rotate(List<List<int?>> l)
    {
      var clone = l.ConvertAll(line => new List<int?>(line));
      foreach(var line in clone)
      {
        line.Add(line[0]);
        line.RemoveAt(0);
      }
      return clone;
    }
  }
}
