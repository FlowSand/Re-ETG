// Decompiled with JetBrains decompiler
// Type: DungeonChainStructure
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;

#nullable disable

public class DungeonChainStructure
  {
    public FlowNodeBuildData parentNode;
    public FlowNodeBuildData optionalRequiredNode;
    public List<FlowNodeBuildData> containedNodes = new List<FlowNodeBuildData>();
    public int previousLoopDistanceMetric = int.MaxValue;
  }

