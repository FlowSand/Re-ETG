// Decompiled with JetBrains decompiler
// Type: Dungeonator.LoopFlowBuilder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Dungeonator;

public class LoopFlowBuilder
{
  public Dictionary<DungeonFlowNode, int> usedSubchainData = new Dictionary<DungeonFlowNode, int>();
  public bool DEBUG_RENDER_CANVASES_SEPARATELY;
  protected DungeonFlow m_flow;
  protected LoopDungeonGenerator m_generator;
  protected List<BuilderFlowNode> allBuilderNodes = new List<BuilderFlowNode>();
  protected Dictionary<PrototypeDungeonRoom, int> m_usedPrototypeRoomData = new Dictionary<PrototypeDungeonRoom, int>();
  protected List<PrototypeDungeonRoom> m_excludedRoomData = new List<PrototypeDungeonRoom>();
  protected int m_currentMaxLengthProceduralHallway;
  protected Dictionary<DungeonFlowSubtypeRestriction, int> roomsOfSubtypeRemaining;
  public static ObjectPool<List<BuilderFlowNode>> BuilderFlowNodeListPool = new ObjectPool<List<BuilderFlowNode>>((ObjectPool<List<BuilderFlowNode>>.Factory) (() => new List<BuilderFlowNode>()), 10);
  protected bool AttachWarpCanvasSuccess;
  protected bool AttachNewCanvasSuccess;
  protected const int MAX_LOOP_REGENERATION_ATTEMPTS = 100;
  protected const int MAX_NONLOOP_REGENERATION_ATTEMPTS = 5;
  public SemioticLayoutManager DeferredGeneratedLayout;
  public bool DeferredGenerationSuccess;
  private List<RuntimeInjectionMetadata> m_postprocessInjectionData = new List<RuntimeInjectionMetadata>();
  private RuntimeInjectionFlags m_runtimeInjectionFlags = new RuntimeInjectionFlags();
  private Dictionary<SharedInjectionData, RuntimeInjectionMetadata> m_previouslyGeneratedRuntimeMetadata = new Dictionary<SharedInjectionData, RuntimeInjectionMetadata>();

  public LoopFlowBuilder(DungeonFlow flow, LoopDungeonGenerator generator)
  {
    this.m_flow = flow;
    this.m_generator = generator;
  }

  public BuilderFlowNode ConstructNodeForInjection(
    PrototypeDungeonRoom exactRoom,
    ProceduralFlowModifierData modData,
    RuntimeInjectionMetadata optionalMetadata)
  {
    DungeonFlowNode n = new DungeonFlowNode(this.m_flow);
    n.overrideExactRoom = exactRoom;
    n.priority = DungeonFlowNode.NodePriority.MANDATORY;
    if ((double) BraveRandom.GenerationRandomValue() < (double) modData.chanceToLock)
      n.forcedDoorType = DungeonFlowNode.ForcedDoorType.LOCKED;
    BuilderFlowNode builderFlowNode = new BuilderFlowNode(n);
    builderFlowNode.assignedPrototypeRoom = exactRoom;
    builderFlowNode.childBuilderNodes = new List<BuilderFlowNode>();
    builderFlowNode.IsInjectedNode = true;
    if (optionalMetadata != null && optionalMetadata.forceSecret)
    {
      n.roomCategory = PrototypeDungeonRoom.RoomCategory.SECRET;
      builderFlowNode.usesOverrideCategory = true;
      builderFlowNode.overrideCategory = PrototypeDungeonRoom.RoomCategory.SECRET;
    }
    return builderFlowNode;
  }

  protected void InjectValidator_RandomCombatRoom(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (current.parentBuilderNode == null || !current.IsOfDepth(modData.RandomNodeChildMinDistanceFromEntrance) || metastructure.ContainedInBidirectionalLoop(current) || current.node.isWarpWingEntrance || !current.IsStandardCategory || !((UnityEngine.Object) current.assignedPrototypeRoom != (UnityEngine.Object) null) || !current.assignedPrototypeRoom.ContainsEnemies)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_EndOfChain(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (current.parentBuilderNode == null || current.node.isWarpWingEntrance || current.node.roomCategory == PrototypeDungeonRoom.RoomCategory.EXIT || current.childBuilderNodes.Count != 0 || current.Category == PrototypeDungeonRoom.RoomCategory.SECRET || current.parentBuilderNode != null && current.parentBuilderNode.node.isWarpWingEntrance || current.loopConnectedBuilderNode != null && !current.node.loopTargetIsOneWay)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_HubAdjacentChainStart(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (current.parentBuilderNode == null || current.node.isWarpWingEntrance || current.parentBuilderNode.Category != PrototypeDungeonRoom.RoomCategory.HUB)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_HubAdjacentNoLink(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (current.Category != PrototypeDungeonRoom.RoomCategory.HUB)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_RandomNodeChild(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (!current.IsStandardCategory || current.node.isWarpWingEntrance || current.node.roomCategory == PrototypeDungeonRoom.RoomCategory.EXIT || !current.IsOfDepth(modData.RandomNodeChildMinDistanceFromEntrance) || current.parentBuilderNode != null && current.parentBuilderNode.node.isWarpWingEntrance)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_AfterBoss(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (current.parentBuilderNode == null || current.node.isWarpWingEntrance || current.parentBuilderNode.Category != PrototypeDungeonRoom.RoomCategory.BOSS)
      return;
    validNodes.Add(current);
  }

  protected void InjectValidator_BlackMarket(
    BuilderFlowNode current,
    List<BuilderFlowNode> validNodes,
    ProceduralFlowModifierData modData,
    FlowCompositeMetastructure metastructure)
  {
    if (!((UnityEngine.Object) current.assignedPrototypeRoom != (UnityEngine.Object) null) || !current.assignedPrototypeRoom.name.Contains("Black Market"))
      return;
    validNodes.Add(current);
  }

  protected void InjectNodeNoLinks(
    ProceduralFlowModifierData modData,
    PrototypeDungeonRoom exactRoom,
    BuilderFlowNode root,
    FlowCompositeMetastructure metastructure,
    RuntimeInjectionMetadata optionalMetadata)
  {
    BuilderFlowNode builderFlowNode = this.ConstructNodeForInjection(exactRoom, modData, optionalMetadata);
    builderFlowNode.node.isWarpWingEntrance = true;
    builderFlowNode.node.handlesOwnWarping = true;
    root.childBuilderNodes.Add(builderFlowNode);
    builderFlowNode.parentBuilderNode = root;
    builderFlowNode.InjectionTarget = root;
    this.allBuilderNodes.Add(builderFlowNode);
  }

  protected bool InjectNodeBefore(
    ProceduralFlowModifierData modData,
    PrototypeDungeonRoom exactRoom,
    BuilderFlowNode root,
    Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure> validator,
    FlowCompositeMetastructure metastructure,
    RuntimeInjectionMetadata optionalMetadata)
  {
    optionalMetadata.forceSecret = false;
    BuilderFlowNode builderFlowNode1 = this.ConstructNodeForInjection(exactRoom, modData, optionalMetadata);
    List<BuilderFlowNode> builderFlowNodeList = new List<BuilderFlowNode>();
    Stack<BuilderFlowNode> builderFlowNodeStack = new Stack<BuilderFlowNode>();
    builderFlowNodeStack.Push(root);
    while (builderFlowNodeStack.Count > 0)
    {
      BuilderFlowNode builderFlowNode2 = builderFlowNodeStack.Pop();
      validator(builderFlowNode2, builderFlowNodeList, modData, metastructure);
      for (int index = 0; index < builderFlowNode2.childBuilderNodes.Count; ++index)
        builderFlowNodeStack.Push(builderFlowNode2.childBuilderNodes[index]);
    }
    if (builderFlowNodeList.Count <= 0)
      return false;
    BuilderFlowNode builderFlowNode3 = builderFlowNodeList[BraveRandom.GenerationRandomRange(0, builderFlowNodeList.Count)];
    BuilderFlowNode parentBuilderNode = builderFlowNode3.parentBuilderNode;
    parentBuilderNode.childBuilderNodes.Remove(builderFlowNode3);
    parentBuilderNode.childBuilderNodes.Add(builderFlowNode1);
    builderFlowNode1.parentBuilderNode = parentBuilderNode;
    builderFlowNode3.parentBuilderNode = builderFlowNode1;
    builderFlowNode1.childBuilderNodes.Add(builderFlowNode3);
    builderFlowNode1.InjectionTarget = builderFlowNode3;
    this.allBuilderNodes.Add(builderFlowNode1);
    return true;
  }

  protected bool InjectNodeAfter(
    ProceduralFlowModifierData modData,
    PrototypeDungeonRoom exactRoom,
    BuilderFlowNode root,
    Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure> validator,
    FlowCompositeMetastructure metastructure,
    RuntimeInjectionMetadata optionalMetadata)
  {
    BuilderFlowNode builderFlowNode1 = this.ConstructNodeForInjection(exactRoom, modData, optionalMetadata);
    builderFlowNode1.node.isWarpWingEntrance = modData.IsWarpWing;
    List<BuilderFlowNode> builderFlowNodeList = new List<BuilderFlowNode>();
    Stack<BuilderFlowNode> builderFlowNodeStack = new Stack<BuilderFlowNode>();
    builderFlowNodeStack.Push(root);
    while (builderFlowNodeStack.Count > 0)
    {
      BuilderFlowNode builderFlowNode2 = builderFlowNodeStack.Pop();
      validator(builderFlowNode2, builderFlowNodeList, modData, metastructure);
      for (int index = 0; index < builderFlowNode2.childBuilderNodes.Count; ++index)
        builderFlowNodeStack.Push(builderFlowNode2.childBuilderNodes[index]);
    }
    if (builderFlowNodeList.Count <= 0)
      return false;
    BuilderFlowNode builderFlowNode3 = builderFlowNodeList[BraveRandom.GenerationRandomRange(0, builderFlowNodeList.Count)];
    builderFlowNode3.childBuilderNodes.Add(builderFlowNode1);
    builderFlowNode1.parentBuilderNode = builderFlowNode3;
    builderFlowNode1.childBuilderNodes = new List<BuilderFlowNode>();
    builderFlowNode1.InjectionTarget = builderFlowNode3;
    this.allBuilderNodes.Add(builderFlowNode1);
    return true;
  }

  protected void RecurseCombatRooms(
    BuilderFlowNode currentCheckNode,
    List<BuilderFlowNode> currentSequence,
    int desiredDepth,
    List<List<BuilderFlowNode>> validSequences)
  {
    bool flag = currentSequence.Count == desiredDepth - 1 || currentCheckNode.childBuilderNodes.Count > 0;
    if (currentSequence.Count == desiredDepth - 1 && currentCheckNode.loopConnectedBuilderNode != null || !currentCheckNode.IsStandardCategory || !((UnityEngine.Object) currentCheckNode.assignedPrototypeRoom != (UnityEngine.Object) null) || !currentCheckNode.assignedPrototypeRoom.ContainsEnemies || !flag)
      return;
    List<BuilderFlowNode> currentSequence1 = new List<BuilderFlowNode>((IEnumerable<BuilderFlowNode>) currentSequence);
    currentSequence1.Add(currentCheckNode);
    if (currentSequence1.Count == desiredDepth)
    {
      validSequences.Add(currentSequence1);
    }
    else
    {
      for (int index = 0; index < currentCheckNode.childBuilderNodes.Count; ++index)
        this.RecurseCombatRooms(currentCheckNode.childBuilderNodes[index], currentSequence1, desiredDepth, validSequences);
    }
  }

  protected void HandleInjectionFrame(
    ProceduralFlowModifierData modData,
    BuilderFlowNode root,
    RuntimeInjectionMetadata optionalMetadata,
    FlowCompositeMetastructure metastructure)
  {
    int framedCombatNodes = modData.framedCombatNodes;
    optionalMetadata.forceSecret = false;
    BuilderFlowNode builderFlowNode1 = this.ConstructNodeForInjection(modData.exactRoom, modData, optionalMetadata);
    BuilderFlowNode builderFlowNode2 = this.ConstructNodeForInjection(modData.exactSecondaryRoom, modData, optionalMetadata);
    List<List<BuilderFlowNode>> validSequences = new List<List<BuilderFlowNode>>();
    Stack<BuilderFlowNode> builderFlowNodeStack = new Stack<BuilderFlowNode>();
    builderFlowNodeStack.Push(root);
    List<BuilderFlowNode> currentSequence = new List<BuilderFlowNode>();
    while (builderFlowNodeStack.Count > 0)
    {
      BuilderFlowNode currentCheckNode = builderFlowNodeStack.Pop();
      this.RecurseCombatRooms(currentCheckNode, currentSequence, framedCombatNodes, validSequences);
      for (int index = 0; index < currentCheckNode.childBuilderNodes.Count; ++index)
        builderFlowNodeStack.Push(currentCheckNode.childBuilderNodes[index]);
    }
    if (validSequences.Count <= 0)
      return;
    List<BuilderFlowNode> collection = validSequences[BraveRandom.GenerationRandomRange(0, validSequences.Count)];
    List<BuilderFlowNode> builderFlowNodeList = new List<BuilderFlowNode>();
    builderFlowNodeList.Add(builderFlowNode1);
    builderFlowNodeList.AddRange((IEnumerable<BuilderFlowNode>) collection);
    builderFlowNodeList.Add(builderFlowNode2);
    BuilderFlowNode builderFlowNode3 = collection[0];
    BuilderFlowNode parentBuilderNode = builderFlowNode3.parentBuilderNode;
    parentBuilderNode.childBuilderNodes.Remove(builderFlowNode3);
    parentBuilderNode.childBuilderNodes.Add(builderFlowNode1);
    builderFlowNode1.parentBuilderNode = parentBuilderNode;
    builderFlowNode3.parentBuilderNode = builderFlowNode1;
    builderFlowNode1.childBuilderNodes.Add(builderFlowNode3);
    builderFlowNode1.InjectionFrameSequence = builderFlowNodeList;
    this.allBuilderNodes.Add(builderFlowNode1);
    BuilderFlowNode builderFlowNode4 = collection[collection.Count - 1];
    builderFlowNode4.childBuilderNodes.Add(builderFlowNode2);
    builderFlowNode2.parentBuilderNode = builderFlowNode4;
    builderFlowNode2.childBuilderNodes = new List<BuilderFlowNode>();
    builderFlowNode2.InjectionFrameSequence = builderFlowNodeList;
    this.allBuilderNodes.Add(builderFlowNode2);
  }

  protected bool ProcessSingleNodeInjection(
    ProceduralFlowModifierData currentInjectionData,
    BuilderFlowNode root,
    RuntimeInjectionFlags injectionFlags,
    FlowCompositeMetastructure metastructure,
    RuntimeInjectionMetadata optionalMetadata = null)
  {
    bool flag1 = false;
    if ((UnityEngine.Object) currentInjectionData.RequiredValidPlaceable != (UnityEngine.Object) null && !currentInjectionData.RequiredValidPlaceable.HasValidPlaceable())
    {
      if (flag1)
        UnityEngine.Debug.LogError((object) $"Failing Injection because {currentInjectionData.RequiredValidPlaceable.name} has no valid placeable.");
      return false;
    }
    bool flag2 = false;
    if (!flag2 && !currentInjectionData.PrerequisitesMet)
    {
      if (flag1)
        UnityEngine.Debug.Log((object) $"Failing Injection because {currentInjectionData.annotation} has unmet prerequisites.");
      return false;
    }
    if (!flag2 && (UnityEngine.Object) currentInjectionData.exactRoom != (UnityEngine.Object) null && !currentInjectionData.exactRoom.CheckPrerequisites())
    {
      if (flag1)
        UnityEngine.Debug.Log((object) $"Failing Injection because {currentInjectionData.exactRoom.name} has unmet prerequisites.");
      return false;
    }
    PrototypeDungeonRoom exactRoom = currentInjectionData.exactRoom;
    if ((UnityEngine.Object) currentInjectionData.roomTable != (UnityEngine.Object) null && (UnityEngine.Object) currentInjectionData.exactRoom == (UnityEngine.Object) null)
    {
      WeightedRoom weightedRoom = currentInjectionData.roomTable.SelectByWeight();
      if (weightedRoom != null)
        exactRoom = weightedRoom.room;
    }
    if ((UnityEngine.Object) exactRoom == (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) currentInjectionData.roomTable != (UnityEngine.Object) null)
      {
        if (flag1)
          UnityEngine.Debug.Log((object) $"Failing Injection because {currentInjectionData.roomTable.name} has no valid rooms in its table.");
        return false;
      }
      if (flag1)
        UnityEngine.Debug.Log((object) $"Failing Injection because {currentInjectionData.annotation} is a NULL room injection!");
      return true;
    }
    if (optionalMetadata != null && optionalMetadata.SucceededRandomizationCheckMap.ContainsKey(currentInjectionData))
    {
      if (!optionalMetadata.SucceededRandomizationCheckMap[currentInjectionData])
      {
        if (flag1)
          UnityEngine.Debug.Log((object) $"Failing Injection on {currentInjectionData.annotation} by CACHED RNG.");
        return false;
      }
    }
    else
    {
      if (!flag2 && (double) BraveRandom.GenerationRandomValue() > (double) currentInjectionData.chanceToSpawn)
      {
        if (flag1)
          UnityEngine.Debug.Log((object) $"Failing Injection on {currentInjectionData.annotation} by RNG.");
        optionalMetadata?.SucceededRandomizationCheckMap.Add(currentInjectionData, false);
        return false;
      }
      optionalMetadata?.SucceededRandomizationCheckMap.Add(currentInjectionData, true);
    }
    if (!flag2 && !exactRoom.injectionFlags.IsValid(injectionFlags))
    {
      if (flag1)
        UnityEngine.Debug.Log((object) $"Failing Injection because {exactRoom.name} has invalid injection flags state.");
      return false;
    }
    if (injectionFlags.Merge(exactRoom.injectionFlags))
      UnityEngine.Debug.Log((object) ("Assigning FIREPLACE from room: " + exactRoom.name));
    ProceduralFlowModifierData.FlowModifierPlacementType modifierPlacementType = currentInjectionData.GetPlacementRule();
    if (optionalMetadata != null && optionalMetadata.forceSecret && !currentInjectionData.DEBUG_FORCE_SPAWN)
    {
      if (!currentInjectionData.CanBeForcedSecret)
      {
        if (flag1)
          UnityEngine.Debug.Log((object) $"Failing Injection because {currentInjectionData.annotation} cannot be forced SECRET.");
        return false;
      }
      modifierPlacementType = ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD;
    }
    if (flag1 && (UnityEngine.Object) exactRoom != (UnityEngine.Object) null)
      UnityEngine.Debug.Log((object) ("Succeeding injection of room : " + exactRoom.name));
    bool flag3 = true;
    switch (modifierPlacementType)
    {
      case ProceduralFlowModifierData.FlowModifierPlacementType.BEFORE_ANY_COMBAT_ROOM:
        flag3 = this.InjectNodeBefore(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_RandomCombatRoom), metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN:
        flag3 = this.InjectNodeAfter(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_EndOfChain), metastructure, optionalMetadata);
        if (!flag3)
        {
          flag3 = this.InjectNodeAfter(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_RandomNodeChild), metastructure, optionalMetadata);
          break;
        }
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.HUB_ADJACENT_CHAIN_START:
        flag3 = this.InjectNodeBefore(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_HubAdjacentChainStart), metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.HUB_ADJACENT_NO_LINK:
        flag3 = this.InjectNodeAfter(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_HubAdjacentNoLink), metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD:
        flag3 = this.InjectNodeAfter(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_RandomNodeChild), metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.COMBAT_FRAME:
        this.HandleInjectionFrame(currentInjectionData, root, optionalMetadata, metastructure);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.NO_LINKS:
        this.InjectNodeNoLinks(currentInjectionData, exactRoom, root, metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.AFTER_BOSS:
        flag3 = this.InjectNodeBefore(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_AfterBoss), metastructure, optionalMetadata);
        break;
      case ProceduralFlowModifierData.FlowModifierPlacementType.BLACK_MARKET:
        flag3 = this.InjectNodeAfter(currentInjectionData, exactRoom, root, new Action<BuilderFlowNode, List<BuilderFlowNode>, ProceduralFlowModifierData, FlowCompositeMetastructure>(this.InjectValidator_BlackMarket), metastructure, optionalMetadata);
        break;
    }
    if (flag3 && (UnityEngine.Object) exactRoom.requiredInjectionData != (UnityEngine.Object) null)
    {
      RuntimeInjectionMetadata sourceMetadata = new RuntimeInjectionMetadata(exactRoom.requiredInjectionData);
      this.HandleNodeInjection(root, sourceMetadata, injectionFlags, metastructure);
    }
    return flag3;
  }

  protected void HandleNodeInjection(
    BuilderFlowNode root,
    RuntimeInjectionMetadata sourceMetadata,
    RuntimeInjectionFlags injectionFlags,
    FlowCompositeMetastructure metastructure)
  {
    SharedInjectionData injectionData = sourceMetadata.injectionData;
    if ((UnityEngine.Object) injectionData != (UnityEngine.Object) null && injectionData.InjectionData.Count > 0)
    {
      List<int> intList = Enumerable.Range(0, injectionData.InjectionData.Count).ToList<int>().GenerationShuffle<int>();
      if (injectionData.OnlyOne)
      {
        ProceduralFlowModifierData currentInjectionData = (ProceduralFlowModifierData) null;
        float num1 = injectionData.ChanceToSpawnOne;
        bool flag = false;
        if (injectionData.IsNPCCell)
        {
          num1 += (float) GameStatsManager.Instance.NumberRunsValidCellWithoutSpawn / 50f;
          if (MetaInjectionData.CellGeneratedForCurrentBlueprint || BraveRandom.IgnoreGenerationDifferentiator)
            num1 = 0.0f;
          if (injectionData.InjectionData.Count > 1 && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            num1 = 0.0f;
          if (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON && !GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_ACTIVE_IN_FOYER))
          {
            flag = true;
            num1 = 1f;
          }
        }
        if ((double) BraveRandom.GenerationRandomValue() < (double) num1)
        {
          float num2 = 0.0f;
          for (int index = 0; index < injectionData.InjectionData.Count; ++index)
          {
            ProceduralFlowModifierData flowModifierData = injectionData.InjectionData[index];
            if ((!flowModifierData.OncePerRun || !MetaInjectionData.InjectionSetsUsedThisRun.Contains(flowModifierData)) && (!injectionData.IsNPCCell || flowModifierData.PrerequisitesMet) && (!injectionData.IgnoreUnmetPrerequisiteEntries || flowModifierData.PrerequisitesMet))
              num2 += flowModifierData.selectionWeight;
          }
          float num3 = BraveRandom.GenerationRandomValue() * num2;
          float num4 = 0.0f;
          if (sourceMetadata != null && sourceMetadata.HasAssignedModDataExactRoom)
          {
            currentInjectionData = sourceMetadata.AssignedModifierData;
          }
          else
          {
            ProceduralFlowModifierData lostAdventurerSet;
            if (this.ShouldDoLostAdventurerHelp(injectionData, out lostAdventurerSet))
            {
              currentInjectionData = lostAdventurerSet;
              if (flag)
                currentInjectionData = injectionData.InjectionData[0];
            }
            else
            {
              for (int index = 0; index < injectionData.InjectionData.Count; ++index)
              {
                ProceduralFlowModifierData flowModifierData = injectionData.InjectionData[index];
                if ((!flowModifierData.OncePerRun || !MetaInjectionData.InjectionSetsUsedThisRun.Contains(flowModifierData)) && (!injectionData.IsNPCCell || flowModifierData.PrerequisitesMet) && (!injectionData.IgnoreUnmetPrerequisiteEntries || flowModifierData.PrerequisitesMet))
                {
                  num4 += flowModifierData.selectionWeight;
                  if ((double) num4 > (double) num3)
                  {
                    currentInjectionData = flowModifierData;
                    break;
                  }
                }
              }
              if (flag)
                currentInjectionData = injectionData.InjectionData[0];
            }
          }
          if (sourceMetadata != null && !sourceMetadata.HasAssignedModDataExactRoom)
          {
            sourceMetadata.HasAssignedModDataExactRoom = true;
            if (currentInjectionData != null)
              UnityEngine.Debug.Log((object) ("Assigning METADATA: " + currentInjectionData.annotation));
            sourceMetadata.AssignedModifierData = currentInjectionData;
            if (currentInjectionData != null && currentInjectionData.OncePerRun)
              MetaInjectionData.InjectionSetsUsedThisRun.Add(currentInjectionData);
          }
          if (currentInjectionData != null && !this.ProcessSingleNodeInjection(currentInjectionData, root, injectionFlags, metastructure, sourceMetadata))
            ;
        }
      }
      else
      {
        for (int index = 0; index < injectionData.InjectionData.Count; ++index)
          this.ProcessSingleNodeInjection(injectionData.InjectionData[intList[index]], root, injectionFlags, metastructure, sourceMetadata);
      }
    }
    if (!((UnityEngine.Object) injectionData != (UnityEngine.Object) null) || injectionData.AttachedInjectionData.Count <= 0)
      return;
    for (int index = 0; index < injectionData.AttachedInjectionData.Count; ++index)
    {
      RuntimeInjectionMetadata sourceMetadata1 = new RuntimeInjectionMetadata(injectionData.AttachedInjectionData[index]);
      sourceMetadata1.CopyMetadata(sourceMetadata);
      this.HandleNodeInjection(root, sourceMetadata1, injectionFlags, metastructure);
    }
  }

  private bool ShouldDoLostAdventurerHelp(
    SharedInjectionData injectionData,
    out ProceduralFlowModifierData lostAdventurerSet)
  {
    lostAdventurerSet = (ProceduralFlowModifierData) null;
    for (int index = 0; index < injectionData.InjectionData.Count; ++index)
    {
      ProceduralFlowModifierData flowModifierData = injectionData.InjectionData[index];
      if ((!flowModifierData.OncePerRun || !MetaInjectionData.InjectionSetsUsedThisRun.Contains(flowModifierData)) && (!injectionData.IsNPCCell || flowModifierData.PrerequisitesMet) && (!injectionData.IgnoreUnmetPrerequisiteEntries || flowModifierData.PrerequisitesMet) && !(flowModifierData.annotation != "lost adventurer"))
      {
        GungeonFlags? flagFromFloor = this.LostAdventurerGetFlagFromFloor(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
        if (!flagFromFloor.HasValue || this.LostAdventurerGetFloorsHelped() != 4 || GameStatsManager.Instance.GetFlag(flagFromFloor.Value))
          return false;
        lostAdventurerSet = flowModifierData;
        return true;
      }
    }
    return false;
  }

  private GungeonFlags? LostAdventurerGetFlagFromFloor(GlobalDungeonData.ValidTilesets floor)
  {
    switch (floor)
    {
      case GlobalDungeonData.ValidTilesets.GUNGEON:
        return new GungeonFlags?(GungeonFlags.LOST_ADVENTURER_HELPED_GUNGEON);
      case GlobalDungeonData.ValidTilesets.CASTLEGEON:
        return new GungeonFlags?(GungeonFlags.LOST_ADVENTURER_HELPED_CASTLE);
      case GlobalDungeonData.ValidTilesets.MINEGEON:
        return new GungeonFlags?(GungeonFlags.LOST_ADVENTURER_HELPED_MINES);
      case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
        return new GungeonFlags?(GungeonFlags.LOST_ADVENTURER_HELPED_CATACOMBS);
      case GlobalDungeonData.ValidTilesets.FORGEGEON:
        return new GungeonFlags?(GungeonFlags.LOST_ADVENTURER_HELPED_FORGE);
      default:
        return new GungeonFlags?();
    }
  }

  private int LostAdventurerGetFloorsHelped()
  {
    List<GungeonFlags> gungeonFlagsList = new List<GungeonFlags>((IEnumerable<GungeonFlags>) new GungeonFlags[5]
    {
      GungeonFlags.LOST_ADVENTURER_HELPED_CASTLE,
      GungeonFlags.LOST_ADVENTURER_HELPED_GUNGEON,
      GungeonFlags.LOST_ADVENTURER_HELPED_MINES,
      GungeonFlags.LOST_ADVENTURER_HELPED_CATACOMBS,
      GungeonFlags.LOST_ADVENTURER_HELPED_FORGE
    });
    int floorsHelped = 0;
    for (int index = 0; index < gungeonFlagsList.Count; ++index)
    {
      if (GameStatsManager.Instance.GetFlag(gungeonFlagsList[index]))
        ++floorsHelped;
    }
    return floorsHelped;
  }

  protected void HandleNodeInjection(
    BuilderFlowNode root,
    List<ProceduralFlowModifierData> flowInjectionData,
    RuntimeInjectionFlags injectionFlags,
    FlowCompositeMetastructure metastructure)
  {
    if (flowInjectionData == null || flowInjectionData.Count <= 0)
      return;
    for (int index = 0; index < flowInjectionData.Count; ++index)
      this.ProcessSingleNodeInjection(flowInjectionData[index], root, injectionFlags, metastructure);
  }

  protected BuilderFlowNode ComposeFlowTree()
  {
    Stack<BuilderFlowNode> builderFlowNodeStack = new Stack<BuilderFlowNode>();
    builderFlowNodeStack.Push(new BuilderFlowNode(this.m_flow.FirstNode));
    BuilderFlowNode builderFlowNode = builderFlowNodeStack.Peek();
    int num1 = 0;
    builderFlowNode.identifier = num1;
    int num2 = num1 + 1;
    while (builderFlowNodeStack.Count > 0)
    {
      BuilderFlowNode parentBuilderNode = builderFlowNodeStack.Pop();
      if (parentBuilderNode.childBuilderNodes == null)
        parentBuilderNode.childBuilderNodes = this.m_flow.NewGetNodeChildrenToBuild(parentBuilderNode, this);
      this.allBuilderNodes.Add(parentBuilderNode);
      for (int index = 0; index < parentBuilderNode.childBuilderNodes.Count; ++index)
      {
        if (!builderFlowNodeStack.Contains(parentBuilderNode.childBuilderNodes[index]))
        {
          if (parentBuilderNode.childBuilderNodes[index].identifier < 0)
          {
            parentBuilderNode.childBuilderNodes[index].identifier = num2;
            ++num2;
          }
          else
            UnityEngine.Debug.Log((object) "assigning already-assigned identifier");
          builderFlowNodeStack.Push(parentBuilderNode.childBuilderNodes[index]);
        }
      }
    }
    for (int index1 = 0; index1 < this.allBuilderNodes.Count; ++index1)
    {
      if (!string.IsNullOrEmpty(this.allBuilderNodes[index1].node.loopTargetNodeGuid))
      {
        DungeonFlowNode nodeFromGuid = this.m_flow.GetNodeFromGuid(this.allBuilderNodes[index1].node.loopTargetNodeGuid);
        for (int index2 = 0; index2 < this.allBuilderNodes.Count; ++index2)
        {
          if (this.allBuilderNodes[index2].node == nodeFromGuid)
          {
            this.allBuilderNodes[index1].loopConnectedBuilderNode = this.allBuilderNodes[index2];
            this.allBuilderNodes[index2].loopConnectedBuilderNode = this.allBuilderNodes[index1];
          }
        }
      }
    }
    return builderFlowNode;
  }

  protected BuilderFlowNode RerootTreeAtHighestConnectivity(BuilderFlowNode root)
  {
    int connectivity = root.Connectivity;
    BuilderFlowNode builderFlowNode1 = root;
    Queue<BuilderFlowNode> builderFlowNodeQueue = new Queue<BuilderFlowNode>();
    builderFlowNodeQueue.Enqueue(root);
    while (builderFlowNodeQueue.Count > 0)
    {
      BuilderFlowNode builderFlowNode2 = builderFlowNodeQueue.Dequeue();
      if (builderFlowNode2.Connectivity > connectivity)
      {
        connectivity = builderFlowNode2.Connectivity;
        builderFlowNode1 = builderFlowNode2;
      }
      for (int index = 0; index < builderFlowNode2.childBuilderNodes.Count; ++index)
        builderFlowNodeQueue.Enqueue(builderFlowNode2.childBuilderNodes[index]);
    }
    builderFlowNode1.MakeNodeTreeRoot();
    return builderFlowNode1;
  }

  protected void PerformOperationOnTreeNodes(BuilderFlowNode root, Action<BuilderFlowNode> action)
  {
    Queue<BuilderFlowNode> builderFlowNodeQueue = new Queue<BuilderFlowNode>();
    builderFlowNodeQueue.Enqueue(root);
    while (builderFlowNodeQueue.Count > 0)
    {
      BuilderFlowNode builderFlowNode = builderFlowNodeQueue.Dequeue();
      action(builderFlowNode);
      for (int index = 0; index < builderFlowNode.childBuilderNodes.Count; ++index)
        builderFlowNodeQueue.Enqueue(builderFlowNode.childBuilderNodes[index]);
    }
  }

  protected DungeonFlowSubtypeRestriction GetSubtypeRestrictionFromRoom(PrototypeDungeonRoom room)
  {
    foreach (DungeonFlowSubtypeRestriction key in this.roomsOfSubtypeRemaining.Keys)
    {
      if (key.baseCategoryRestriction == room.category && (room.category == PrototypeDungeonRoom.RoomCategory.BOSS && room.subCategoryBoss == key.bossSubcategoryRestriction || room.category == PrototypeDungeonRoom.RoomCategory.NORMAL && room.subCategoryNormal == key.normalSubcategoryRestriction || room.category == PrototypeDungeonRoom.RoomCategory.SPECIAL && room.subCategorySpecial == key.specialSubcategoryRestriction || room.category == PrototypeDungeonRoom.RoomCategory.SECRET && room.subCategorySecret == key.secretSubcategoryRestriction))
        return key;
    }
    return (DungeonFlowSubtypeRestriction) null;
  }

  protected bool CheckRoomAgainstRestrictedSubtypes(PrototypeDungeonRoom room)
  {
    DungeonFlowSubtypeRestriction restrictionFromRoom = this.GetSubtypeRestrictionFromRoom(room);
    return restrictionFromRoom != null && this.roomsOfSubtypeRemaining[restrictionFromRoom] <= 0;
  }

  protected List<WeightedRoom> GetViableAvailableRooms(
    PrototypeDungeonRoom.RoomCategory category,
    int requiredExits,
    List<WeightedRoom> source,
    out float totalAvailableWeight,
    LoopFlowBuilder.FallbackLevel fallback = LoopFlowBuilder.FallbackLevel.NOT_FALLBACK)
  {
    List<WeightedRoom> viableAvailableRooms = new List<WeightedRoom>();
    List<int> intList = Enumerable.Range(0, source.Count).ToList<int>().GenerationShuffle<int>();
    totalAvailableWeight = 0.0f;
    for (int index1 = 0; index1 < source.Count; ++index1)
    {
      int index2 = intList[index1];
      WeightedRoom weightedRoom = source[index2];
      PrototypeDungeonRoom room = weightedRoom.room;
      float weight = weightedRoom.weight;
      if (!((UnityEngine.Object) weightedRoom.room == (UnityEngine.Object) null) && !this.CheckRoomAgainstRestrictedSubtypes(room) && room.exitData.exits.Count >= requiredExits && (requiredExits != 1 || room.category != PrototypeDungeonRoom.RoomCategory.NORMAL || room.subCategoryNormal != PrototypeDungeonRoom.RoomNormalSubCategory.TRAP) && (!Enum.IsDefined(typeof (PrototypeDungeonRoom.RoomCategory), (object) category) || room.category == category) && (fallback != LoopFlowBuilder.FallbackLevel.NOT_FALLBACK || weightedRoom.room.ForceAllowDuplicates || !this.m_usedPrototypeRoomData.ContainsKey(weightedRoom.room)))
      {
        int num = GameStatsManager.Instance.QueryRoomDifferentiator(weightedRoom.room);
        if (fallback == LoopFlowBuilder.FallbackLevel.NOT_FALLBACK && !weightedRoom.room.ForceAllowDuplicates && weightedRoom.room.category != PrototypeDungeonRoom.RoomCategory.SPECIAL && num > 0)
          weight *= Mathf.Clamp01((float) (1.0 - 0.33000001311302185 * (double) num));
        if (!this.m_excludedRoomData.Contains(weightedRoom.room) && weightedRoom.CheckPrerequisites() && room.CheckPrerequisites() && room.injectionFlags.IsValid(this.m_runtimeInjectionFlags) && (fallback == LoopFlowBuilder.FallbackLevel.FALLBACK_EMERGENCY || category == PrototypeDungeonRoom.RoomCategory.NORMAL || !weightedRoom.limitedCopies || !this.m_usedPrototypeRoomData.ContainsKey(weightedRoom.room) || this.m_usedPrototypeRoomData[weightedRoom.room] < weightedRoom.maxCopies))
        {
          viableAvailableRooms.Add(weightedRoom);
          totalAvailableWeight += weight;
        }
      }
    }
    return viableAvailableRooms;
  }

  public PrototypeDungeonRoom GetAvailableRoomByExitDirection(
    PrototypeDungeonRoom.RoomCategory category,
    int requiredExits,
    List<DungeonData.Direction> exitDirections,
    List<WeightedRoom> source,
    LoopFlowBuilder.FallbackLevel fallback = LoopFlowBuilder.FallbackLevel.NOT_FALLBACK)
  {
    float totalAvailableWeight = 0.0f;
    List<WeightedRoom> viableAvailableRooms = this.GetViableAvailableRooms(category, requiredExits, source, out totalAvailableWeight, fallback);
    for (int index1 = 0; index1 < viableAvailableRooms.Count; ++index1)
    {
      WeightedRoom weightedRoom = viableAvailableRooms[index1];
      bool flag = false;
      for (int index2 = 0; index2 < weightedRoom.room.exitData.exits.Count; ++index2)
      {
        PrototypeRoomExit exit = weightedRoom.room.exitData.exits[index2];
        if (exitDirections.Contains(exit.exitDirection))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        totalAvailableWeight -= weightedRoom.weight;
        viableAvailableRooms.RemoveAt(index1);
        --index1;
      }
    }
    if (viableAvailableRooms.Count == 0 && fallback == LoopFlowBuilder.FallbackLevel.NOT_FALLBACK)
      return this.GetAvailableRoomByExitDirection(category, requiredExits, exitDirections, source, LoopFlowBuilder.FallbackLevel.FALLBACK_STANDARD);
    if (viableAvailableRooms.Count == 0 && fallback == LoopFlowBuilder.FallbackLevel.FALLBACK_EMERGENCY)
      return this.GetAvailableRoomByExitDirection(category, requiredExits, exitDirections, source, LoopFlowBuilder.FallbackLevel.FALLBACK_EMERGENCY);
    if (viableAvailableRooms.Count == 0)
    {
      if (category == PrototypeDungeonRoom.RoomCategory.CONNECTOR)
        return this.GetAvailableRoomByExitDirection(PrototypeDungeonRoom.RoomCategory.NORMAL, requiredExits, exitDirections, source);
      UnityEngine.Debug.LogError((object) "Falling back due to lack of non-duplicate rooms FAILED. This should never happen.");
    }
    float num = BraveRandom.GenerationRandomValue() * totalAvailableWeight;
    for (int index = 0; index < viableAvailableRooms.Count; ++index)
    {
      num -= viableAvailableRooms[index].weight;
      if ((double) num <= 0.0)
        return viableAvailableRooms[index].room;
    }
    return viableAvailableRooms == null || viableAvailableRooms.Count == 0 ? (PrototypeDungeonRoom) null : viableAvailableRooms[0].room;
  }

  public PrototypeDungeonRoom GetAvailableRoom(
    PrototypeDungeonRoom.RoomCategory category,
    int requiredExits,
    List<WeightedRoom> source,
    LoopFlowBuilder.FallbackLevel fallback = LoopFlowBuilder.FallbackLevel.NOT_FALLBACK)
  {
    float totalAvailableWeight = 0.0f;
    List<WeightedRoom> viableAvailableRooms = this.GetViableAvailableRooms(category, requiredExits, source, out totalAvailableWeight, fallback);
    if (viableAvailableRooms.Count == 0 && fallback == LoopFlowBuilder.FallbackLevel.NOT_FALLBACK)
      return this.GetAvailableRoom(category, requiredExits, source, LoopFlowBuilder.FallbackLevel.FALLBACK_STANDARD);
    if (viableAvailableRooms.Count == 0 && fallback == LoopFlowBuilder.FallbackLevel.FALLBACK_STANDARD)
      return this.GetAvailableRoom(category, requiredExits, source, LoopFlowBuilder.FallbackLevel.FALLBACK_EMERGENCY);
    if (viableAvailableRooms.Count == 0)
    {
      switch (category)
      {
        case PrototypeDungeonRoom.RoomCategory.CONNECTOR:
        case PrototypeDungeonRoom.RoomCategory.HUB:
          UnityEngine.Debug.LogError((object) "Replacing failed CONNECTOR/HUB room with room of type NORMAL.");
          return this.GetAvailableRoom(PrototypeDungeonRoom.RoomCategory.NORMAL, requiredExits, source);
        case PrototypeDungeonRoom.RoomCategory.SECRET:
          return (PrototypeDungeonRoom) null;
        default:
          UnityEngine.Debug.LogError((object) $"Falling back due to lack of non-duplicate rooms ({requiredExits.ToString()},{source.Count.ToString()}) in list of length: {source.Count.ToString()}. FAILED: {category.ToString()}. This should never happen.");
          goto case PrototypeDungeonRoom.RoomCategory.SECRET;
      }
    }
    else
    {
      float num = BraveRandom.GenerationRandomValue() * totalAvailableWeight;
      for (int index = 0; index < viableAvailableRooms.Count; ++index)
      {
        num -= viableAvailableRooms[index].weight;
        if ((double) num <= 0.0)
          return viableAvailableRooms[index].room;
      }
      return viableAvailableRooms[0].room;
    }
  }

  public void ClearPlacedRoomData(BuilderFlowNode buildData)
  {
    if (!((UnityEngine.Object) buildData.assignedPrototypeRoom != (UnityEngine.Object) null))
      return;
    DungeonFlowSubtypeRestriction restrictionFromRoom = this.GetSubtypeRestrictionFromRoom(buildData.assignedPrototypeRoom);
    if (restrictionFromRoom != null)
      ++this.roomsOfSubtypeRemaining[restrictionFromRoom];
    if (this.m_usedPrototypeRoomData.ContainsKey(buildData.assignedPrototypeRoom))
    {
      if (this.m_usedPrototypeRoomData[buildData.assignedPrototypeRoom] > 1)
        this.m_usedPrototypeRoomData[buildData.assignedPrototypeRoom] = this.m_usedPrototypeRoomData[buildData.assignedPrototypeRoom] - 1;
      else
        this.m_usedPrototypeRoomData.Remove(buildData.assignedPrototypeRoom);
    }
    for (int index = 0; index < buildData.assignedPrototypeRoom.excludedOtherRooms.Count; ++index)
      this.m_excludedRoomData.Remove(buildData.assignedPrototypeRoom.excludedOtherRooms[index]);
    if (buildData.assignedPrototypeRoom.injectionFlags.CastleFireplace)
      this.m_runtimeInjectionFlags.CastleFireplace = false;
    buildData.assignedPrototypeRoom = (PrototypeDungeonRoom) null;
  }

  private bool PostprocessInjectionDataContains(SharedInjectionData test)
  {
    for (int index = 0; index < this.m_postprocessInjectionData.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_postprocessInjectionData[index].injectionData == (UnityEngine.Object) test)
        return true;
    }
    return false;
  }

  public void NotifyPlacedRoomData(PrototypeDungeonRoom assignedRoom)
  {
    PrototypeDungeonRoom prototypeDungeonRoom = !((UnityEngine.Object) assignedRoom.MirrorSource != (UnityEngine.Object) null) ? assignedRoom : assignedRoom.MirrorSource;
    DungeonFlowSubtypeRestriction restrictionFromRoom = this.GetSubtypeRestrictionFromRoom(prototypeDungeonRoom);
    if (restrictionFromRoom != null)
      --this.roomsOfSubtypeRemaining[restrictionFromRoom];
    if (this.m_usedPrototypeRoomData.ContainsKey(prototypeDungeonRoom))
      ++this.m_usedPrototypeRoomData[prototypeDungeonRoom];
    else
      this.m_usedPrototypeRoomData.Add(prototypeDungeonRoom, 1);
    for (int index = 0; index < prototypeDungeonRoom.excludedOtherRooms.Count; ++index)
      this.m_excludedRoomData.Add(prototypeDungeonRoom.excludedOtherRooms[index]);
    if ((UnityEngine.Object) prototypeDungeonRoom.requiredInjectionData != (UnityEngine.Object) null && !this.PostprocessInjectionDataContains(prototypeDungeonRoom.requiredInjectionData))
      this.m_postprocessInjectionData.Add(new RuntimeInjectionMetadata(prototypeDungeonRoom.requiredInjectionData));
    if (!this.m_runtimeInjectionFlags.Merge(prototypeDungeonRoom.injectionFlags))
      return;
    UnityEngine.Debug.Log((object) ("Assigning FIREPLACE from room " + prototypeDungeonRoom.name));
  }

  protected void HandleBossFoyerAcquisition(BuilderFlowNode buildData)
  {
    this.HandleBossFoyerAcquisition(buildData, false);
  }

  protected void HandleBossFoyerAcquisition(BuilderFlowNode buildData, bool isFallback)
  {
    BuilderFlowNode builderFlowNode = (BuilderFlowNode) null;
    for (int index = 0; index < buildData.childBuilderNodes.Count; ++index)
    {
      if (buildData.childBuilderNodes[index].Category == PrototypeDungeonRoom.RoomCategory.BOSS)
        builderFlowNode = buildData.childBuilderNodes[index];
    }
    if (builderFlowNode == null)
      return;
    this.ClearPlacedRoomData(buildData);
    if ((UnityEngine.Object) buildData.node.overrideExactRoom != (UnityEngine.Object) null)
    {
      buildData.assignedPrototypeRoom = buildData.node.overrideExactRoom;
    }
    else
    {
      GenericRoomTable genericRoomTable = this.m_flow.fallbackRoomTable;
      if ((UnityEngine.Object) buildData.node.overrideRoomTable != (UnityEngine.Object) null)
        genericRoomTable = buildData.node.overrideRoomTable;
      List<WeightedRoom> weightedRoomList = new List<WeightedRoom>((IEnumerable<WeightedRoom>) genericRoomTable.GetCompiledList());
      for (int index1 = 0; index1 < weightedRoomList.Count; ++index1)
      {
        PrototypeDungeonRoom room = weightedRoomList[index1].room;
        if (!isFallback && !room.CheckPrerequisites())
        {
          weightedRoomList.RemoveAt(index1);
          --index1;
        }
        else
        {
          bool flag = false;
          if ((UnityEngine.Object) room != (UnityEngine.Object) null)
          {
            for (int index2 = 0; index2 < builderFlowNode.assignedPrototypeRoom.exitData.exits.Count; ++index2)
            {
              if (builderFlowNode.assignedPrototypeRoom.exitData.exits[index2].exitType != PrototypeRoomExit.ExitType.EXIT_ONLY && room.GetExitsMatchingDirection((DungeonData.Direction) ((int) (builderFlowNode.assignedPrototypeRoom.exitData.exits[index2].exitDirection + 4) % 8), PrototypeRoomExit.ExitType.EXIT_ONLY).Count > 0)
              {
                flag = true;
                break;
              }
            }
          }
          if (!flag)
          {
            weightedRoomList.RemoveAt(index1);
            --index1;
          }
        }
      }
      PrototypeDungeonRoom prototypeDungeonRoom = (PrototypeDungeonRoom) null;
      float num1 = 0.0f;
      for (int index = 0; index < weightedRoomList.Count; ++index)
        num1 += weightedRoomList[index].weight;
      float num2 = BraveRandom.GenerationRandomValue() * num1;
      for (int index = 0; index < weightedRoomList.Count; ++index)
      {
        num2 -= weightedRoomList[index].weight;
        if ((double) num2 <= 0.0)
        {
          prototypeDungeonRoom = weightedRoomList[index].room;
          break;
        }
      }
      if (weightedRoomList.Count > 0 && (UnityEngine.Object) prototypeDungeonRoom == (UnityEngine.Object) null)
        prototypeDungeonRoom = weightedRoomList[weightedRoomList.Count - 1].room;
      if ((UnityEngine.Object) prototypeDungeonRoom != (UnityEngine.Object) null)
      {
        buildData.assignedPrototypeRoom = prototypeDungeonRoom;
      }
      else
      {
        if (!isFallback)
        {
          this.HandleBossFoyerAcquisition(buildData, true);
          return;
        }
        UnityEngine.Debug.LogError((object) "Failed to acquire a boss foyer! Something has gone wrong, or there is somehow not a boss foyer that matches the entrance direction for this boss chamber.");
      }
    }
    if (!((UnityEngine.Object) buildData.assignedPrototypeRoom != (UnityEngine.Object) null))
      return;
    this.NotifyPlacedRoomData(buildData.assignedPrototypeRoom);
  }

  protected void AcquirePrototypeRoom(BuilderFlowNode buildData)
  {
    if (this.roomsOfSubtypeRemaining == null)
    {
      this.roomsOfSubtypeRemaining = new Dictionary<DungeonFlowSubtypeRestriction, int>();
      for (int index = 0; index < this.m_flow.subtypeRestrictions.Count; ++index)
        this.roomsOfSubtypeRemaining.Add(this.m_flow.subtypeRestrictions[index], this.m_flow.subtypeRestrictions[index].maximumRoomsOfSubtype);
    }
    this.ClearPlacedRoomData(buildData);
    if (buildData.node.UsesGlobalBossData)
      buildData.assignedPrototypeRoom = GameManager.Instance.BossManager.SelectBossRoom();
    else if ((UnityEngine.Object) buildData.node.overrideExactRoom != (UnityEngine.Object) null)
    {
      buildData.assignedPrototypeRoom = buildData.node.overrideExactRoom;
    }
    else
    {
      PrototypeDungeonRoom.RoomCategory category = !buildData.usesOverrideCategory ? buildData.node.roomCategory : buildData.overrideCategory;
      if (category == PrototypeDungeonRoom.RoomCategory.CONNECTOR)
      {
        buildData.AcquiresRoomAsNecessary = true;
      }
      else
      {
        GenericRoomTable genericRoomTable = this.m_flow.fallbackRoomTable;
        if ((UnityEngine.Object) buildData.node.overrideRoomTable != (UnityEngine.Object) null)
          genericRoomTable = buildData.node.overrideRoomTable;
        List<WeightedRoom> compiledList = genericRoomTable.GetCompiledList();
        PrototypeDungeonRoom availableRoom = this.GetAvailableRoom(category, buildData.Connectivity, compiledList);
        if ((UnityEngine.Object) availableRoom != (UnityEngine.Object) null)
          buildData.assignedPrototypeRoom = availableRoom;
        else if (category != PrototypeDungeonRoom.RoomCategory.SECRET)
          UnityEngine.Debug.LogError((object) $"Failed to acquire a prototype room. This means the list is too sparse for the relevant category ({category.ToString()}) or something has gone terribly wrong.");
      }
    }
    if ((UnityEngine.Object) buildData.assignedPrototypeRoom != (UnityEngine.Object) null)
      this.NotifyPlacedRoomData(buildData.assignedPrototypeRoom);
    else if (buildData.AcquiresRoomAsNecessary || buildData.node.priority != DungeonFlowNode.NodePriority.OPTIONAL)
      ;
  }

  protected void AssignInjectionDataToRoomHandler(BuilderFlowNode buildData)
  {
    if (buildData.instanceRoom == null)
      return;
    if (buildData.InjectionTarget != null)
      buildData.instanceRoom.injectionTarget = buildData.InjectionTarget.instanceRoom;
    if (buildData.InjectionFrameSequence == null)
      return;
    List<RoomHandler> roomHandlerList = new List<RoomHandler>();
    for (int index = 0; index < buildData.InjectionFrameSequence.Count; ++index)
      roomHandlerList.Add(buildData.InjectionFrameSequence[index].instanceRoom);
    buildData.instanceRoom.injectionFrameData = roomHandlerList;
  }

  protected void DebugPrintTree(BuilderFlowNode root)
  {
    Stack<BuilderFlowNode> builderFlowNodeStack = new Stack<BuilderFlowNode>();
    builderFlowNodeStack.Push(root);
    while (builderFlowNodeStack.Count > 0)
    {
      BuilderFlowNode builderFlowNode = builderFlowNodeStack.Pop();
      if (builderFlowNode.node != (DungeonFlowNode) null)
        UnityEngine.Debug.Log((object) $"{(object) builderFlowNode.identifier}|{builderFlowNode.node.roomCategory.ToString()}");
      for (int index = 0; index < builderFlowNode.childBuilderNodes.Count; ++index)
        builderFlowNodeStack.Push(builderFlowNode.childBuilderNodes[index]);
    }
  }

  public List<BuilderFlowNode> FindPathBetweenNodesAdvanced(
    BuilderFlowNode origin,
    BuilderFlowNode target,
    List<Tuple<BuilderFlowNode, BuilderFlowNode>> excludedConnections)
  {
    Dictionary<BuilderFlowNode, int> dictionary1 = new Dictionary<BuilderFlowNode, int>();
    Dictionary<BuilderFlowNode, BuilderFlowNode> dictionary2 = new Dictionary<BuilderFlowNode, BuilderFlowNode>();
    for (int index = 0; index < this.allBuilderNodes.Count; ++index)
    {
      int num = int.MaxValue;
      if (this.allBuilderNodes[index] == origin)
        num = 0;
      dictionary1.Add(this.allBuilderNodes[index], num);
    }
    BuilderFlowNode key1 = origin;
    int num1 = 1;
    do
    {
      List<BuilderFlowNode> excluded = LoopFlowBuilder.BuilderFlowNodeListPool.Allocate();
      for (int index = 0; index < excludedConnections.Count; ++index)
      {
        Tuple<BuilderFlowNode, BuilderFlowNode> excludedConnection = excludedConnections[index];
        if (excludedConnection.First == key1)
          excluded.Add(excludedConnection.Second);
      }
      List<BuilderFlowNode> allConnectedNodes = key1.GetAllConnectedNodes(excluded);
      excluded.Clear();
      LoopFlowBuilder.BuilderFlowNodeListPool.Free(ref excluded);
      for (int index = 0; index < allConnectedNodes.Count; ++index)
      {
        if (allConnectedNodes[index] == target)
        {
          dictionary2.Add(allConnectedNodes[index], key1);
          goto label_29;
        }
        if (dictionary1.ContainsKey(allConnectedNodes[index]) && dictionary1[allConnectedNodes[index]] > num1)
        {
          dictionary1[allConnectedNodes[index]] = num1;
          if (dictionary2.ContainsKey(allConnectedNodes[index]))
            dictionary2[allConnectedNodes[index]] = key1;
          else
            dictionary2.Add(allConnectedNodes[index], key1);
        }
      }
      dictionary1.Remove(key1);
      if (dictionary1.Count == 0)
        return (List<BuilderFlowNode>) null;
      key1 = (BuilderFlowNode) null;
      num1 = int.MaxValue;
      foreach (BuilderFlowNode key2 in dictionary1.Keys)
      {
        if (dictionary1[key2] < num1)
        {
          key1 = key2;
          num1 = dictionary1[key2];
        }
      }
    }
    while (key1 != null);
label_29:
    if (!dictionary2.ContainsKey(target))
      return (List<BuilderFlowNode>) null;
    List<BuilderFlowNode> betweenNodesAdvanced = new List<BuilderFlowNode>();
    for (BuilderFlowNode key3 = target; key3 != null; key3 = !dictionary2.ContainsKey(key3) ? (BuilderFlowNode) null : dictionary2[key3])
      betweenNodesAdvanced.Insert(0, key3);
    return betweenNodesAdvanced;
  }

  public List<BuilderFlowNode> FindPathBetweenNodes(
    BuilderFlowNode origin,
    BuilderFlowNode target,
    bool excludeDirect = false,
    params BuilderFlowNode[] excluded)
  {
    Dictionary<BuilderFlowNode, int> dictionary1 = new Dictionary<BuilderFlowNode, int>();
    Dictionary<BuilderFlowNode, BuilderFlowNode> dictionary2 = new Dictionary<BuilderFlowNode, BuilderFlowNode>();
    for (int index = 0; index < this.allBuilderNodes.Count; ++index)
    {
      int num = int.MaxValue;
      if (this.allBuilderNodes[index] == origin)
        num = 0;
      dictionary1.Add(this.allBuilderNodes[index], num);
    }
    BuilderFlowNode key1 = origin;
    int num1 = 1;
    BuilderFlowNode[] builderFlowNodeArray1;
    if (excluded == null)
      builderFlowNodeArray1 = new BuilderFlowNode[1]
      {
        target
      };
    else
      builderFlowNodeArray1 = new BuilderFlowNode[excluded.Length + 1];
    BuilderFlowNode[] builderFlowNodeArray2 = builderFlowNodeArray1;
    if (excluded != null)
    {
      builderFlowNodeArray2[builderFlowNodeArray2.Length - 1] = target;
      for (int index = 0; index < excluded.Length; ++index)
        builderFlowNodeArray2[index] = excluded[index];
    }
    do
    {
      List<BuilderFlowNode> builderFlowNodeList = !excludeDirect || key1 != origin ? key1.GetAllConnectedNodes(excluded) : key1.GetAllConnectedNodes(builderFlowNodeArray2);
      for (int index = 0; index < builderFlowNodeList.Count; ++index)
      {
        if (builderFlowNodeList[index] == target)
        {
          dictionary2.Add(builderFlowNodeList[index], key1);
          goto label_30;
        }
        if (dictionary1.ContainsKey(builderFlowNodeList[index]) && dictionary1[builderFlowNodeList[index]] > num1)
        {
          dictionary1[builderFlowNodeList[index]] = num1;
          if (dictionary2.ContainsKey(builderFlowNodeList[index]))
            dictionary2[builderFlowNodeList[index]] = key1;
          else
            dictionary2.Add(builderFlowNodeList[index], key1);
        }
      }
      dictionary1.Remove(key1);
      if (dictionary1.Count == 0)
        return (List<BuilderFlowNode>) null;
      key1 = (BuilderFlowNode) null;
      num1 = int.MaxValue;
      foreach (BuilderFlowNode key2 in dictionary1.Keys)
      {
        if (dictionary1[key2] < num1)
        {
          key1 = key2;
          num1 = dictionary1[key2];
        }
      }
    }
    while (key1 != null);
label_30:
    if (!dictionary2.ContainsKey(target))
      return (List<BuilderFlowNode>) null;
    List<BuilderFlowNode> pathBetweenNodes = new List<BuilderFlowNode>();
    for (BuilderFlowNode key3 = target; key3 != null; key3 = !dictionary2.ContainsKey(key3) ? (BuilderFlowNode) null : dictionary2[key3])
      pathBetweenNodes.Insert(0, key3);
    return pathBetweenNodes;
  }

  public List<BuilderFlowNode> GetSubloopsFromLoop(LoopBuilderComposite loopComposite)
  {
    List<Tuple<BuilderFlowNode, BuilderFlowNode>> excludedConnections = new List<Tuple<BuilderFlowNode, BuilderFlowNode>>();
    for (int index1 = 0; index1 < loopComposite.Nodes.Count; ++index1)
    {
      BuilderFlowNode node = loopComposite.Nodes[index1];
      List<BuilderFlowNode> allConnectedNodes = node.GetAllConnectedNodes();
      for (int index2 = 0; index2 < allConnectedNodes.Count; ++index2)
      {
        BuilderFlowNode second = allConnectedNodes[index2];
        if (loopComposite.Nodes.Contains(second))
          excludedConnections.Add(new Tuple<BuilderFlowNode, BuilderFlowNode>(node, second));
      }
    }
    for (int index3 = 0; index3 < loopComposite.Nodes.Count; ++index3)
    {
      BuilderFlowNode node1 = loopComposite.Nodes[index3];
      for (int index4 = index3 + 1; index4 < loopComposite.Nodes.Count; ++index4)
      {
        BuilderFlowNode node2 = loopComposite.Nodes[index4];
        List<BuilderFlowNode> betweenNodesAdvanced = this.FindPathBetweenNodesAdvanced(node1, node2, excludedConnections);
        if (betweenNodesAdvanced != null)
          return betweenNodesAdvanced;
      }
    }
    return (List<BuilderFlowNode>) null;
  }

  public List<BuilderFlowNode> FindSimplestContainingLoop(
    BuilderFlowNode origin,
    List<BuilderFlowNode> usedNodes)
  {
    List<BuilderFlowNode> allConnectedNodes = origin.GetAllConnectedNodes();
    List<BuilderFlowNode> simplestContainingLoop = (List<BuilderFlowNode>) null;
    int num = int.MaxValue;
    for (int index = 0; index < allConnectedNodes.Count; ++index)
    {
      List<BuilderFlowNode> pathBetweenNodes = this.FindPathBetweenNodes(origin, allConnectedNodes[index], true, usedNodes.ToArray());
      if (pathBetweenNodes != null && pathBetweenNodes.Count < num)
      {
        num = pathBetweenNodes.Count;
        simplestContainingLoop = pathBetweenNodes;
      }
    }
    return simplestContainingLoop;
  }

  public void ConvertTreeToCompositeStructure(
    BuilderFlowNode currentRoot,
    List<BuilderFlowNode> currentRunningList,
    FlowCompositeMetastructure currentMetastructure)
  {
    List<BuilderFlowNode> simplestContainingLoop = this.FindSimplestContainingLoop(currentRoot, currentMetastructure.usedList);
    if (simplestContainingLoop != null)
    {
      currentMetastructure.loopLists.Add(simplestContainingLoop);
      LoopBuilderComposite builderComposite = new LoopBuilderComposite(simplestContainingLoop, this.m_flow, this, LoopBuilderComposite.CompositeStyle.LOOP);
      currentMetastructure.usedList.AddRange((IEnumerable<BuilderFlowNode>) simplestContainingLoop);
      List<BuilderFlowNode> externalConnectedNodes = builderComposite.ExternalConnectedNodes;
      for (int index = 0; index < externalConnectedNodes.Count; ++index)
      {
        BuilderFlowNode builderFlowNode = externalConnectedNodes[index];
        BuilderFlowNode connectedInternalNode = builderComposite.GetConnectedInternalNode(builderFlowNode);
        if (builderFlowNode.loopConnectedBuilderNode != connectedInternalNode && connectedInternalNode.loopConnectedBuilderNode != builderFlowNode && !currentMetastructure.usedList.Contains(builderFlowNode))
          this.ConvertTreeToCompositeStructure(builderFlowNode, (List<BuilderFlowNode>) null, currentMetastructure);
      }
    }
    else
    {
      if (currentRoot.node.isWarpWingEntrance)
        currentRunningList = (List<BuilderFlowNode>) null;
      else if (currentRoot.IsInjectedNode && currentRoot.node.childNodeGuids.Count == 0)
        currentRunningList = (List<BuilderFlowNode>) null;
      if (currentRunningList == null)
      {
        currentRunningList = new List<BuilderFlowNode>();
        currentMetastructure.compositeLists.Add(currentRunningList);
      }
      currentRunningList.Add(currentRoot);
      currentMetastructure.usedList.Add(currentRoot);
      for (int index = 0; index < currentRoot.childBuilderNodes.Count; ++index)
      {
        BuilderFlowNode childBuilderNode = currentRoot.childBuilderNodes[index];
        if (!currentMetastructure.usedList.Contains(childBuilderNode))
          this.ConvertTreeToCompositeStructure(childBuilderNode, currentRunningList, currentMetastructure);
      }
    }
  }

  protected bool ConnectTwoPlacedLayoutNodes(
    BuilderFlowNode internalNode,
    BuilderFlowNode externalNode,
    SemioticLayoutManager layout)
  {
    List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> tupleList = new List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
    List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>> collection = new List<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>();
    bool flag = false;
    for (int index1 = 0; index1 < externalNode.instanceRoom.area.prototypeRoom.exitData.exits.Count; ++index1)
    {
      for (int index2 = 0; index2 < internalNode.instanceRoom.area.prototypeRoom.exitData.exits.Count; ++index2)
      {
        PrototypeRoomExit exit1 = externalNode.instanceRoom.area.prototypeRoom.exitData.exits[index1];
        PrototypeRoomExit exit2 = internalNode.instanceRoom.area.prototypeRoom.exitData.exits[index2];
        if (!externalNode.instanceRoom.area.instanceUsedExits.Contains(exit1) && !internalNode.instanceRoom.area.instanceUsedExits.Contains(exit2))
        {
          RuntimeRoomExitData first = new RuntimeRoomExitData(exit1);
          RuntimeRoomExitData second = new RuntimeRoomExitData(exit2);
          Tuple<RuntimeRoomExitData, RuntimeRoomExitData> tuple = new Tuple<RuntimeRoomExitData, RuntimeRoomExitData>(first, second);
          if (first.referencedExit.exitDirection == DungeonData.Direction.EAST && second.referencedExit.exitDirection == DungeonData.Direction.WEST || first.referencedExit.exitDirection == DungeonData.Direction.WEST && second.referencedExit.exitDirection == DungeonData.Direction.EAST || first.referencedExit.exitDirection == DungeonData.Direction.NORTH && second.referencedExit.exitDirection == DungeonData.Direction.SOUTH || first.referencedExit.exitDirection == DungeonData.Direction.SOUTH && second.referencedExit.exitDirection == DungeonData.Direction.NORTH)
            tupleList.Add(tuple);
          else if (first.referencedExit.exitDirection != second.referencedExit.exitDirection)
            collection.Add(tuple);
        }
      }
    }
    tupleList.AddRange((IEnumerable<Tuple<RuntimeRoomExitData, RuntimeRoomExitData>>) collection);
    RuntimeRoomExitData exitL = (RuntimeRoomExitData) null;
    RuntimeRoomExitData exitR = (RuntimeRoomExitData) null;
    List<IntVector2> inputPath = (List<IntVector2>) null;
    int num = int.MaxValue;
    for (int index = 0; index < tupleList.Count; ++index)
    {
      RuntimeRoomExitData first = tupleList[index].First;
      RuntimeRoomExitData second = tupleList[index].Second;
      PrototypeRoomExit referencedExit1 = first.referencedExit;
      PrototypeRoomExit referencedExit2 = second.referencedExit;
      IntVector2 startPosition = externalNode.instanceRoom.area.basePosition + referencedExit1.GetExitOrigin(referencedExit1.exitLength + 3) - IntVector2.One;
      IntVector2 endPosition = internalNode.instanceRoom.area.basePosition + referencedExit2.GetExitOrigin(referencedExit2.exitLength + 3) - IntVector2.One;
      SemioticLayoutManager semioticLayoutManager = new SemioticLayoutManager();
      semioticLayoutManager.MergeLayout(layout);
      RuntimeRoomExitData exit3 = new RuntimeRoomExitData(referencedExit1);
      exit3.additionalExitLength = 1;
      RuntimeRoomExitData exit4 = new RuntimeRoomExitData(referencedExit2);
      exit4.additionalExitLength = 1;
      semioticLayoutManager.StampComplexExitTemporary(exit3, externalNode.instanceRoom.area);
      semioticLayoutManager.StampComplexExitTemporary(exit4, internalNode.instanceRoom.area);
      List<IntVector2> intVector2List = semioticLayoutManager.PathfindHallway(startPosition, endPosition);
      semioticLayoutManager.ClearTemporary();
      semioticLayoutManager.OnDestroy();
      if (intVector2List != null && intVector2List.Count > 0 && intVector2List.Count < num)
      {
        exitL = first;
        exitR = second;
        inputPath = intVector2List;
        num = intVector2List.Count;
        flag = true;
      }
    }
    if (flag)
    {
      exitL.additionalExitLength = 0;
      exitR.additionalExitLength = 0;
      LoopBuilderComposite.PlaceProceduralPathRoom(inputPath, exitL, exitR, externalNode.instanceRoom, internalNode.instanceRoom, layout);
      if (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
        exitL.oneWayDoor = true;
      this.m_currentMaxLengthProceduralHallway = Mathf.Max(this.m_currentMaxLengthProceduralHallway, inputPath.Count);
    }
    return flag;
  }

  [DebuggerHidden]
  protected IEnumerable AttachWarpCanvasToLayout(
    BuilderFlowNode externalNode,
    BuilderFlowNode internalNode,
    SemioticLayoutManager canvas,
    SemioticLayoutManager layout)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoopFlowBuilder.<AttachWarpCanvasToLayout>c__Iterator0 layout1 = new LoopFlowBuilder.<AttachWarpCanvasToLayout>c__Iterator0()
    {
      externalNode = externalNode,
      internalNode = internalNode,
      layout = layout,
      canvas = canvas,
      $this = this
    };
    // ISSUE: reference to a compiler-generated field
    layout1.$PC = -2;
    return (IEnumerable) layout1;
  }

  protected bool NodeHasExitGroupsToCheck(BuilderFlowNode node)
  {
    List<PrototypeRoomExit.ExitGroup> definedExitGroups = node.assignedPrototypeRoom.exitData.GetDefinedExitGroups();
    bool check = definedExitGroups.Count > 1;
    for (int index = 0; index < node.instanceRoom.area.instanceUsedExits.Count; ++index)
      definedExitGroups.Remove(node.instanceRoom.area.instanceUsedExits[index].exitGroup);
    if (definedExitGroups.Count == 0)
      check = false;
    return check;
  }

  [DebuggerHidden]
  protected IEnumerable AttachNewCanvasToLayout(
    BuilderFlowNode externalNode,
    BuilderFlowNode internalNode,
    SemioticLayoutManager canvas,
    SemioticLayoutManager layout)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoopFlowBuilder.<AttachNewCanvasToLayout>c__Iterator1 layout1 = new LoopFlowBuilder.<AttachNewCanvasToLayout>c__Iterator1()
    {
      externalNode = externalNode,
      internalNode = internalNode,
      layout = layout,
      canvas = canvas,
      $this = this
    };
    // ISSUE: reference to a compiler-generated field
    layout1.$PC = -2;
    return (IEnumerable) layout1;
  }

  public SemioticLayoutManager Build(out bool generationSucceeded)
  {
    IEnumerator enumerator = this.DeferredBuild().GetEnumerator();
    do
      ;
    while (enumerator.MoveNext());
    generationSucceeded = this.DeferredGenerationSuccess;
    return this.DeferredGeneratedLayout;
  }

  private void AttachPregeneratedInjectionData()
  {
    if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH || GameManager.Instance.GeneratingLevelOverrideState != GameManager.LevelOverrideState.NONE)
      return;
    GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId;
    if (!MetaInjectionData.CurrentRunBlueprint.ContainsKey(tilesetId))
      return;
    for (int index = 0; index < MetaInjectionData.CurrentRunBlueprint[tilesetId].Count; ++index)
    {
      this.m_postprocessInjectionData.Add(MetaInjectionData.CurrentRunBlueprint[tilesetId][index]);
      if (MetaInjectionData.CurrentRunBlueprint[tilesetId][index].injectionData.name.Contains("Subshop"))
        this.m_runtimeInjectionFlags.ShopAnnexed = true;
    }
  }

  private bool IsCompositeWarpWing(LoopBuilderComposite composite)
  {
    for (int index = 0; index < composite.ExternalConnectedNodes.Count; ++index)
    {
      BuilderFlowNode externalConnectedNode = composite.ExternalConnectedNodes[index];
      BuilderFlowNode connectedInternalNode = composite.GetConnectedInternalNode(externalConnectedNode);
      if (connectedInternalNode != null && connectedInternalNode.node.isWarpWingEntrance)
        return true;
    }
    return false;
  }

  [DebuggerHidden]
  public IEnumerable DeferredBuild()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LoopFlowBuilder.<DeferredBuild>c__Iterator2 deferredBuildCIterator2 = new LoopFlowBuilder.<DeferredBuild>c__Iterator2()
    {
      $this = this
    };
    // ISSUE: reference to a compiler-generated field
    deferredBuildCIterator2.$PC = -2;
    return (IEnumerable) deferredBuildCIterator2;
  }

  private void SanityCheckRooms(SemioticLayoutManager layout)
  {
    for (int index1 = 0; index1 < layout.Rooms.Count; ++index1)
    {
      RoomHandler room = layout.Rooms[index1];
      if (!room.area.IsProceduralRoom)
      {
        bool flag = false;
        for (int index2 = 0; index2 < this.allBuilderNodes.Count; ++index2)
        {
          if (this.allBuilderNodes[index2].instanceRoom == room)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          layout.Rooms.RemoveAt(index1);
          --index1;
        }
      }
    }
  }

  public enum FallbackLevel
  {
    NOT_FALLBACK,
    FALLBACK_STANDARD,
    FALLBACK_EMERGENCY,
  }
}
