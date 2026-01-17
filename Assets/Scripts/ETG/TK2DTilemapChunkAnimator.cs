// Decompiled with JetBrains decompiler
// Type: TK2DTilemapChunkAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TK2DTilemapChunkAnimator : MonoBehaviour
{
  public static Dictionary<IntVector2, List<TilemapAnimatorTileManager>> PositionToAnimatorMap = new Dictionary<IntVector2, List<TilemapAnimatorTileManager>>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
  private List<TilemapAnimatorTileManager> m_tiles;
  private Mesh m_refMesh;
  private tk2dTileMap m_refTilemap;
  private Vector2[] m_currentUVs;

  public void Initialize(
    List<TilemapAnimatorTileManager> tiles,
    Mesh refMesh,
    tk2dTileMap refTilemap)
  {
    this.m_tiles = tiles;
    for (int index = 0; index < this.m_tiles.Count; ++index)
      this.m_tiles[index].animator = this;
    this.m_refMesh = refMesh;
    this.m_refTilemap = refTilemap;
    this.m_currentUVs = this.m_refMesh.uv;
  }

  private void Update()
  {
    bool flag = false;
    for (int index = 0; index < this.m_tiles.Count; ++index)
    {
      if (this.m_tiles[index].UpdateRelevantSection(ref this.m_currentUVs, this.m_refMesh, this.m_refTilemap, BraveTime.DeltaTime))
        flag = true;
    }
    if (!flag)
      return;
    this.m_refMesh.uv = this.m_currentUVs;
  }
}
