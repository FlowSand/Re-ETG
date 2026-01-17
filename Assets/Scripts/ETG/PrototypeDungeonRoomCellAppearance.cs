// Decompiled with JetBrains decompiler
// Type: PrototypeDungeonRoomCellAppearance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[Serializable]
public class PrototypeDungeonRoomCellAppearance
{
  [SerializeField]
  public int overrideDungeonMaterialIndex = -1;
  [SerializeField]
  public bool IsPhantomCarpet;
  [SerializeField]
  public bool ForceDisallowGoop;
  [SerializeField]
  public CellVisualData.CellFloorType OverrideFloorType;
  [SerializeField]
  public PrototypeIndexOverrideData globalOverrideIndices;
  [SerializeField]
  private List<PrototypeIndexOverrideData> m_overrideIndices;

  public PrototypeDungeonRoomCellAppearance()
  {
    this.globalOverrideIndices = new PrototypeIndexOverrideData();
    this.m_overrideIndices = new List<PrototypeIndexOverrideData>();
  }

  public bool HasChanges()
  {
    return this.overrideDungeonMaterialIndex != -1 || this.IsPhantomCarpet || this.ForceDisallowGoop || this.OverrideFloorType != CellVisualData.CellFloorType.Stone || this.globalOverrideIndices.indices.Count != 0 || this.m_overrideIndices.Count != 0;
  }

  public bool HasAnyOverride()
  {
    for (int index = 0; index < this.m_overrideIndices.Count; ++index)
    {
      if (this.m_overrideIndices[index].indices != null && this.m_overrideIndices[index].indices.Count != 0)
        return true;
    }
    return false;
  }

  public List<int> GetOverridesForTilemap(
    PrototypeDungeonRoom sourceRoom,
    GlobalDungeonData.ValidTilesets tileset)
  {
    if ((sourceRoom.overriddenTilesets & tileset) == tileset)
    {
      int index = Mathf.RoundToInt(Mathf.Log((float) tileset, 2f));
      while (index >= this.m_overrideIndices.Count)
        this.m_overrideIndices.Add(new PrototypeIndexOverrideData());
      if (this.m_overrideIndices[index].indices.Count > 0)
        return this.m_overrideIndices[index].indices;
      if (this.globalOverrideIndices.indices.Count > 0)
        return this.globalOverrideIndices.indices;
    }
    return (List<int>) null;
  }

  public void SetAllOverridesForTilemap(
    GlobalDungeonData.ValidTilesets tileset,
    List<int> overrides)
  {
    int index = Mathf.RoundToInt(Mathf.Log((float) tileset, 2f));
    PrototypeIndexOverrideData indexOverrideData = new PrototypeIndexOverrideData();
    indexOverrideData.indices = overrides;
    if (index < this.m_overrideIndices.Count)
    {
      this.m_overrideIndices[index] = indexOverrideData;
    }
    else
    {
      while (index != this.m_overrideIndices.Count)
        this.m_overrideIndices.Add(new PrototypeIndexOverrideData());
      this.m_overrideIndices.Add(indexOverrideData);
    }
  }

  public void ClearOverrideForTilemap(GlobalDungeonData.ValidTilesets tileset)
  {
    int index = Mathf.RoundToInt(Mathf.Log((float) tileset, 2f));
    if (index >= this.m_overrideIndices.Count)
      return;
    this.m_overrideIndices[index] = new PrototypeIndexOverrideData();
  }

  public void ClearAllOverrideData()
  {
    this.m_overrideIndices.Clear();
    this.globalOverrideIndices.indices.Clear();
  }

  public void MirrorData(PrototypeDungeonRoomCellAppearance source)
  {
    this.overrideDungeonMaterialIndex = source.overrideDungeonMaterialIndex;
    this.IsPhantomCarpet = source.IsPhantomCarpet;
    this.ForceDisallowGoop = source.ForceDisallowGoop;
    this.OverrideFloorType = source.OverrideFloorType;
    this.globalOverrideIndices = new PrototypeIndexOverrideData();
    this.globalOverrideIndices.indices = new List<int>();
    if (source.globalOverrideIndices.indices != null)
    {
      for (int index = 0; index < source.globalOverrideIndices.indices.Count; ++index)
        this.globalOverrideIndices.indices.Add(source.globalOverrideIndices.indices[index]);
    }
    this.m_overrideIndices = new List<PrototypeIndexOverrideData>();
    for (int index1 = 0; index1 < source.m_overrideIndices.Count; ++index1)
    {
      this.m_overrideIndices.Add(new PrototypeIndexOverrideData());
      this.m_overrideIndices[index1].indices = new List<int>();
      if (source.m_overrideIndices[index1].indices != null)
      {
        for (int index2 = 0; index2 < source.m_overrideIndices[index1].indices.Count; ++index2)
          this.m_overrideIndices[index1].indices.Add(source.m_overrideIndices[index1].indices[index2]);
      }
    }
  }
}
