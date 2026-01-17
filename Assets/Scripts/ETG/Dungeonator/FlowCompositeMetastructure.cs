// Decompiled with JetBrains decompiler
// Type: Dungeonator.FlowCompositeMetastructure
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
  public class FlowCompositeMetastructure
  {
    public List<List<BuilderFlowNode>> loopLists = new List<List<BuilderFlowNode>>();
    public List<List<BuilderFlowNode>> compositeLists = new List<List<BuilderFlowNode>>();
    public List<BuilderFlowNode> usedList = new List<BuilderFlowNode>();

    public bool ContainedInBidirectionalLoop(BuilderFlowNode node)
    {
      for (int index1 = 0; index1 < this.loopLists.Count; ++index1)
      {
        if (this.loopLists[index1].Contains(node))
        {
          bool flag = true;
          for (int index2 = 0; index2 < this.loopLists[index1].Count; ++index2)
          {
            if (this.loopLists[index1][index2].loopConnectedBuilderNode != null && this.loopLists[index1][index2].node.loopTargetIsOneWay)
              flag = false;
          }
          return flag;
        }
      }
      return false;
    }
  }
}
