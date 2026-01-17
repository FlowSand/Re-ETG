// Decompiled with JetBrains decompiler
// Type: Dungeonator.BuilderFlowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class BuilderFlowNode : ArbitraryFlowBuildData
  {
    public int identifier = -1;
    public DungeonFlowNode node;
    public bool AcquiresRoomAsNecessary;
    public PrototypeDungeonRoom assignedPrototypeRoom;
    public RoomHandler instanceRoom;
    public bool usesOverrideCategory;
    public PrototypeDungeonRoom.RoomCategory overrideCategory;
    public BuilderFlowNode parentBuilderNode;
    public List<BuilderFlowNode> childBuilderNodes;
    public BuilderFlowNode loopConnectedBuilderNode;
    public Dictionary<PrototypeRoomExit, BuilderFlowNode> exitToNodeMap = new Dictionary<PrototypeRoomExit, BuilderFlowNode>();
    public Dictionary<BuilderFlowNode, PrototypeRoomExit> nodeToExitMap = new Dictionary<BuilderFlowNode, PrototypeRoomExit>();
    public BuilderFlowNode InjectionTarget;
    public List<BuilderFlowNode> InjectionFrameSequence;
    public bool IsInjectedNode;

    public BuilderFlowNode(DungeonFlowNode n) => this.node = n;

    public int Connectivity
    {
      get
      {
        int num = 0;
        if (this.parentBuilderNode != null)
          ++num;
        if (this.loopConnectedBuilderNode != null)
          ++num;
        return num + this.childBuilderNodes.Count;
      }
    }

    public void ClearData()
    {
      this.exitToNodeMap.Clear();
      this.nodeToExitMap.Clear();
      this.IsInjectedNode = false;
    }

    public PrototypeDungeonRoom.RoomCategory Category
    {
      get
      {
        if (this.usesOverrideCategory)
          return this.overrideCategory;
        return (Object) this.assignedPrototypeRoom != (Object) null ? this.assignedPrototypeRoom.category : this.node.roomCategory;
      }
    }

    public bool IsOfDepth(int depth)
    {
      BuilderFlowNode builderFlowNode = this;
      for (int index = 0; index < depth; ++index)
      {
        if (builderFlowNode.parentBuilderNode == null)
          return false;
        builderFlowNode = builderFlowNode.parentBuilderNode;
      }
      return true;
    }

    public bool IsStandardCategory
    {
      get
      {
        return (this.Category == PrototypeDungeonRoom.RoomCategory.NORMAL || this.Category == PrototypeDungeonRoom.RoomCategory.CONNECTOR || this.Category == PrototypeDungeonRoom.RoomCategory.HUB) && (this.Category != PrototypeDungeonRoom.RoomCategory.NORMAL || !((Object) this.assignedPrototypeRoom != (Object) null) || this.assignedPrototypeRoom.subCategoryNormal != PrototypeDungeonRoom.RoomNormalSubCategory.TRAP);
      }
    }

    public List<BuilderFlowNode> GetAllConnectedNodes(List<BuilderFlowNode> excluded)
    {
      List<BuilderFlowNode> allConnectedNodes = new List<BuilderFlowNode>((IEnumerable<BuilderFlowNode>) this.childBuilderNodes);
      if (this.parentBuilderNode != null)
        allConnectedNodes.Add(this.parentBuilderNode);
      if (this.loopConnectedBuilderNode != null)
        allConnectedNodes.Add(this.loopConnectedBuilderNode);
      for (int index = 0; index < excluded.Count; ++index)
        allConnectedNodes.Remove(excluded[index]);
      return allConnectedNodes;
    }

    public List<BuilderFlowNode> GetAllConnectedNodes(params BuilderFlowNode[] excluded)
    {
      List<BuilderFlowNode> allConnectedNodes = new List<BuilderFlowNode>((IEnumerable<BuilderFlowNode>) this.childBuilderNodes);
      if (this.parentBuilderNode != null)
        allConnectedNodes.Add(this.parentBuilderNode);
      if (this.loopConnectedBuilderNode != null)
        allConnectedNodes.Add(this.loopConnectedBuilderNode);
      for (int index = 0; index < excluded.Length; ++index)
        allConnectedNodes.Remove(excluded[index]);
      return allConnectedNodes;
    }

    public void MakeNodeTreeRoot()
    {
      if (this.parentBuilderNode == null)
        return;
      BuilderFlowNode builderFlowNode1 = (BuilderFlowNode) null;
      BuilderFlowNode builderFlowNode2 = this;
      BuilderFlowNode parentBuilderNode;
      for (BuilderFlowNode builderFlowNode3 = this.parentBuilderNode; builderFlowNode3 != null; builderFlowNode3 = parentBuilderNode)
      {
        parentBuilderNode = builderFlowNode3.parentBuilderNode;
        builderFlowNode2.parentBuilderNode = builderFlowNode1;
        builderFlowNode2.childBuilderNodes.Add(builderFlowNode3);
        builderFlowNode3.parentBuilderNode = builderFlowNode2;
        builderFlowNode3.childBuilderNodes.Remove(builderFlowNode2);
        builderFlowNode1 = builderFlowNode2;
        builderFlowNode2 = builderFlowNode3;
      }
    }
  }
}
