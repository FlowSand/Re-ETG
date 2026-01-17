// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DesiredItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Serializable]
public class DesiredItem
{
  public GungeonFlags flagToSet;
  public DesiredItem.DetectType type;
  public int specificItemId;
  public int amount;

  public enum DetectType
  {
    SPECIFIC_ITEM,
    CURRENCY,
    META_CURRENCY,
    KEYS,
  }
}
