// Decompiled with JetBrains decompiler
// Type: Dungeonator.SemioticDungeonGenSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class SemioticDungeonGenSettings
  {
    [SerializeField]
    public List<DungeonFlow> flows;
    [SerializeField]
    public List<ExtraIncludedRoomData> mandatoryExtraRooms;
    [SerializeField]
    public List<ExtraIncludedRoomData> optionalExtraRooms;
    [SerializeField]
    public int MAX_GENERATION_ATTEMPTS = 25;
    [SerializeField]
    public bool DEBUG_RENDER_CANVASES_SEPARATELY;

    public DungeonFlow GetRandomFlow()
    {
      if ((UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON && !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_MET_PREVIOUSLY))
        return this.flows[0];
      float num1 = 0.0f;
      List<DungeonFlow> dungeonFlowList1 = new List<DungeonFlow>();
      float num2 = 0.0f;
      List<DungeonFlow> dungeonFlowList2 = new List<DungeonFlow>();
      for (int index = 0; index < this.flows.Count; ++index)
      {
        if (GameStatsManager.Instance.QueryFlowDifferentiator(this.flows[index].name) > 0)
        {
          ++num1;
          dungeonFlowList1.Add(this.flows[index]);
        }
        else
        {
          ++num2;
          dungeonFlowList2.Add(this.flows[index]);
        }
      }
      if (dungeonFlowList2.Count <= 0 && dungeonFlowList1.Count > 0)
      {
        dungeonFlowList2 = dungeonFlowList1;
        num2 = num1;
      }
      if (dungeonFlowList2.Count <= 0)
        return (DungeonFlow) null;
      float num3 = BraveRandom.GenerationRandomValue() * num2;
      float num4 = 0.0f;
      for (int index = 0; index < dungeonFlowList2.Count; ++index)
      {
        ++num4;
        if ((double) num4 >= (double) num3)
          return dungeonFlowList2[index];
      }
      return this.flows[BraveRandom.GenerationRandomRange(0, this.flows.Count)];
    }
  }
}
