using System;
using System.Collections.Generic;
using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable

public class tk2dTileMapData : ScriptableObject
  {
    public Vector3 tileSize;
    public Vector3 tileOrigin;
    public tk2dTileMapData.TileType tileType;
    public tk2dTileMapData.SortMethod sortMethod;
    public bool layersFixedZ;
    public bool useSortingLayers;
    public GameObject[] tilePrefabs = new GameObject[0];
    [SerializeField]
    private TileInfo[] tileInfo = new TileInfo[0];
    [SerializeField]
    public List<LayerInfo> tileMapLayers = new List<LayerInfo>();

    public int NumLayers
    {
      get
      {
        if (this.tileMapLayers == null || this.tileMapLayers.Count == 0)
          this.InitLayers();
        return this.tileMapLayers.Count;
      }
    }

    public LayerInfo[] Layers
    {
      get
      {
        if (this.tileMapLayers == null || this.tileMapLayers.Count == 0)
          this.InitLayers();
        return this.tileMapLayers.ToArray();
      }
    }

    public TileInfo GetTileInfoForSprite(int tileId)
    {
      return this.tileInfo == null || tileId < 0 || tileId >= this.tileInfo.Length ? (TileInfo) null : this.tileInfo[tileId];
    }

    public TileInfo[] GetOrCreateTileInfo(int numTiles)
    {
      bool flag = false;
      if (this.tileInfo == null)
      {
        this.tileInfo = new TileInfo[numTiles];
        flag = true;
      }
      else if (this.tileInfo.Length != numTiles)
      {
        Array.Resize<TileInfo>(ref this.tileInfo, numTiles);
        flag = true;
      }
      if (flag)
      {
        for (int index = 0; index < this.tileInfo.Length; ++index)
        {
          if (this.tileInfo[index] == null)
            this.tileInfo[index] = new TileInfo();
        }
      }
      return this.tileInfo;
    }

    public void GetTileOffset(out float x, out float y)
    {
      switch (this.tileType)
      {
        case tk2dTileMapData.TileType.Isometric:
          x = 0.5f;
          y = 0.0f;
          break;
        default:
          x = 0.0f;
          y = 0.0f;
          break;
      }
    }

    private void InitLayers()
    {
      this.tileMapLayers = new List<LayerInfo>();
      LayerInfo layerInfo = new LayerInfo();
      this.tileMapLayers.Add(new LayerInfo()
      {
        name = "Layer 0",
        hash = 1892887448,
        z = 0.0f
      });
    }

    public enum SortMethod
    {
      BottomLeft,
      TopLeft,
      BottomRight,
      TopRight,
    }

    public enum TileType
    {
      Rectangular,
      Isometric,
    }
  }

