// Decompiled with JetBrains decompiler
// Type: IntToStringSansGarbage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public static class IntToStringSansGarbage
{
  private static Dictionary<int, string> m_map = new Dictionary<int, string>();

  public static string GetStringForInt(int input)
  {
    if (IntToStringSansGarbage.m_map.ContainsKey(input))
      return IntToStringSansGarbage.m_map[input];
    string stringForInt = input.ToString();
    IntToStringSansGarbage.m_map.Add(input, stringForInt);
    if (IntToStringSansGarbage.m_map.Count > 25000)
    {
      Debug.LogError((object) "Int To String (sans Garbage) map count greater than 25000!");
      IntToStringSansGarbage.m_map.Clear();
    }
    return stringForInt;
  }
}
