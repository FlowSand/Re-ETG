// Decompiled with JetBrains decompiler
// Type: Dungeonator.ProceduralFlowModifierData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator;

[Serializable]
public class ProceduralFlowModifierData
{
  public string annotation;
  public bool DEBUG_FORCE_SPAWN;
  public bool OncePerRun;
  public List<ProceduralFlowModifierData.FlowModifierPlacementType> placementRules;
  public GenericRoomTable roomTable;
  public PrototypeDungeonRoom exactRoom;
  public bool IsWarpWing;
  public bool RequiresMasteryToken;
  public float chanceToLock;
  public float selectionWeight = 1f;
  public float chanceToSpawn = 1f;
  [SerializeField]
  public DungeonPlaceable RequiredValidPlaceable;
  public DungeonPrerequisite[] prerequisites;
  public bool CanBeForcedSecret = true;
  [Header("For Random Node Child")]
  public int RandomNodeChildMinDistanceFromEntrance;
  [Header("For Combat Frame")]
  public PrototypeDungeonRoom exactSecondaryRoom;
  public int framedCombatNodes;

  public bool PrerequisitesMet
  {
    get
    {
      for (int index = 0; index < this.prerequisites.Length; ++index)
      {
        if (!this.prerequisites[index].CheckConditionsFulfilled())
          return false;
      }
      return !this.RequiresMasteryToken || !GameManager.HasInstance || !(bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer || GameManager.Instance.PrimaryPlayer.MasteryTokensCollectedThisRun > 0;
    }
  }

  public ProceduralFlowModifierData.FlowModifierPlacementType GetPlacementRule()
  {
    return this.placementRules[BraveRandom.GenerationRandomRange(0, this.placementRules.Count)];
  }

  public enum FlowModifierPlacementType
  {
    BEFORE_ANY_COMBAT_ROOM,
    END_OF_CHAIN,
    HUB_ADJACENT_CHAIN_START,
    HUB_ADJACENT_NO_LINK,
    RANDOM_NODE_CHILD,
    COMBAT_FRAME,
    NO_LINKS,
    AFTER_BOSS,
    BLACK_MARKET,
  }
}
