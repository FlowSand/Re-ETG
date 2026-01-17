// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MassiveSaveFlagEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Serializable]
public class MassiveSaveFlagEntry
{
  public GungeonFlags RequiredFlag;
  public bool RequiredFlagState = true;
  public GungeonFlags CompletedFlag;
  public string mode;
}
