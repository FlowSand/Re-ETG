// Decompiled with JetBrains decompiler
// Type: tk2dTileMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tk2dRuntime;
using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/TileMap/TileMap")]
public class tk2dTileMap : MonoBehaviour, ISpriteCollectionForceBuild
{
  public string editorDataGUID = string.Empty;
  public tk2dTileMapData data;
  public GameObject renderData;
  [SerializeField]
  private tk2dSpriteCollectionData spriteCollection;
  [SerializeField]
  private int spriteCollectionKey;
  public int width = 128 /*0x80*/;
  public int height = 128 /*0x80*/;
  public int partitionSizeX = 32 /*0x20*/;
  public int partitionSizeY = 32 /*0x20*/;
  public bool isGungeonTilemap = true;
  [SerializeField]
  private Layer[] layers;
  [SerializeField]
  private ColorChannel colorChannel;
  [SerializeField]
  private GameObject prefabsRoot;
  [SerializeField]
  private List<tk2dTileMap.TilemapPrefabInstance> tilePrefabsList = new List<tk2dTileMap.TilemapPrefabInstance>();
  [SerializeField]
  private bool _inEditMode;
  public string serializedMeshPath;

  public tk2dSpriteCollectionData Editor__SpriteCollection
  {
    get => this.spriteCollection;
    set => this.spriteCollection = value;
  }

  public tk2dSpriteCollectionData SpriteCollectionInst
  {
    get
    {
      return (UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null ? this.spriteCollection.inst : (tk2dSpriteCollectionData) null;
    }
  }

  public bool AllowEdit => this._inEditMode;

  private void Awake()
  {
    bool flag = true;
    if ((bool) (UnityEngine.Object) this.SpriteCollectionInst && (this.SpriteCollectionInst.buildKey != this.spriteCollectionKey || this.SpriteCollectionInst.needMaterialInstance))
      flag = false;
    if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
    {
      if (Application.isPlaying && this._inEditMode || !flag)
      {
        this.EndEditMode();
      }
      else
      {
        if (!((UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null) || !((UnityEngine.Object) this.data != (UnityEngine.Object) null) || !((UnityEngine.Object) this.renderData == (UnityEngine.Object) null))
          return;
        this.Build(tk2dTileMap.BuildFlags.ForceBuild);
      }
    }
    else if (this._inEditMode)
    {
      UnityEngine.Debug.LogError((object) $"Tilemap {this.name} is still in edit mode. Please fix.Building overhead will be significant.");
      this.EndEditMode();
    }
    else if (!flag)
    {
      this.Build(tk2dTileMap.BuildFlags.ForceBuild);
    }
    else
    {
      if (!((UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null) || !((UnityEngine.Object) this.data != (UnityEngine.Object) null) || !((UnityEngine.Object) this.renderData == (UnityEngine.Object) null))
        return;
      this.Build(tk2dTileMap.BuildFlags.ForceBuild);
    }
  }

  private void OnDestroy()
  {
    if (this.layers != null)
    {
      foreach (Layer layer in this.layers)
        layer.DestroyGameData(this);
    }
    if (!((UnityEngine.Object) this.renderData != (UnityEngine.Object) null))
      return;
    tk2dUtil.DestroyImmediate((UnityEngine.Object) this.renderData);
  }

  public void Build() => this.Build(tk2dTileMap.BuildFlags.Default);

  public void ForceBuild() => this.Build(tk2dTileMap.BuildFlags.ForceBuild);

  private void ClearSpawnedInstances()
  {
    if (this.layers == null)
      return;
    BuilderUtil.HideTileMapPrefabs(this);
    for (int index1 = 0; index1 < this.layers.Length; ++index1)
    {
      Layer layer = this.layers[index1];
      for (int index2 = 0; index2 < layer.spriteChannel.chunks.Length; ++index2)
      {
        SpriteChunk chunk = layer.spriteChannel.chunks[index2];
        if (!((UnityEngine.Object) chunk.gameObject == (UnityEngine.Object) null))
        {
          Transform transform = chunk.gameObject.transform;
          List<Transform> transformList = new List<Transform>();
          for (int index3 = 0; index3 < transform.childCount; ++index3)
            transformList.Add(transform.GetChild(index3));
          for (int index4 = 0; index4 < transformList.Count; ++index4)
            tk2dUtil.DestroyImmediate((UnityEngine.Object) transformList[index4].gameObject);
        }
      }
    }
  }

  private void SetPrefabsRootActive(bool active)
  {
    if (!((UnityEngine.Object) this.prefabsRoot != (UnityEngine.Object) null))
      return;
    tk2dUtil.SetActive(this.prefabsRoot, active);
  }

  public void Build(tk2dTileMap.BuildFlags buildFlags)
  {
    IEnumerator enumerator = this.DeferredBuild(buildFlags);
    do
      ;
    while (enumerator.MoveNext());
  }

  [DebuggerHidden]
  public IEnumerator DeferredBuild(tk2dTileMap.BuildFlags buildFlags)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new tk2dTileMap.\u003CDeferredBuild\u003Ec__Iterator0()
    {
      buildFlags = buildFlags,
      \u0024this = this
    };
  }

  public bool GetTileAtPosition(Vector3 position, out int x, out int y)
  {
    float x1;
    float y1;
    bool tileFracAtPosition = this.GetTileFracAtPosition(position, out x1, out y1);
    x = (int) x1;
    y = (int) y1;
    return tileFracAtPosition;
  }

  public bool GetTileFracAtPosition(Vector3 position, out float x, out float y)
  {
    switch (this.data.tileType)
    {
      case tk2dTileMapData.TileType.Rectangular:
        Vector3 vector3_1 = this.transform.worldToLocalMatrix.MultiplyPoint(position);
        x = (vector3_1.x - this.data.tileOrigin.x) / this.data.tileSize.x;
        y = (vector3_1.y - this.data.tileOrigin.y) / this.data.tileSize.y;
        return (double) x >= 0.0 && (double) x <= (double) this.width && (double) y >= 0.0 && (double) y <= (double) this.height;
      case tk2dTileMapData.TileType.Isometric:
        if ((double) this.data.tileSize.x != 0.0)
        {
          float num1 = Mathf.Atan2(this.data.tileSize.y, this.data.tileSize.x / 2f);
          Vector3 vector3_2 = this.transform.worldToLocalMatrix.MultiplyPoint(position);
          x = (vector3_2.x - this.data.tileOrigin.x) / this.data.tileSize.x;
          y = (vector3_2.y - this.data.tileOrigin.y) / this.data.tileSize.y;
          float num2 = y * 0.5f;
          int num3 = (int) num2;
          float y1 = num2 - (float) num3;
          float num4 = x % 1f;
          x = (float) (int) x;
          y = (float) (num3 * 2);
          if ((double) num4 > 0.5)
          {
            if ((double) y1 > 0.5 && (double) Mathf.Atan2(1f - y1, (float) (((double) num4 - 0.5) * 2.0)) < (double) num1)
              ++y;
            else if ((double) y1 < 0.5 && (double) Mathf.Atan2(y1, (float) (((double) num4 - 0.5) * 2.0)) < (double) num1)
              --y;
          }
          else if ((double) num4 < 0.5)
          {
            if ((double) y1 > 0.5 && (double) Mathf.Atan2(y1 - 0.5f, num4 * 2f) > (double) num1)
            {
              ++y;
              --x;
            }
            if ((double) y1 < 0.5 && (double) Mathf.Atan2(y1, (float) ((0.5 - (double) num4) * 2.0)) < (double) num1)
            {
              --y;
              --x;
            }
          }
          return (double) x >= 0.0 && (double) x <= (double) this.width && (double) y >= 0.0 && (double) y <= (double) this.height;
        }
        break;
    }
    x = 0.0f;
    y = 0.0f;
    return false;
  }

  public Vector3 GetTilePosition(int x, int y)
  {
    switch (this.data.tileType)
    {
      case tk2dTileMapData.TileType.Isometric:
        return this.transform.localToWorldMatrix.MultiplyPoint(new Vector3(((float) x + ((y & 1) != 0 ? 0.5f : 0.0f)) * this.data.tileSize.x + this.data.tileOrigin.x, (float) y * this.data.tileSize.y + this.data.tileOrigin.y, 0.0f));
      default:
        return this.transform.localToWorldMatrix.MultiplyPoint(new Vector3((float) x * this.data.tileSize.x + this.data.tileOrigin.x, (float) y * this.data.tileSize.y + this.data.tileOrigin.y, 0.0f));
    }
  }

  public int GetTileIdAtPosition(Vector3 position, int layer)
  {
    int x;
    int y;
    return layer < 0 || layer >= this.layers.Length || !this.GetTileAtPosition(position, out x, out y) ? -1 : this.layers[layer].GetTile(x, y);
  }

  public TileInfo GetTileInfoForTileId(int tileId) => this.data.GetTileInfoForSprite(tileId);

  public Color GetInterpolatedColorAtPosition(Vector3 position)
  {
    Vector3 vector3 = this.transform.worldToLocalMatrix.MultiplyPoint(position);
    int x = (int) (((double) vector3.x - (double) this.data.tileOrigin.x) / (double) this.data.tileSize.x);
    int y = (int) (((double) vector3.y - (double) this.data.tileOrigin.y) / (double) this.data.tileSize.y);
    if (this.colorChannel == null || this.colorChannel.IsEmpty)
      return Color.white;
    if (x < 0 || x >= this.width || y < 0 || y >= this.height)
      return this.colorChannel.clearColor;
    int offset;
    ColorChunk chunkAndCoordinate = this.colorChannel.FindChunkAndCoordinate(x, y, out offset);
    if (chunkAndCoordinate.Empty)
      return this.colorChannel.clearColor;
    int num1 = this.partitionSizeX + 1;
    Color color1 = (Color) chunkAndCoordinate.colors[offset];
    Color color2 = (Color) chunkAndCoordinate.colors[offset + 1];
    Color color3 = (Color) chunkAndCoordinate.colors[offset + num1];
    Color color4 = (Color) chunkAndCoordinate.colors[offset + num1 + 1];
    float num2 = (float) x * this.data.tileSize.x + this.data.tileOrigin.x;
    float num3 = (float) y * this.data.tileSize.y + this.data.tileOrigin.y;
    float t1 = (vector3.x - num2) / this.data.tileSize.x;
    float t2 = (vector3.y - num3) / this.data.tileSize.y;
    return Color.Lerp(Color.Lerp(color1, color2, t1), Color.Lerp(color3, color4, t1), t2);
  }

  public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
  {
    if (!((UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null))
      return false;
    return (UnityEngine.Object) spriteCollection == (UnityEngine.Object) this.spriteCollection || (UnityEngine.Object) spriteCollection == (UnityEngine.Object) this.spriteCollection.inst;
  }

  public void EndEditMode()
  {
    this._inEditMode = false;
    this.SetPrefabsRootActive(true);
    this.Build(tk2dTileMap.BuildFlags.ForceBuild);
    if (!((UnityEngine.Object) this.prefabsRoot != (UnityEngine.Object) null))
      return;
    tk2dUtil.DestroyImmediate((UnityEngine.Object) this.prefabsRoot);
    this.prefabsRoot = (GameObject) null;
  }

  public bool AreSpritesInitialized() => this.layers != null;

  public bool HasColorChannel() => this.colorChannel != null && !this.colorChannel.IsEmpty;

  public void CreateColorChannel()
  {
    this.colorChannel = new ColorChannel(this.width, this.height, this.partitionSizeX, this.partitionSizeY);
    this.colorChannel.Create();
  }

  public void DeleteColorChannel() => this.colorChannel.Delete();

  public void DeleteSprites(int layerId, int x0, int y0, int x1, int y1)
  {
    x0 = Mathf.Clamp(x0, 0, this.width - 1);
    y0 = Mathf.Clamp(y0, 0, this.height - 1);
    x1 = Mathf.Clamp(x1, 0, this.width - 1);
    y1 = Mathf.Clamp(y1, 0, this.height - 1);
    int num1 = x1 - x0 + 1;
    int num2 = y1 - y0 + 1;
    Layer layer = this.layers[layerId];
    for (int index1 = 0; index1 < num2; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
        layer.SetTile(x0 + index2, y0 + index1, -1);
    }
    layer.OptimizeIncremental();
  }

  public void TouchMesh(Mesh mesh)
  {
  }

  public void DestroyMesh(Mesh mesh) => tk2dUtil.DestroyImmediate((UnityEngine.Object) mesh);

  public int GetTilePrefabsListCount() => this.tilePrefabsList.Count;

  public List<tk2dTileMap.TilemapPrefabInstance> TilePrefabsList => this.tilePrefabsList;

  public void GetTilePrefabsListItem(
    int index,
    out int x,
    out int y,
    out int layer,
    out GameObject instance)
  {
    tk2dTileMap.TilemapPrefabInstance tilePrefabs = this.tilePrefabsList[index];
    x = tilePrefabs.x;
    y = tilePrefabs.y;
    layer = tilePrefabs.layer;
    instance = tilePrefabs.instance;
  }

  public void SetTilePrefabsList(
    List<int> xs,
    List<int> ys,
    List<int> layers,
    List<GameObject> instances)
  {
    int count = instances.Count;
    this.tilePrefabsList = new List<tk2dTileMap.TilemapPrefabInstance>(count);
    for (int index = 0; index < count; ++index)
      this.tilePrefabsList.Add(new tk2dTileMap.TilemapPrefabInstance()
      {
        x = xs[index],
        y = ys[index],
        layer = layers[index],
        instance = instances[index]
      });
  }

  public Layer[] Layers
  {
    get => this.layers;
    set => this.layers = value;
  }

  public ColorChannel ColorChannel
  {
    get => this.colorChannel;
    set => this.colorChannel = value;
  }

  public GameObject PrefabsRoot
  {
    get => this.prefabsRoot;
    set => this.prefabsRoot = value;
  }

  public int GetTile(int x, int y, int layer)
  {
    return layer < 0 || layer >= this.layers.Length ? -1 : this.layers[layer].GetTile(x, y);
  }

  public tk2dTileFlags GetTileFlags(int x, int y, int layer)
  {
    return layer < 0 || layer >= this.layers.Length ? tk2dTileFlags.None : this.layers[layer].GetTileFlags(x, y);
  }

  public void SetTile(int x, int y, int layer, int tile)
  {
    if (layer < 0 || layer >= this.layers.Length)
      return;
    this.layers[layer].SetTile(x, y, tile);
  }

  public void SetTileFlags(int x, int y, int layer, tk2dTileFlags flags)
  {
    if (layer < 0 || layer >= this.layers.Length)
      return;
    this.layers[layer].SetTileFlags(x, y, flags);
  }

  public void ClearTile(int x, int y, int layer)
  {
    if (layer < 0 || layer >= this.layers.Length)
      return;
    this.layers[layer].ClearTile(x, y);
  }

  [Serializable]
  public class TilemapPrefabInstance
  {
    public int x;
    public int y;
    public int layer;
    public GameObject instance;
  }

  [Flags]
  public enum BuildFlags
  {
    Default = 0,
    EditMode = 1,
    ForceBuild = 2,
  }
}
