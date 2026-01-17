// Decompiled with JetBrains decompiler
// Type: SimpleTilesetAnimationSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
[Serializable]
public class SimpleTilesetAnimationSequence
{
  public SimpleTilesetAnimationSequence.TilesetSequencePlayStyle playstyle;
  public float loopDelayMin = 5f;
  public float loopDelayMax = 10f;
  public int loopceptionTarget = -1;
  public int loopceptionMin = 1;
  public int loopceptionMax = 3;
  public int coreceptionMin = 1;
  public int coreceptionMax = 1;
  public bool randomStartFrame;
  public List<SimpleTilesetAnimationSequenceEntry> entries = new List<SimpleTilesetAnimationSequenceEntry>();

  public enum TilesetSequencePlayStyle
  {
    SIMPLE_LOOP,
    DELAYED_LOOP,
    RANDOM_FRAMES,
    TRIGGERED_ONCE,
    LOOPCEPTION,
  }
}
