// Decompiled with JetBrains decompiler
// Type: tk2dSpriteCollectionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using tk2dRuntime;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollectionData")]
public class tk2dSpriteCollectionData : MonoBehaviour
  {
    public const int CURRENT_VERSION = 3;
    public int version;
    public bool materialIdsValid;
    public bool needMaterialInstance;
    public tk2dSpriteDefinition[] spriteDefinitions;
    [SerializeField]
    public List<int> SpriteIDsWithBagelColliders = new List<int>();
    [SerializeField]
    public List<BagelColliderData> SpriteDefinedBagelColliders = new List<BagelColliderData>();
    [SerializeField]
    public List<int> SpriteIDsWithAttachPoints = new List<int>();
    [SerializeField]
    public List<AttachPointData> SpriteDefinedAttachPoints = new List<AttachPointData>();
    [SerializeField]
    public List<int> SpriteIDsWithNeighborDependencies = new List<int>();
    [SerializeField]
    public List<NeighborDependencyData> SpriteDefinedIndexNeighborDependencies = new List<NeighborDependencyData>();
    [SerializeField]
    public List<int> SpriteIDsWithAnimationSequences = new List<int>();
    [SerializeField]
    public List<SimpleTilesetAnimationSequence> SpriteDefinedAnimationSequences = new List<SimpleTilesetAnimationSequence>();
    private Dictionary<string, int> spriteNameLookupDict;
    public bool premultipliedAlpha;
    public bool shouldGenerateTilemapReflectionData;
    public Material material;
    public Material[] materials;
    [NonSerialized]
    public Material[] materialInsts;
    [NonSerialized]
    public Texture2D[] textureInsts = new Texture2D[0];
    public Texture[] textures;
    public TextAsset[] pngTextures = new TextAsset[0];
    public int[] materialPngTextureId = new int[0];
    public FilterMode textureFilterMode = FilterMode.Bilinear;
    public bool textureMipMaps;
    public bool allowMultipleAtlases;
    public string spriteCollectionGUID;
    public string spriteCollectionName;
    public string assetName = string.Empty;
    public bool loadable;
    public float invOrthoSize = 1f;
    public float halfTargetHeight = 1f;
    public int buildKey;
    public string dataGuid = string.Empty;
    public bool managedSpriteCollection;
    public bool hasPlatformData;
    public string[] spriteCollectionPlatforms;
    public string[] spriteCollectionPlatformGUIDs;
    private tk2dSpriteCollectionData platformSpecificData;

    public bool Transient { get; set; }

    public BagelCollider[] GetBagelColliders(int spriteId)
    {
      int index = this.SpriteIDsWithBagelColliders.IndexOf(spriteId);
      return index >= 0 ? this.SpriteDefinedBagelColliders[index].bagelColliders : (BagelCollider[]) null;
    }

    public void SetBagelColliders(int spriteId, BagelCollider[] bcs)
    {
      if (bcs == null || bcs.Length == 0)
      {
        int index = this.SpriteIDsWithBagelColliders.IndexOf(spriteId);
        if (index < 0)
          return;
        this.SpriteIDsWithBagelColliders.RemoveAt(index);
        this.SpriteDefinedBagelColliders.RemoveAt(index);
      }
      else if (this.SpriteIDsWithBagelColliders.Contains(spriteId))
      {
        this.SpriteDefinedBagelColliders[this.SpriteIDsWithBagelColliders.IndexOf(spriteId)] = new BagelColliderData(bcs);
      }
      else
      {
        this.SpriteIDsWithBagelColliders.Add(spriteId);
        this.SpriteDefinedBagelColliders.Add(new BagelColliderData(bcs));
      }
    }

    public tk2dSpriteDefinition.AttachPoint[] GetAttachPoints(int spriteId)
    {
      int index = this.SpriteIDsWithAttachPoints.IndexOf(spriteId);
      return index >= 0 ? this.SpriteDefinedAttachPoints[index].attachPoints : (tk2dSpriteDefinition.AttachPoint[]) null;
    }

    public void ClearAttachPoints(int spriteId)
    {
      int index = this.SpriteIDsWithAttachPoints.IndexOf(spriteId);
      if (index < 0)
        return;
      this.SpriteIDsWithAttachPoints.RemoveAt(index);
      this.SpriteDefinedAttachPoints.RemoveAt(index);
    }

    public void SetAttachPoints(int spriteId, tk2dSpriteDefinition.AttachPoint[] aps)
    {
      if (aps == null || aps.Length == 0)
        this.ClearAttachPoints(spriteId);
      else if (this.SpriteIDsWithAttachPoints.Contains(spriteId))
      {
        this.SpriteDefinedAttachPoints[this.SpriteIDsWithAttachPoints.IndexOf(spriteId)] = new AttachPointData(aps);
      }
      else
      {
        this.SpriteIDsWithAttachPoints.Add(spriteId);
        this.SpriteDefinedAttachPoints.Add(new AttachPointData(aps));
      }
    }

    public void ClearDependencies(int spriteId)
    {
      int index = this.SpriteIDsWithNeighborDependencies.IndexOf(spriteId);
      if (index < 0)
        return;
      this.SpriteIDsWithNeighborDependencies.RemoveAt(index);
      this.SpriteDefinedIndexNeighborDependencies.RemoveAt(index);
    }

    public List<IndexNeighborDependency> NewDependencies(int spriteId)
    {
      List<IndexNeighborDependency> dependencies = this.GetDependencies(spriteId);
      if (dependencies != null)
        return dependencies;
      List<IndexNeighborDependency> bcs = new List<IndexNeighborDependency>();
      this.SpriteIDsWithNeighborDependencies.Add(spriteId);
      this.SpriteDefinedIndexNeighborDependencies.Add(new NeighborDependencyData(bcs));
      return bcs;
    }

    public List<IndexNeighborDependency> GetDependencies(int spriteId)
    {
      int index = this.SpriteIDsWithNeighborDependencies.IndexOf(spriteId);
      return index >= 0 ? this.SpriteDefinedIndexNeighborDependencies[index].neighborDependencies : (List<IndexNeighborDependency>) null;
    }

    public SimpleTilesetAnimationSequence NewAnimationSequence(int spriteId)
    {
      SimpleTilesetAnimationSequence animationSequence1 = this.GetAnimationSequence(spriteId);
      if (animationSequence1 != null)
        return animationSequence1;
      SimpleTilesetAnimationSequence animationSequence2 = new SimpleTilesetAnimationSequence();
      this.SpriteIDsWithAnimationSequences.Add(spriteId);
      this.SpriteDefinedAnimationSequences.Add(animationSequence2);
      return animationSequence2;
    }

    public SimpleTilesetAnimationSequence GetAnimationSequence(int spriteId)
    {
      int index = this.SpriteIDsWithAnimationSequences.IndexOf(spriteId);
      return index >= 0 ? this.SpriteDefinedAnimationSequences[index] : (SimpleTilesetAnimationSequence) null;
    }

    public int Count => this.inst.spriteDefinitions.Length;

    public int GetSpriteIdByName(string name) => this.GetSpriteIdByName(name, 0);

    public int GetSpriteIdByName(string name, int defaultValue)
    {
      this.inst.InitDictionary();
      int num = defaultValue;
      return !this.inst.spriteNameLookupDict.TryGetValue(name, out num) ? defaultValue : num;
    }

    public tk2dSpriteDefinition GetSpriteDefinition(string name)
    {
      int spriteIdByName = this.GetSpriteIdByName(name, -1);
      return spriteIdByName == -1 ? (tk2dSpriteDefinition) null : this.spriteDefinitions[spriteIdByName];
    }

    public void InitDictionary()
    {
      if (this.spriteNameLookupDict != null)
        return;
      this.spriteNameLookupDict = new Dictionary<string, int>(this.spriteDefinitions.Length);
      for (int index = 0; index < this.spriteDefinitions.Length; ++index)
        this.spriteNameLookupDict[this.spriteDefinitions[index].name] = index;
    }

    public tk2dSpriteDefinition FirstValidDefinition
    {
      get
      {
        foreach (tk2dSpriteDefinition spriteDefinition in this.inst.spriteDefinitions)
        {
          if (spriteDefinition.Valid)
            return spriteDefinition;
        }
        return (tk2dSpriteDefinition) null;
      }
    }

    public bool IsValidSpriteId(int id)
    {
      return id >= 0 && id < this.inst.spriteDefinitions.Length && this.inst.spriteDefinitions[id].Valid;
    }

    public int FirstValidDefinitionIndex
    {
      get
      {
        tk2dSpriteCollectionData inst = this.inst;
        for (int validDefinitionIndex = 0; validDefinitionIndex < inst.spriteDefinitions.Length; ++validDefinitionIndex)
        {
          if (inst.spriteDefinitions[validDefinitionIndex].Valid)
            return validDefinitionIndex;
        }
        return -1;
      }
    }

    public void InitMaterialIds()
    {
      if (this.inst.materialIdsValid)
        return;
      int num = -1;
      Dictionary<Material, int> dictionary = new Dictionary<Material, int>();
      for (int index = 0; index < this.inst.materials.Length; ++index)
      {
        if (num == -1 && (UnityEngine.Object) this.inst.materials[index] != (UnityEngine.Object) null)
          num = index;
        dictionary[this.materials[index]] = index;
      }
      if (num == -1)
      {
        Debug.LogError((object) "Init material ids failed.");
      }
      else
      {
        foreach (tk2dSpriteDefinition spriteDefinition in this.inst.spriteDefinitions)
        {
          if (!dictionary.TryGetValue(spriteDefinition.material, out spriteDefinition.materialId))
            spriteDefinition.materialId = num;
        }
        this.inst.materialIdsValid = true;
      }
    }

    public List<Tuple<int, TilesetIndexMetadata>> GetIndicesForTileType(
      TilesetIndexMetadata.TilesetFlagType flagType)
    {
      if (this.spriteDefinitions == null)
        return (List<Tuple<int, TilesetIndexMetadata>>) null;
      List<Tuple<int, TilesetIndexMetadata>> tupleList = new List<Tuple<int, TilesetIndexMetadata>>();
      for (int first = 0; first < this.spriteDefinitions.Length; ++first)
      {
        if (this.spriteDefinitions[first].metadata != null && (this.spriteDefinitions[first].metadata.type & flagType) != (TilesetIndexMetadata.TilesetFlagType) 0)
        {
          Tuple<int, TilesetIndexMetadata> tuple = new Tuple<int, TilesetIndexMetadata>(first, this.spriteDefinitions[first].metadata);
          tupleList.Add(tuple);
        }
      }
      return tupleList.Count == 0 ? (List<Tuple<int, TilesetIndexMetadata>>) null : tupleList;
    }

    public tk2dSpriteCollectionData inst
    {
      get
      {
        if ((UnityEngine.Object) this.platformSpecificData == (UnityEngine.Object) null)
        {
          if (this.hasPlatformData)
          {
            string currentPlatform = tk2dSystem.CurrentPlatform;
            string guid = string.Empty;
            for (int index = 0; index < this.spriteCollectionPlatforms.Length; ++index)
            {
              if (this.spriteCollectionPlatforms[index] == currentPlatform)
              {
                guid = this.spriteCollectionPlatformGUIDs[index];
                break;
              }
            }
            if (guid.Length == 0)
              guid = this.spriteCollectionPlatformGUIDs[0];
            this.platformSpecificData = tk2dSystem.LoadResourceByGUID<tk2dSpriteCollectionData>(guid);
          }
          else
            this.platformSpecificData = this;
        }
        this.platformSpecificData.Init();
        return this.platformSpecificData;
      }
    }

    private void Init()
    {
      if (this.materialInsts != null)
        return;
      if (this.spriteDefinitions == null)
        this.spriteDefinitions = new tk2dSpriteDefinition[0];
      if (this.materials == null)
        this.materials = new Material[0];
      this.materialInsts = new Material[this.materials.Length];
      if (this.needMaterialInstance)
      {
        if (tk2dSystem.OverrideBuildMaterial)
        {
          for (int index = 0; index < this.materials.Length; ++index)
            this.materialInsts[index] = new Material(Shader.Find("tk2d/BlendVertexColor"));
        }
        else
        {
          bool flag = false;
          if (this.pngTextures.Length > 0)
          {
            flag = true;
            this.textureInsts = new Texture2D[this.pngTextures.Length];
            for (int index = 0; index < this.pngTextures.Length; ++index)
            {
              Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, this.textureMipMaps);
              tex.LoadImage(this.pngTextures[index].bytes);
              this.textureInsts[index] = tex;
              tex.filterMode = this.textureFilterMode;
            }
          }
          for (int index1 = 0; index1 < this.materials.Length; ++index1)
          {
            this.materialInsts[index1] = UnityEngine.Object.Instantiate<Material>(this.materials[index1]);
            if (flag)
            {
              int index2 = this.materialPngTextureId.Length != 0 ? this.materialPngTextureId[index1] : 0;
              this.materialInsts[index1].mainTexture = (Texture) this.textureInsts[index2];
            }
          }
        }
        for (int index = 0; index < this.spriteDefinitions.Length; ++index)
        {
          tk2dSpriteDefinition spriteDefinition = this.spriteDefinitions[index];
          spriteDefinition.materialInst = this.materialInsts[spriteDefinition.materialId];
        }
      }
      else
      {
        for (int index = 0; index < this.materials.Length; ++index)
          this.materialInsts[index] = this.materials[index];
        for (int index = 0; index < this.spriteDefinitions.Length; ++index)
        {
          tk2dSpriteDefinition spriteDefinition = this.spriteDefinitions[index];
          spriteDefinition.materialInst = spriteDefinition.material;
        }
      }
      tk2dEditorSpriteDataUnloader.Register(this);
    }

    public static tk2dSpriteCollectionData CreateFromTexture(
      Texture texture,
      tk2dSpriteCollectionSize size,
      string[] names,
      Rect[] regions,
      Vector2[] anchors)
    {
      return SpriteCollectionGenerator.CreateFromTexture(texture, size, names, regions, anchors);
    }

    public static tk2dSpriteCollectionData CreateFromTexturePacker(
      tk2dSpriteCollectionSize size,
      string texturePackerData,
      Texture texture)
    {
      return SpriteCollectionGenerator.CreateFromTexturePacker(size, texturePackerData, texture);
    }

    public void ResetPlatformData()
    {
      tk2dEditorSpriteDataUnloader.Unregister(this);
      if ((UnityEngine.Object) this.platformSpecificData != (UnityEngine.Object) null)
        this.platformSpecificData.DestroyTextureInsts();
      this.DestroyTextureInsts();
      if ((bool) (UnityEngine.Object) this.platformSpecificData)
        this.platformSpecificData = (tk2dSpriteCollectionData) null;
      this.materialInsts = (Material[]) null;
    }

    private void DestroyTextureInsts()
    {
      foreach (UnityEngine.Object textureInst in this.textureInsts)
        UnityEngine.Object.DestroyImmediate(textureInst);
      this.textureInsts = new Texture2D[0];
    }

    public void UnloadTextures()
    {
      tk2dSpriteCollectionData inst = this.inst;
      foreach (UnityEngine.Object texture in inst.textures)
        Resources.UnloadAsset(texture);
      inst.DestroyTextureInsts();
    }

    private void OnDestroy()
    {
      if (this.Transient)
      {
        foreach (UnityEngine.Object material in this.materials)
          UnityEngine.Object.DestroyImmediate(material);
      }
      else if (this.needMaterialInstance)
      {
        foreach (UnityEngine.Object materialInst in this.materialInsts)
          UnityEngine.Object.DestroyImmediate(materialInst);
        this.materialInsts = new Material[0];
        foreach (UnityEngine.Object textureInst in this.textureInsts)
          UnityEngine.Object.DestroyImmediate(textureInst);
        this.textureInsts = new Texture2D[0];
      }
      this.ResetPlatformData();
    }
  }

