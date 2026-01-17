// Decompiled with JetBrains decompiler
// Type: DungeonFlowLevelEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class DungeonFlowLevelEntry
{
  public string flowPath;
  public float weight = 1f;
  public FlowLevelEntryMode levelMode;
  public DungeonPrerequisite[] prerequisites;
  public bool forceUseIfAvailable;
  public bool overridesLevelDetailText;
  public string overrideLevelDetailText = string.Empty;
}
