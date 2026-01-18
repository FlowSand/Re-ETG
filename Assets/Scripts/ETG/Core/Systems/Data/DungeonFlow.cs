using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class DungeonFlow : ScriptableObject
    {
        [SerializeField]
        public GenericRoomTable fallbackRoomTable;
        [SerializeField]
        public GenericRoomTable phantomRoomTable;
        [SerializeField]
        public List<DungeonFlowSubtypeRestriction> subtypeRestrictions;
        [NonSerialized]
        public GenericRoomTable evolvedRoomTable;
        [SerializeField]
        private List<DungeonFlowNode> m_nodes;
        [SerializeField]
        private List<string> m_nodeGuids;
        [SerializeField]
        private string m_firstNodeGuid;
        [SerializeField]
        public List<ProceduralFlowModifierData> flowInjectionData;
        [SerializeField]
        public List<SharedInjectionData> sharedInjectionData;

        public int GetAverageNumberRooms()
        {
            int averageNumberRooms = 0;
            for (int index = 0; index < this.m_nodes.Count; ++index)
                averageNumberRooms += this.m_nodes[index].GetAverageNumberNodes();
            return averageNumberRooms;
        }

        public List<DungeonFlowNode> AllNodes => this.m_nodes;

        public DungeonFlowNode FirstNode
        {
            get => this.GetNodeFromGuid(this.m_firstNodeGuid);
            set => this.m_firstNodeGuid = value.guidAsString;
        }

        public void Initialize()
        {
            this.m_nodes = new List<DungeonFlowNode>();
            this.m_nodeGuids = new List<string>();
        }

        public void GetNodesRecursive(DungeonFlowNode node, List<DungeonFlowNode> all)
        {
            if (node == (DungeonFlowNode) null)
                return;
            all.Add(node);
            if (node.childNodeGuids == null)
                node.childNodeGuids = new List<string>();
            foreach (string childNodeGuid in node.childNodeGuids)
                this.GetNodesRecursive(this.GetNodeFromGuid(childNodeGuid), all);
        }

        public void AddNodeToFlow(DungeonFlowNode newNode, DungeonFlowNode parent)
        {
            if (this.m_nodeGuids == null || this.m_nodes == null)
                this.Initialize();
            if (this.m_nodeGuids.Contains(newNode.guidAsString))
                return;
            this.m_nodes.Add(newNode);
            this.m_nodeGuids.Add(newNode.guidAsString);
            if (parent != (DungeonFlowNode) null)
            {
                if (parent.childNodeGuids.Contains(newNode.guidAsString))
                    return;
                parent.childNodeGuids.Add(newNode.guidAsString);
                newNode.parentNodeGuid = parent.guidAsString;
            }
            else
                newNode.parentNodeGuid = string.Empty;
        }

        public void DeleteNode(DungeonFlowNode node, bool deleteChain = false)
        {
            if (deleteChain)
            {
                List<DungeonFlowNode> all = new List<DungeonFlowNode>();
                this.GetNodesRecursive(node, all);
                foreach (DungeonFlowNode node1 in all)
                    this.RemoveNodeInternal(node1);
            }
            else
                this.RemoveNodeInternal(node);
        }

        private void RemoveNodeInternal(DungeonFlowNode node)
        {
            if (!string.IsNullOrEmpty(node.parentNodeGuid))
                this.GetNodeFromGuid(node.parentNodeGuid).childNodeGuids.Remove(node.guidAsString);
            foreach (string childNodeGuid in node.childNodeGuids)
                this.GetNodeFromGuid(childNodeGuid).parentNodeGuid = string.Empty;
            if (!string.IsNullOrEmpty(node.loopTargetNodeGuid))
                this.GetNodeFromGuid(node.loopTargetNodeGuid).loopTargetNodeGuid = string.Empty;
            for (int index = 0; index < this.m_nodes.Count; ++index)
            {
                if (this.m_nodes[index].loopTargetNodeGuid == node.guidAsString)
                    this.m_nodes[index].loopTargetNodeGuid = string.Empty;
            }
            node.parentNodeGuid = string.Empty;
            node.childNodeGuids.Clear();
            node.loopTargetNodeGuid = string.Empty;
            this.m_nodes.Remove(node);
            this.m_nodeGuids.Remove(node.guidAsString);
        }

        public bool IsPartOfSubchain(DungeonFlowNode node)
        {
            DungeonFlowNode dungeonFlowNode = node;
            while (!string.IsNullOrEmpty(dungeonFlowNode.parentNodeGuid))
                dungeonFlowNode = this.GetNodeFromGuid(dungeonFlowNode.parentNodeGuid);
            return dungeonFlowNode != this.FirstNode;
        }

        private PrototypeDungeonRoom.RoomCategory GetCategoryFromChar(char c)
        {
            switch (c)
            {
                case 'b':
                    return PrototypeDungeonRoom.RoomCategory.BOSS;
                case 'c':
                    return PrototypeDungeonRoom.RoomCategory.CONNECTOR;
                case 'e':
                    return PrototypeDungeonRoom.RoomCategory.ENTRANCE;
                case 'h':
                    return PrototypeDungeonRoom.RoomCategory.HUB;
                default:
                    switch (c)
                    {
                        case 'r':
                            return PrototypeDungeonRoom.RoomCategory.REWARD;
                        case 's':
                            return PrototypeDungeonRoom.RoomCategory.SPECIAL;
                        case 't':
                            return PrototypeDungeonRoom.RoomCategory.SECRET;
                        case 'x':
                            return PrototypeDungeonRoom.RoomCategory.EXIT;
                        default:
                            return c == 'n' ? PrototypeDungeonRoom.RoomCategory.NORMAL : PrototypeDungeonRoom.RoomCategory.NORMAL;
                    }
            }
        }

        public DungeonFlowNode GetSubchainRootFromNode(DungeonFlowNode source, LoopFlowBuilder builder)
        {
            List<DungeonFlowNode> dungeonFlowNodeList = new List<DungeonFlowNode>();
            for (int index = 0; index < this.m_nodes.Count; ++index)
            {
                if (!string.IsNullOrEmpty(this.m_nodes[index].subchainIdentifier) && source.subchainIdentifiers.Contains(this.m_nodes[index].subchainIdentifier) && (!this.m_nodes[index].limitedCopiesOfSubchain || !builder.usedSubchainData.ContainsKey(this.m_nodes[index]) || builder.usedSubchainData[this.m_nodes[index]] < this.m_nodes[index].maxCopiesOfSubchain))
                    dungeonFlowNodeList.Add(this.m_nodes[index]);
            }
            return dungeonFlowNodeList.Count == 0 ? (DungeonFlowNode) null : dungeonFlowNodeList[BraveRandom.GenerationRandomRange(0, dungeonFlowNodeList.Count)];
        }

        public DungeonFlowNode GetSubchainRootFromNode(DungeonFlowNode source, DungeonFlowBuilder builder)
        {
            List<DungeonFlowNode> dungeonFlowNodeList = new List<DungeonFlowNode>();
            for (int index = 0; index < this.m_nodes.Count; ++index)
            {
                if (!string.IsNullOrEmpty(this.m_nodes[index].subchainIdentifier) && source.subchainIdentifiers.Contains(this.m_nodes[index].subchainIdentifier) && (!this.m_nodes[index].limitedCopiesOfSubchain || !builder.usedSubchainData.ContainsKey(this.m_nodes[index]) || builder.usedSubchainData[this.m_nodes[index]] < this.m_nodes[index].maxCopiesOfSubchain))
                    dungeonFlowNodeList.Add(this.m_nodes[index]);
            }
            return dungeonFlowNodeList.Count == 0 ? (DungeonFlowNode) null : dungeonFlowNodeList[BraveRandom.GenerationRandomRange(0, dungeonFlowNodeList.Count)];
        }

        public List<BuilderFlowNode> NewGetNodeChildrenToBuild(
            BuilderFlowNode parentBuilderNode,
            LoopFlowBuilder builder)
        {
            DungeonFlowNode node = parentBuilderNode.node;
            List<BuilderFlowNode> nodeChildrenToBuild = new List<BuilderFlowNode>();
            for (int index1 = 0; index1 < node.childNodeGuids.Count; ++index1)
            {
                DungeonFlowNode nodeFromGuid = this.GetNodeFromGuid(node.childNodeGuids[index1]);
                if (nodeFromGuid.nodeType == DungeonFlowNode.ControlNodeType.SELECTOR)
                {
                    List<DungeonFlowNode> selectorNodeChildren = this.GetSelectorNodeChildren(nodeFromGuid);
                    for (int index2 = 0; index2 < selectorNodeChildren.Count; ++index2)
                        nodeChildrenToBuild.Add(new BuilderFlowNode(selectorNodeChildren[index2]));
                }
                else if (nodeFromGuid.nodeType == DungeonFlowNode.ControlNodeType.SUBCHAIN)
                {
                    DungeonFlowNode subchainRootFromNode = this.GetSubchainRootFromNode(nodeFromGuid, builder);
                    if (!(subchainRootFromNode == (DungeonFlowNode) null))
                    {
                        if (builder.usedSubchainData.ContainsKey(subchainRootFromNode))
                            ++builder.usedSubchainData[subchainRootFromNode];
                        else
                            builder.usedSubchainData.Add(subchainRootFromNode, 1);
                        nodeChildrenToBuild.Add(new BuilderFlowNode(subchainRootFromNode));
                    }
                }
                else if (nodeFromGuid.nodeExpands)
                {
                    string completion = nodeFromGuid.EvolveChainToCompletion();
                    BuilderFlowNode builderFlowNode1 = (BuilderFlowNode) null;
                    for (int index3 = 0; index3 < completion.Length; ++index3)
                    {
                        PrototypeDungeonRoom.RoomCategory categoryFromChar = this.GetCategoryFromChar(completion[index3]);
                        BuilderFlowNode builderFlowNode2 = new BuilderFlowNode(nodeFromGuid);
                        builderFlowNode2.usesOverrideCategory = true;
                        builderFlowNode2.overrideCategory = categoryFromChar;
                        if (builderFlowNode1 == null)
                        {
                            builderFlowNode2.parentBuilderNode = parentBuilderNode;
                            nodeChildrenToBuild.Add(builderFlowNode2);
                        }
                        else
                        {
                            builderFlowNode2.parentBuilderNode = builderFlowNode1;
                            builderFlowNode1.childBuilderNodes = new List<BuilderFlowNode>();
                            builderFlowNode1.childBuilderNodes.Add(builderFlowNode2);
                        }
                        builderFlowNode1 = builderFlowNode2;
                    }
                }
                else if ((double) BraveRandom.GenerationRandomValue() <= (double) nodeFromGuid.percentChance)
                    nodeChildrenToBuild.Add(new BuilderFlowNode(nodeFromGuid));
            }
            for (int index = 0; index < nodeChildrenToBuild.Count; ++index)
            {
                if (nodeChildrenToBuild[index].parentBuilderNode == null)
                    nodeChildrenToBuild[index].parentBuilderNode = parentBuilderNode;
            }
            return nodeChildrenToBuild;
        }

        public List<FlowNodeBuildData> GetNodeChildrenToBuild(
            DungeonFlowNode source,
            DungeonFlowBuilder builder)
        {
            List<FlowNodeBuildData> nodeChildrenToBuild = new List<FlowNodeBuildData>();
            for (int index1 = 0; index1 < source.childNodeGuids.Count; ++index1)
            {
                DungeonFlowNode nodeFromGuid = this.GetNodeFromGuid(source.childNodeGuids[index1]);
                if (nodeFromGuid.nodeType == DungeonFlowNode.ControlNodeType.SELECTOR)
                {
                    List<DungeonFlowNode> selectorNodeChildren = this.GetSelectorNodeChildren(nodeFromGuid);
                    for (int index2 = 0; index2 < selectorNodeChildren.Count; ++index2)
                        nodeChildrenToBuild.Add(new FlowNodeBuildData(selectorNodeChildren[index2]));
                }
                else if (nodeFromGuid.nodeType == DungeonFlowNode.ControlNodeType.SUBCHAIN)
                {
                    DungeonFlowNode subchainRootFromNode = this.GetSubchainRootFromNode(nodeFromGuid, builder);
                    if (!(subchainRootFromNode == (DungeonFlowNode) null))
                    {
                        if (builder.usedSubchainData.ContainsKey(subchainRootFromNode))
                            ++builder.usedSubchainData[subchainRootFromNode];
                        else
                            builder.usedSubchainData.Add(subchainRootFromNode, 1);
                        nodeChildrenToBuild.Add(new FlowNodeBuildData(subchainRootFromNode));
                    }
                }
                else if (nodeFromGuid.nodeExpands)
                {
                    string completion = nodeFromGuid.EvolveChainToCompletion();
                    FlowNodeBuildData flowNodeBuildData1 = (FlowNodeBuildData) null;
                    for (int index3 = 0; index3 < completion.Length; ++index3)
                    {
                        PrototypeDungeonRoom.RoomCategory categoryFromChar = this.GetCategoryFromChar(completion[index3]);
                        FlowNodeBuildData flowNodeBuildData2 = new FlowNodeBuildData(nodeFromGuid);
                        flowNodeBuildData2.usesOverrideCategory = true;
                        flowNodeBuildData2.overrideCategory = categoryFromChar;
                        if (flowNodeBuildData1 == null)
                        {
                            nodeChildrenToBuild.Add(flowNodeBuildData2);
                        }
                        else
                        {
                            flowNodeBuildData1.childBuildData = new List<FlowNodeBuildData>();
                            flowNodeBuildData1.childBuildData.Add(flowNodeBuildData2);
                        }
                        flowNodeBuildData1 = flowNodeBuildData2;
                    }
                }
                else if ((double) BraveRandom.GenerationRandomValue() <= (double) nodeFromGuid.percentChance)
                    nodeChildrenToBuild.Add(new FlowNodeBuildData(nodeFromGuid));
            }
            int index4 = -1;
            for (int index5 = 0; index5 < nodeChildrenToBuild.Count; ++index5)
            {
                if (this.SubchainContainsRoomOfType(nodeChildrenToBuild[index5].node, PrototypeDungeonRoom.RoomCategory.EXIT))
                {
                    index4 = index5;
                    break;
                }
            }
            if (index4 != -1 && index4 != 0)
            {
                FlowNodeBuildData flowNodeBuildData = nodeChildrenToBuild[index4];
                nodeChildrenToBuild.RemoveAt(index4);
                nodeChildrenToBuild.Insert(0, flowNodeBuildData);
            }
            return nodeChildrenToBuild;
        }

        public List<DungeonFlowNode> GetCapChainRootNodes(DungeonFlowBuilder builder)
        {
            List<DungeonFlowNode> capChainRootNodes = new List<DungeonFlowNode>();
            for (int index = 0; index < this.m_nodes.Count; ++index)
            {
                if (this.m_nodes[index].capSubchain && (!this.m_nodes[index].limitedCopiesOfSubchain || !builder.usedSubchainData.ContainsKey(this.m_nodes[index]) || builder.usedSubchainData[this.m_nodes[index]] < this.m_nodes[index].maxCopiesOfSubchain))
                    capChainRootNodes.Add(this.m_nodes[index]);
            }
            return capChainRootNodes;
        }

        public bool SubchainContainsRoomOfType(
            DungeonFlowNode baseNode,
            PrototypeDungeonRoom.RoomCategory type)
        {
            List<DungeonFlowNode> all = new List<DungeonFlowNode>();
            this.GetNodesRecursive(baseNode, all);
            for (int index = 0; index < all.Count; ++index)
            {
                if (all[index].roomCategory == type)
                    return true;
            }
            return false;
        }

        public List<DungeonFlowNode> GetSelectorNodeChildren(DungeonFlowNode source)
        {
            BraveUtility.Assert(source.nodeType != DungeonFlowNode.ControlNodeType.SELECTOR, "Processing selector node on non-selector node.");
            int num1 = BraveRandom.GenerationRandomRange(source.minChildrenToBuild, source.maxChildrenToBuild + 1);
            List<DungeonFlowNode> selectorNodeChildren = new List<DungeonFlowNode>();
            float num2 = 0.0f;
            for (int index = 0; index < source.childNodeGuids.Count; ++index)
            {
                DungeonFlowNode nodeFromGuid = this.GetNodeFromGuid(source.childNodeGuids[index]);
                num2 += nodeFromGuid.percentChance;
            }
            List<string> stringList = new List<string>((IEnumerable<string>) source.childNodeGuids);
            for (int index1 = 0; index1 < num1; ++index1)
            {
                float num3 = BraveRandom.GenerationRandomValue() * num2;
                float num4 = 0.0f;
                for (int index2 = 0; index2 < stringList.Count; ++index2)
                {
                    DungeonFlowNode nodeFromGuid = this.GetNodeFromGuid(stringList[index2]);
                    num4 += nodeFromGuid.percentChance;
                    if ((double) num4 >= (double) num3)
                    {
                        selectorNodeChildren.Add(nodeFromGuid);
                        if (!source.canBuildDuplicateChildren)
                        {
                            num2 -= nodeFromGuid.percentChance;
                            stringList.RemoveAt(index2);
                            break;
                        }
                        break;
                    }
                }
            }
            return selectorNodeChildren;
        }

        public DungeonFlowNode GetNodeFromGuid(string guid)
        {
            int index = this.m_nodeGuids.IndexOf(guid);
            return index >= 0 ? this.m_nodes[index] : (DungeonFlowNode) null;
        }

        public void ConnectNodes(DungeonFlowNode parent, DungeonFlowNode child)
        {
            if (!string.IsNullOrEmpty(parent.parentNodeGuid))
            {
                for (string parentNodeGuid = parent.parentNodeGuid; !string.IsNullOrEmpty(parentNodeGuid); parentNodeGuid = this.GetNodeFromGuid(parentNodeGuid).parentNodeGuid)
                {
                    if (parentNodeGuid == child.guidAsString)
                        return;
                }
            }
            if (!string.IsNullOrEmpty(child.parentNodeGuid))
                this.GetNodeFromGuid(child.parentNodeGuid).childNodeGuids.Remove(child.guidAsString);
            if (parent.loopTargetNodeGuid == child.guidAsString)
                parent.loopTargetNodeGuid = string.Empty;
            if (child.loopTargetNodeGuid == parent.guidAsString)
                child.loopTargetNodeGuid = string.Empty;
            child.parentNodeGuid = parent.guidAsString;
            parent.childNodeGuids.Add(child.guidAsString);
        }

        public void LoopConnectNodes(DungeonFlowNode chainEnd, DungeonFlowNode loopTarget)
        {
            if (chainEnd.childNodeGuids.Contains(loopTarget.guidAsString) || loopTarget.childNodeGuids.Contains(chainEnd.guidAsString))
                this.DisconnectNodes(chainEnd, loopTarget);
            if (chainEnd.loopTargetNodeGuid == loopTarget.guidAsString)
            {
                if (chainEnd.loopTargetIsOneWay)
                {
                    chainEnd.loopTargetIsOneWay = false;
                    chainEnd.loopTargetNodeGuid = string.Empty;
                }
                else
                    chainEnd.loopTargetIsOneWay = true;
            }
            else
                chainEnd.loopTargetNodeGuid = loopTarget.guidAsString;
        }

        public void DisconnectNodes(DungeonFlowNode node1, DungeonFlowNode node2)
        {
            if (node1.childNodeGuids.Contains(node2.guidAsString))
            {
                node1.childNodeGuids.Remove(node2.guidAsString);
                node2.parentNodeGuid = string.Empty;
            }
            else if (node2.childNodeGuids.Contains(node1.guidAsString))
            {
                node2.childNodeGuids.Remove(node1.guidAsString);
                node1.parentNodeGuid = string.Empty;
            }
            if (node1.loopTargetNodeGuid == node2.guidAsString)
            {
                node1.loopTargetNodeGuid = string.Empty;
            }
            else
            {
                if (!(node2.loopTargetNodeGuid == node1.guidAsString))
                    return;
                node2.loopTargetNodeGuid = string.Empty;
            }
        }

        private DungeonFlowNode GetRootOfNode(DungeonFlowNode node)
        {
            DungeonFlowNode rootOfNode = node;
            while (!string.IsNullOrEmpty(rootOfNode.parentNodeGuid))
                rootOfNode = this.GetNodeFromGuid(rootOfNode.parentNodeGuid);
            return rootOfNode;
        }
    }

