using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable

[Serializable]
public class DungeonFlowNode
  {
    public bool isSubchainStandin;
    public DungeonFlowNode.ControlNodeType nodeType;
    public PrototypeDungeonRoom.RoomCategory roomCategory;
    public float percentChance = 1f;
    public DungeonFlowNode.NodePriority priority;
    public PrototypeDungeonRoom overrideExactRoom;
    public GenericRoomTable overrideRoomTable;
    public bool capSubchain;
    public string subchainIdentifier;
    public bool limitedCopiesOfSubchain;
    public int maxCopiesOfSubchain = 1;
    public List<string> subchainIdentifiers;
    public bool receivesCaps;
    public bool isWarpWingEntrance;
    public bool handlesOwnWarping;
    public DungeonFlowNode.ForcedDoorType forcedDoorType;
    public DungeonFlowNode.ForcedDoorType loopForcedDoorType;
    public bool nodeExpands;
    public string initialChainPrototype = "n";
    public List<ChainRule> chainRules;
    public int minChainLength = 3;
    public int maxChainLength = 8;
    public int minChildrenToBuild = 1;
    public int maxChildrenToBuild = 1;
    public bool canBuildDuplicateChildren;
    public string parentNodeGuid;
    public List<string> childNodeGuids;
    public string loopTargetNodeGuid;
    public bool loopTargetIsOneWay;
    [HideInInspector]
    public string guidAsString;
    public DungeonFlow flow;

    public DungeonFlowNode(DungeonFlow parentFlow)
    {
      this.flow = parentFlow;
      this.childNodeGuids = new List<string>();
      this.guidAsString = Guid.NewGuid().ToString();
    }

    public bool UsesGlobalBossData
    {
      get
      {
        return GameManager.Instance.CurrentGameMode != GameManager.GameMode.BOSSRUSH && GameManager.Instance.CurrentGameMode != GameManager.GameMode.SUPERBOSSRUSH && (UnityEngine.Object) this.overrideExactRoom == (UnityEngine.Object) null && this.roomCategory == PrototypeDungeonRoom.RoomCategory.BOSS;
      }
    }

    public static bool operator ==(DungeonFlowNode a, DungeonFlowNode b)
    {
      if (object.ReferenceEquals((object) a, (object) b))
        return true;
      return (object) a != null && (object) b != null && a.guidAsString == b.guidAsString;
    }

    public static bool operator !=(DungeonFlowNode a, DungeonFlowNode b) => !(a == b);

    protected bool Equals(DungeonFlowNode other)
    {
      return string.Equals(this.guidAsString, other.guidAsString);
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      return obj.GetType() == this.GetType() && this.Equals((DungeonFlowNode) obj);
    }

    public override int GetHashCode()
    {
      return this.guidAsString != null ? this.guidAsString.GetHashCode() : 0;
    }

    public int GetAverageNumberNodes()
    {
      if (this.nodeExpands)
        return Mathf.Max(Mathf.FloorToInt((float) (this.minChainLength + this.maxChainLength) / 2f), 1);
      return this.nodeType == DungeonFlowNode.ControlNodeType.SELECTOR ? 0 : 1;
    }

    public string EvolveChainToCompletion()
    {
      int num = BraveRandom.GenerationRandomRange(this.minChainLength, this.maxChainLength + 1);
      string current = this.initialChainPrototype;
      while (current.Length < num)
      {
        int length = current.Length;
        current = this.EvolveChain(current);
        if (current.Length >= num)
        {
          bool flag = true;
          while (flag)
          {
            flag = false;
            string str = current;
            current = this.ApplyMandatoryRule(current);
            if (str != current)
              flag = true;
          }
        }
        if (current.Length == length)
          break;
      }
      return current;
    }

    private string ApplyMandatoryRule(string current)
    {
      List<ChainRule> source = new List<ChainRule>();
      for (int index = 0; index < this.chainRules.Count; ++index)
      {
        ChainRule chainRule = this.chainRules[index];
        if (chainRule.mandatory && current.Contains(chainRule.form))
          source.Add(chainRule);
      }
      if (source.Count == 0)
        return current;
      ChainRule chainRule1 = this.SelectRuleByWeighting(source);
      MatchCollection matchCollection = Regex.Matches(current, chainRule1.form);
      Match match = matchCollection[BraveRandom.GenerationRandomRange(0, matchCollection.Count)];
      string str1 = match.Index == 0 ? string.Empty : current.Substring(0, match.Index);
      string str2 = match.Index == current.Length - 1 ? string.Empty : current.Substring(match.Index + chainRule1.form.Length);
      return str1 + chainRule1.target + str2;
    }

    public string EvolveChain(string current)
    {
      List<ChainRule> source = new List<ChainRule>();
      for (int index = 0; index < this.chainRules.Count; ++index)
      {
        ChainRule chainRule = this.chainRules[index];
        if (current.Contains(chainRule.form))
          source.Add(chainRule);
      }
      if (source.Count == 0)
      {
        BraveUtility.Log("A DungeonChain has no associated rules. This works if no evolution is desired, but here's a warning just in case...", Color.yellow);
        return current;
      }
      ChainRule chainRule1 = this.SelectRuleByWeighting(source);
      MatchCollection matchCollection = Regex.Matches(current, chainRule1.form);
      Match match = matchCollection[BraveRandom.GenerationRandomRange(0, matchCollection.Count)];
      string str1 = match.Index == 0 ? string.Empty : current.Substring(0, match.Index);
      string str2 = match.Index == current.Length - 1 ? string.Empty : current.Substring(match.Index + chainRule1.form.Length);
      return str1 + chainRule1.target + str2;
    }

    private ChainRule SelectRuleByWeighting(List<ChainRule> source)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < source.Count; ++index)
        num2 += source[index].weight;
      float num3 = BraveRandom.GenerationRandomValue() * num2;
      for (int index = 0; index < source.Count; ++index)
      {
        num1 += source[index].weight;
        if ((double) num1 >= (double) num3)
          return source[index];
      }
      return (ChainRule) null;
    }

    public enum ControlNodeType
    {
      ROOM,
      SUBCHAIN,
      SELECTOR,
    }

    public enum NodePriority
    {
      MANDATORY,
      OPTIONAL,
    }

    public enum ForcedDoorType
    {
      NONE,
      LOCKED,
      ONE_WAY,
    }
  }

