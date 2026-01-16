// Decompiled with JetBrains decompiler
// Type: Dungeonator.SharedInjectionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator;

[Serializable]
public class SharedInjectionData : ScriptableObject
{
  [SerializeField]
  public List<ProceduralFlowModifierData> InjectionData;
  [SerializeField]
  public bool UseInvalidWeightAsNoInjection = true;
  [SerializeField]
  public bool PreventInjectionOfFailedPrerequisites;
  [SerializeField]
  public bool IsNPCCell;
  [SerializeField]
  public bool IgnoreUnmetPrerequisiteEntries;
  [SerializeField]
  public bool OnlyOne;
  [ShowInInspectorIf("OnlyOne", false)]
  public float ChanceToSpawnOne = 0.5f;
  [SerializeField]
  public List<SharedInjectionData> AttachedInjectionData;
}
