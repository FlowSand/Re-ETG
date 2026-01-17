// Decompiled with JetBrains decompiler
// Type: DungeonChain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class DungeonChain : ScriptableObject
    {
      public string initialChainPrototype = "n";
      public List<ChainRule> chainRules;
      public int minChainLength = 3;
      public int maxChainLength = 8;
      public List<ChainRoom> mandatoryIncludedRooms;
      public List<ChainRoom> possiblyIncludedRooms;
      public List<ChainRoom> capRooms;

      public IntVector2 GetMandatoryDifficultyRating()
      {
        int x = 0;
        int y = 0;
        for (int index = 0; index < this.mandatoryIncludedRooms.Count; ++index)
        {
          if (!((Object) this.mandatoryIncludedRooms[index].prototypeRoom == (Object) null))
          {
            x += this.mandatoryIncludedRooms[index].prototypeRoom.MinDifficultyRating;
            y += this.mandatoryIncludedRooms[index].prototypeRoom.MaxDifficultyRating;
          }
        }
        return new IntVector2(x, y);
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
    }

}
