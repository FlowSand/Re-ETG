// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.TileMap.ColorChunk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap;

[Serializable]
public class ColorChunk
{
  public Color32[] colors;
  public Color32[,] colorOverrides;

  public ColorChunk()
  {
    this.colors = new Color32[0];
    this.colorOverrides = new Color32[0, 0];
  }

  public bool Dirty { get; set; }

  public bool Empty => this.colors.Length == 0;
}
