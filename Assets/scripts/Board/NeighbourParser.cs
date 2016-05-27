using UnityEngine;
using System;
using System.Collections.Generic;

namespace Board
{
  public static class NeighbourParser
  {
    public static List<List<int?>> Parse(string data)
    {
      var ret = new List<List<int?>>();
      var lines = data.Split('|');
      foreach(var line in lines)
      {
        var ret_line = new List<int?>();
        var elements = line.Split(' ');
        foreach(var el in elements)
        { ret_line.Add(ParseInt(el)); }
        Debug.Assert(ret_line.Count == 6, "Invalid neighbour data");
        ret.Add(ret_line);
      }
      Debug.Assert(ret.Count >= 1, "Empty neighbour data");
      return ret;
    }

    private static int? ParseInt(string data)
    {
      int i = 0;
      return int.TryParse(data, out i) ? (int?)i : null;
    }
  }
}
