// Decompiled with JetBrains decompiler
// Type: Dungeonator.RuntimeInjectionMetadata
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Dungeonator;

public class RuntimeInjectionMetadata
{
  public SharedInjectionData injectionData;
  public bool forceSecret;
  [NonSerialized]
  public bool HasAssignedModDataExactRoom;
  [NonSerialized]
  public ProceduralFlowModifierData AssignedModifierData;
  public Dictionary<ProceduralFlowModifierData, bool> SucceededRandomizationCheckMap = new Dictionary<ProceduralFlowModifierData, bool>();

  public RuntimeInjectionMetadata(SharedInjectionData data) => this.injectionData = data;

  public void CopyMetadata(RuntimeInjectionMetadata other) => this.forceSecret = other.forceSecret;
}
