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

