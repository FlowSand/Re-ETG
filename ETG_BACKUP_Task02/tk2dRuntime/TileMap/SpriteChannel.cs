// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.TileMap.SpriteChannel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace tk2dRuntime.TileMap;

[Serializable]
public class SpriteChannel
{
  public SpriteChunk[] chunks;

  public SpriteChannel() => this.chunks = new SpriteChunk[0];
}
