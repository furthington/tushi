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
        ret.Add(ret_line);
      }
      return ret;
    }

    private static int? ParseInt(string data)
    {
      int i = 0;
      return int.TryParse(data, out i) ? (int?)i : null;
    }
  }
}
