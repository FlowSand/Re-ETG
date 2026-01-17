// Decompiled with JetBrains decompiler
// Type: tk2dSpriteColliderIsland
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class tk2dSpriteColliderIsland
{
  public bool connected = true;
  public Vector2[] points;

  public bool IsValid() => this.connected ? this.points.Length >= 3 : this.points.Length >= 2;

  public void CopyFrom(tk2dSpriteColliderIsland src)
  {
    this.connected = src.connected;
    this.points = new Vector2[src.points.Length];
    for (int index = 0; index < this.points.Length; ++index)
      this.points[index] = src.points[index];
  }

  public bool CompareTo(tk2dSpriteColliderIsland src)
  {
    if (this.connected != src.connected || this.points.Length != src.points.Length)
      return false;
    for (int index = 0; index < this.points.Length; ++index)
    {
      if (this.points[index] != src.points[index])
        return false;
    }
    return true;
  }
}
