// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.TileMap.LayerInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap;

[Serializable]
public class LayerInfo
{
  public string name;
  public int hash;
  public bool useColor;
  public bool generateCollider;
  public float z = 0.1f;
  public int unityLayer;
  public int renderQueueOffset;
  public string sortingLayerName = string.Empty;
  public int sortingOrder;
  [NonSerialized]
  public bool ForceNonAnimating;
  public bool overrideChunkable;
  public int overrideChunkXOffset;
  public int overrideChunkYOffset;
  [NonSerialized]
  public bool[] preprocessedFlags;
  public bool skipMeshGeneration;
  public PhysicMaterial physicMaterial;
  public PhysicsMaterial2D physicsMaterial2D;

  public LayerInfo()
  {
    this.unityLayer = 0;
    this.useColor = true;
    this.generateCollider = true;
    this.skipMeshGeneration = false;
  }
}
