// Decompiled with JetBrains decompiler
// Type: DeadlyDeadlyGoopManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class DeadlyDeadlyGoopManager : MonoBehaviour
{
  public static bool DrawDebugLines = false;
  public static Dictionary<IntVector2, DeadlyDeadlyGoopManager> allGoopPositionMap = new Dictionary<IntVector2, DeadlyDeadlyGoopManager>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
  public static List<Tuple<Vector2, float>> m_goopExceptions = new List<Tuple<Vector2, float>>();
  private static bool m_DoGoopSpawnSplashes = true;
  public static float GOOP_GRID_SIZE = 0.25f;
  public GoopDefinition goopDefinition;
  public float goopDepth = 1.5f;
  private HashSet<IntVector2> m_goopedPositions = new HashSet<IntVector2>();
  private Dictionary<IntVector2, DeadlyDeadlyGoopManager.GoopPositionData> m_goopedCells = new Dictionary<IntVector2, DeadlyDeadlyGoopManager.GoopPositionData>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
  private Dictionary<int, Vector2> m_uvMap;
  private Dictionary<GameActor, float> m_containedActors = new Dictionary<GameActor, float>();
  private List<Vector2> m_centerUVOptions = new List<Vector2>();
  private bool[,] m_dirtyFlags;
  private bool[,] m_colorDirtyFlags;
  private MeshRenderer[,] m_mrs;
  private Mesh[,] m_meshes;
  private Vector3[] m_vertexArray;
  private Vector2[] m_uvArray;
  private Vector2[] m_uv2Array;
  private Color32[] m_colorArray;
  private int[] m_triangleArray;
  private List<IntVector2> m_removalPositions = new List<IntVector2>();
  private int CHUNK_SIZE = 5;
  private bool m_isPlayingFireAudio;
  private bool m_isPlayingAcidAudio;
  private Shader m_shader;
  private Texture2D m_texture;
  private Texture2D m_worldTexture;
  private static int MainTexPropertyID = -1;
  private static int WorldTexPropertyID = -1;
  private static int OpaquenessMultiplyPropertyID = -1;
  private static int BrightnessMultiplyPropertyID = -1;
  private static int TintColorPropertyID = -1;
  private uint m_lastElecSemaphore;
  private ParticleSystem m_fireSystem;
  private ParticleSystem m_fireIntroSystem;
  private ParticleSystem m_fireOutroSystem;
  private ParticleSystem m_elecSystem;
  private int m_currentUpdateBin;
  private CircularBuffer<float> m_deltaTimes = new CircularBuffer<float>(4);
  private const bool c_CULL_MESHES = true;
  private const int UPDATE_EVERY_N_FRAMES = 3;
  public Color ElecColor0 = new Color(1f, 1f, 1f, 1f);
  public Color ElecColor2 = new Color(1f, 1f, 10f, 1f);
  public float divFactor = 8.7f;
  public float tFactor = 4.2f;
  private GameObject m_genericSplashPrefab;
  private static BitArray2D m_pointsArray = new BitArray2D(true);
  private static IntVector2 s_goopPointCenter = new IntVector2(0, 0);
  private static float s_goopPointRadius;
  private static float s_goopPointRadiusSquare;

  public static void ClearPerLevelData()
  {
    StaticReferenceManager.AllGoops.Clear();
    DeadlyDeadlyGoopManager.allGoopPositionMap.Clear();
    DeadlyDeadlyGoopManager.m_goopExceptions.Clear();
  }

  public static void ReinitializeData()
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].ReinitializeArrays();
  }

  public static void ForceClearGoopsInCell(IntVector2 cellPos)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
    {
      DeadlyDeadlyGoopManager allGoop = StaticReferenceManager.AllGoops[index];
      IntVector2 intVector2 = (cellPos.ToVector2() / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2();
      for (int x = intVector2.x; (double) x < (double) intVector2.x + 1.0 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE; ++x)
      {
        for (int y = intVector2.y; (double) y < (double) intVector2.y + 1.0 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE; ++y)
          allGoop.RemoveGoopedPosition(new IntVector2(x, y));
      }
    }
  }

  public static int CountGoopsInRadius(Vector2 centerPosition, float radius)
  {
    int num = 0;
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
    {
      DeadlyDeadlyGoopManager allGoop = StaticReferenceManager.AllGoops[index];
      num += allGoop.CountGoopCircle(centerPosition, radius);
    }
    return num;
  }

  public static void DelayedClearGoopsInRadius(Vector2 centerPosition, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].RemoveGoopCircle(centerPosition, radius);
  }

  public static void FreezeGoopsCircle(Vector2 position, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].FreezeGoopCircle(position, radius);
  }

  public static void IgniteGoopsCircle(Vector2 position, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].IgniteGoopCircle(position, radius);
    for (int index = 0; index < StaticReferenceManager.AllGrasses.Count; ++index)
      StaticReferenceManager.AllGrasses[index].IgniteCircle(position, radius);
  }

  public static void IgniteGoopsLine(Vector2 p1, Vector2 p2, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].IgniteGoopLine(p1, p2, radius);
  }

  public void IgniteGoopLine(Vector2 p1, Vector2 p2, float radius)
  {
    float num1 = 0.0f;
    for (float num2 = Vector2.Distance(p2, p1); (double) num1 < (double) num2 + (double) radius; num1 += radius)
      this.IgniteGoopCircle(p1 + (p2 - p1).normalized * num1, radius);
  }

  public static void ElectrifyGoopsLine(Vector2 p1, Vector2 p2, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].ElectrifyGoopLine(p1, p2, radius);
  }

  public static void FreezeGoopsLine(Vector2 p1, Vector2 p2, float radius)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      StaticReferenceManager.AllGoops[index].FreezeGoopLine(p1, p2, radius);
  }

  public void ElectrifyGoopLine(Vector2 p1, Vector2 p2, float radius)
  {
    float num1 = 0.0f;
    for (float num2 = Vector2.Distance(p2, p1); (double) num1 < (double) num2 + (double) radius; num1 += radius)
      this.ElectrifyGoopCircle(p1 + (p2 - p1).normalized * num1, radius);
  }

  public void FreezeGoopLine(Vector2 p1, Vector2 p2, float radius)
  {
    float num1 = 0.0f;
    for (float num2 = Vector2.Distance(p2, p1); (double) num1 < (double) num2 + (double) radius; num1 += radius)
      this.FreezeGoopCircle(p1 + (p2 - p1).normalized * num1, radius);
  }

  public static DeadlyDeadlyGoopManager GetGoopManagerForGoopType(GoopDefinition goopDef)
  {
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
    {
      if ((bool) (Object) StaticReferenceManager.AllGoops[index] && (Object) StaticReferenceManager.AllGoops[index].goopDefinition == (Object) goopDef)
        return StaticReferenceManager.AllGoops[index];
    }
    DeadlyDeadlyGoopManager managerForGoopType = new GameObject("goop_" + goopDef.name).AddComponent<DeadlyDeadlyGoopManager>();
    managerForGoopType.SetTexture(goopDef.goopTexture, goopDef.worldTexture);
    managerForGoopType.goopDefinition = goopDef;
    managerForGoopType.InitialzeUV2IfNecessary();
    StaticReferenceManager.AllGoops.Add(managerForGoopType);
    managerForGoopType.InitializeParticleSystems();
    return managerForGoopType;
  }

  public static int RegisterUngoopableCircle(Vector2 center, float radius)
  {
    float second = radius * radius;
    DeadlyDeadlyGoopManager.m_goopExceptions.Add(Tuple.Create<Vector2, float>(center, second));
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
    {
      if ((bool) (Object) StaticReferenceManager.AllGoops[index])
        StaticReferenceManager.AllGoops[index].RemoveGoopCircle(center, radius);
    }
    return DeadlyDeadlyGoopManager.m_goopExceptions.Count - 1;
  }

  public static void UpdateUngoopableCircle(int id, Vector2 center, float radius)
  {
    if (id < 0 || id >= DeadlyDeadlyGoopManager.m_goopExceptions.Count)
      return;
    DeadlyDeadlyGoopManager.m_goopExceptions[id].First = center;
    DeadlyDeadlyGoopManager.m_goopExceptions[id].Second = radius * radius;
    for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
    {
      if ((bool) (Object) StaticReferenceManager.AllGoops[index])
        StaticReferenceManager.AllGoops[index].RemoveGoopCircle(center, radius);
    }
  }

  public static void DeregisterUngoopableCircle(int id)
  {
    if (DeadlyDeadlyGoopManager.m_goopExceptions == null || id >= DeadlyDeadlyGoopManager.m_goopExceptions.Count || id < 0)
      return;
    DeadlyDeadlyGoopManager.m_goopExceptions[id] = (Tuple<Vector2, float>) null;
  }

  private static void InitializePropertyIDs()
  {
    if (DeadlyDeadlyGoopManager.TintColorPropertyID != -1)
      return;
    DeadlyDeadlyGoopManager.TintColorPropertyID = Shader.PropertyToID("_TintColor");
    DeadlyDeadlyGoopManager.MainTexPropertyID = Shader.PropertyToID("_MainTex");
    DeadlyDeadlyGoopManager.WorldTexPropertyID = Shader.PropertyToID("_WorldTex");
    DeadlyDeadlyGoopManager.OpaquenessMultiplyPropertyID = Shader.PropertyToID("_OpaquenessMultiply");
    DeadlyDeadlyGoopManager.BrightnessMultiplyPropertyID = Shader.PropertyToID("_BrightnessMultiply");
  }

  public void SetTexture(Texture2D goopTexture, Texture2D worldTexture)
  {
    this.m_texture = goopTexture;
    this.m_worldTexture = worldTexture;
    for (int index1 = 0; index1 < this.m_mrs.GetLength(0); ++index1)
    {
      for (int index2 = 0; index2 < this.m_mrs.GetLength(1); ++index2)
      {
        if ((Object) this.m_mrs[index1, index2] != (Object) null && (bool) (Object) this.m_mrs[index1, index2])
        {
          this.m_mrs[index1, index2].material.SetTexture(DeadlyDeadlyGoopManager.MainTexPropertyID, (Texture) goopTexture);
          this.m_mrs[index1, index2].material.SetTexture(DeadlyDeadlyGoopManager.WorldTexPropertyID, (Texture) worldTexture);
        }
      }
    }
  }

  public void Awake()
  {
    DeadlyDeadlyGoopManager.InitializePropertyIDs();
    this.ConstructUVMap();
    int length1 = Mathf.RoundToInt((float) (4.0 * ((double) this.CHUNK_SIZE / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) * ((double) this.CHUNK_SIZE / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE)));
    this.m_vertexArray = new Vector3[length1];
    this.m_uvArray = new Vector2[length1];
    this.m_uv2Array = new Vector2[length1];
    for (int index = 0; index < length1; ++index)
      this.m_uv2Array[index] = Vector2.zero;
    this.m_colorArray = new Color32[length1];
    this.m_triangleArray = new int[length1 / 4 * 6];
    int length2 = Mathf.CeilToInt((float) GameManager.Instance.Dungeon.Width / (float) this.CHUNK_SIZE);
    int length3 = Mathf.CeilToInt((float) GameManager.Instance.Dungeon.Height / (float) this.CHUNK_SIZE);
    this.m_mrs = new MeshRenderer[length2, length3];
    this.m_meshes = new Mesh[length2, length3];
    this.m_dirtyFlags = new bool[length2, length3];
    this.m_colorDirtyFlags = new bool[length2, length3];
    this.m_shader = ShaderCache.Acquire("Brave/GoopShader");
  }

  public void ReinitializeArrays()
  {
    int rows = Mathf.CeilToInt((float) GameManager.Instance.Dungeon.Width / (float) this.CHUNK_SIZE);
    int cols = Mathf.CeilToInt((float) GameManager.Instance.Dungeon.Height / (float) this.CHUNK_SIZE);
    this.m_mrs = BraveUtility.MultidimensionalArrayResize<MeshRenderer>(this.m_mrs, rows, cols);
    this.m_meshes = BraveUtility.MultidimensionalArrayResize<Mesh>(this.m_meshes, rows, cols);
    this.m_dirtyFlags = BraveUtility.MultidimensionalArrayResize<bool>(this.m_dirtyFlags, rows, cols);
    this.m_colorDirtyFlags = BraveUtility.MultidimensionalArrayResize<bool>(this.m_colorDirtyFlags, rows, cols);
  }

  private Mesh GetChunkMesh(int chunkX, int chunkY)
  {
    if ((Object) this.m_meshes[chunkX, chunkY] != (Object) null)
      return this.m_meshes[chunkX, chunkY];
    GameObject gameObject = new GameObject($"goop_{this.goopDefinition.name}_chunk_{chunkX}_{chunkY}");
    gameObject.transform.position = new Vector3((float) (chunkX * this.CHUNK_SIZE), (float) (chunkY * this.CHUNK_SIZE), (float) (chunkY * this.CHUNK_SIZE) + this.goopDepth);
    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    Mesh chunkMesh = new Mesh();
    chunkMesh.MarkDynamic();
    meshFilter.mesh = chunkMesh;
    Material material = new Material(this.m_shader);
    if ((Object) this.m_texture != (Object) null)
    {
      material.SetTexture(DeadlyDeadlyGoopManager.MainTexPropertyID, (Texture) this.m_texture);
      material.SetTexture(DeadlyDeadlyGoopManager.WorldTexPropertyID, (Texture) this.m_worldTexture);
    }
    if (this.goopDefinition.isOily)
      material.SetFloat("_OilGoop", 1f);
    if (this.goopDefinition.usesOverrideOpaqueness)
      material.SetFloat(DeadlyDeadlyGoopManager.OpaquenessMultiplyPropertyID, this.goopDefinition.overrideOpaqueness);
    meshRenderer.material = material;
    this.m_mrs[chunkX, chunkY] = meshRenderer;
    this.m_meshes[chunkX, chunkY] = chunkMesh;
    int num1 = chunkX * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = num1 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = chunkY * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = num3 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    for (int index1 = num1; index1 < num2; ++index1)
    {
      for (int index2 = num3; index2 < num4; ++index2)
      {
        IntVector2 goopPos;
        goopPos.x = index1;
        goopPos.y = index2;
        this.InitMesh(goopPos, chunkX, chunkY);
      }
    }
    chunkMesh.vertices = this.m_vertexArray;
    chunkMesh.triangles = this.m_triangleArray;
    return chunkMesh;
  }

  private void ConstructUVMap()
  {
    this.m_uvMap = new Dictionary<int, Vector2>();
    this.m_uvMap.Add(62, new Vector2(0.0f, 0.0f));
    this.m_uvMap.Add(191, new Vector2(0.375f, 0.0f));
    this.m_uvMap.Add(254, new Vector2(0.375f, 0.0f));
    this.m_uvMap.Add(124, new Vector2(0.5f, 0.0f));
    this.m_uvMap.Add(31 /*0x1F*/, new Vector2(0.625f, 0.0f));
    this.m_uvMap.Add(241, new Vector2(0.75f, 0.0f));
    this.m_uvMap.Add(199, new Vector2(0.875f, 0.0f));
    this.m_uvMap.Add(14, new Vector2(0.0f, 0.125f));
    this.m_uvMap.Add(143, new Vector2(0.125f, 0.125f));
    this.m_uvMap.Add(238, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(239, new Vector2(0.375f, 0.0f));
    this.m_uvMap.Add(221, new Vector2(0.5f, 0.125f));
    this.m_uvMap.Add(119, new Vector2(0.625f, 0.125f));
    this.m_uvMap.Add(56, new Vector2(0.0f, 0.25f));
    this.m_uvMap.Add(187, new Vector2(0.125f, 0.25f));
    this.m_uvMap.Add(248, new Vector2(0.25f, 0.25f));
    this.m_uvMap.Add(251, new Vector2(0.375f, 0.0f));
    this.m_uvMap.Add(60, new Vector2(0.5f, 0.25f));
    this.m_uvMap.Add(30, new Vector2(0.625f, 0.25f));
    this.m_uvMap.Add(225, new Vector2(0.75f, 0.25f));
    this.m_uvMap.Add(195, new Vector2(0.875f, 0.25f));
    this.m_uvMap.Add(0, new Vector2(0.0f, 0.375f));
    this.m_uvMap.Add(131, new Vector2(0.125f, 0.375f));
    this.m_uvMap.Add(224 /*0xE0*/, new Vector2(0.25f, 0.375f));
    this.m_uvMap.Add(227, new Vector2(0.375f, 0.375f));
    this.m_uvMap.Add(126, new Vector2(0.5f, 0.375f));
    this.m_uvMap.Add(63 /*0x3F*/, new Vector2(0.625f, 0.375f));
    this.m_uvMap.Add(243, new Vector2(0.75f, 0.375f));
    this.m_uvMap.Add(231, new Vector2(0.875f, 0.375f));
    this.m_uvMap.Add(253, new Vector2(0.0f, 0.5f));
    this.m_uvMap.Add(223, new Vector2(0.125f, 0.5f));
    this.m_uvMap.Add((int) sbyte.MaxValue, new Vector2(0.25f, 0.5f));
    this.m_uvMap.Add(247, new Vector2(0.375f, 0.5f));
    this.m_uvMap.Add(249, new Vector2(0.5f, 0.5f));
    this.m_uvMap.Add(207, new Vector2(0.625f, 0.5f));
    this.m_uvMap.Add(252, new Vector2(0.75f, 0.5f));
    this.m_uvMap.Add(159, new Vector2(0.875f, 0.5f));
    this.m_uvMap.Add(68, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(102, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(204, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(17, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(51, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(153, new Vector2(0.25f, 0.125f));
    this.m_uvMap.Add(16 /*0x10*/, new Vector2(0.0f, 0.625f));
    this.m_uvMap.Add(4, new Vector2(0.125f, 0.625f));
    this.m_uvMap.Add(64 /*0x40*/, new Vector2(0.25f, 0.625f));
    this.m_uvMap.Add(1, new Vector2(0.375f, 0.625f));
    this.m_uvMap.Add(240 /*0xF0*/, new Vector2(0.5f, 0.625f));
    this.m_uvMap.Add(135, new Vector2(0.625f, 0.625f));
    this.m_uvMap.Add(120, new Vector2(0.75f, 0.625f));
    this.m_uvMap.Add(15, new Vector2(0.875f, 0.625f));
    this.m_uvMap.Add(-1, new Vector2(0.0f, 0.375f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.0f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.0f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.0f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.0f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.0f));
    this.m_centerUVOptions.Add(new Vector2(0.0f, 0.875f));
    this.m_centerUVOptions.Add(new Vector2(0.125f, 0.875f));
    this.m_centerUVOptions.Add(new Vector2(0.25f, 0.875f));
    this.m_centerUVOptions.Add(new Vector2(0.375f, 0.875f));
  }

  public void ProcessProjectile(Projectile p)
  {
    for (int index = 0; index < this.goopDefinition.goopDamageTypeInteractions.Count; ++index)
    {
      GoopDefinition.GoopDamageTypeInteraction damageTypeInteraction = this.goopDefinition.goopDamageTypeInteractions[index];
      bool flag = damageTypeInteraction.damageType == CoreDamageTypes.Ice && p.AppliesFreeze;
      if (((p.damageTypes & damageTypeInteraction.damageType) == damageTypeInteraction.damageType || flag) && this.IsPositionInGoop(p.specRigidbody.UnitCenter))
      {
        if (damageTypeInteraction.ignitionMode == GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.IGNITE)
          this.IgniteGoopCircle(p.specRigidbody.UnitCenter, 1f);
        else if (damageTypeInteraction.ignitionMode != GoopDefinition.GoopDamageTypeInteraction.GoopIgnitionMode.DOUSE)
          ;
        if (damageTypeInteraction.electrifiesGoop)
        {
          IntVector2 intVector2 = (p.specRigidbody.UnitCenter / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
          int num = (int) AkSoundEngine.PostEvent("Play_ENV_puddle_zap_01", GameManager.Instance.PrimaryPlayer.gameObject);
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleRecursiveElectrification(intVector2));
        }
        if (damageTypeInteraction.freezesGoop)
          this.FreezeGoopCircle(p.specRigidbody.UnitCenter, 1f);
      }
    }
  }

  private void ElectrifyCell(IntVector2 cellIndex)
  {
    if (!this.goopDefinition.CanBeElectrified || !this.m_goopedCells.ContainsKey(cellIndex) || this.m_goopedCells[cellIndex] == null || this.m_goopedCells[cellIndex].IsFrozen || (double) this.m_goopedCells[cellIndex].remainingLifespan < (double) this.goopDefinition.fadePeriod)
      return;
    if (!this.m_goopedCells[cellIndex].IsElectrified)
    {
      this.m_goopedCells[cellIndex].IsElectrified = true;
      this.m_goopedCells[cellIndex].remainingElecTimer = 0.0f;
    }
    this.m_goopedCells[cellIndex].remainingElectrifiedTime = this.goopDefinition.electrifiedTime;
  }

  [DebuggerHidden]
  private IEnumerator HandleRecursiveElectrification(IntVector2 cellIndex)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DeadlyDeadlyGoopManager.\u003CHandleRecursiveElectrification\u003Ec__Iterator0()
    {
      cellIndex = cellIndex,
      \u0024this = this
    };
  }

  private void FreezeCell(IntVector2 cellIndex)
  {
    if (!this.goopDefinition.CanBeFrozen)
      return;
    DeadlyDeadlyGoopManager.GoopPositionData goopedCell = this.m_goopedCells[cellIndex];
    goopedCell.IsFrozen = true;
    goopedCell.remainingFreezeTimer = this.goopDefinition.freezeLifespan;
  }

  public void ElectrifyGoopCircle(Vector2 center, float radius)
  {
    if (!this.goopDefinition.CanBeElectrified)
      return;
    int num1 = Mathf.CeilToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.FloorToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.CeilToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.FloorToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 a = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    for (int x = num1; x < num2; ++x)
    {
      bool flag = false;
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 intVector2 = new IntVector2(x, y);
        if (this.m_goopedCells.ContainsKey(intVector2) && (double) Vector2.Distance(a, intVector2.ToVector2()) <= (double) num5)
        {
          flag = true;
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleRecursiveElectrification(intVector2));
          break;
        }
      }
      if (flag)
        break;
    }
  }

  public void FreezeGoopCircle(Vector2 center, float radius)
  {
    if (!this.goopDefinition.CanBeFrozen)
      return;
    int num1 = Mathf.CeilToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.FloorToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.CeilToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.FloorToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 a = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 intVector2 = new IntVector2(x, y);
        if (this.m_goopedCells.ContainsKey(intVector2) && (double) Vector2.Distance(a, intVector2.ToVector2()) <= (double) num5)
          this.FreezeCell(intVector2);
      }
    }
  }

  private void IgniteCell(IntVector2 cellIndex)
  {
    if (!this.goopDefinition.CanBeIgnited)
      return;
    DeadlyDeadlyGoopManager.GoopPositionData goopedCell = this.m_goopedCells[cellIndex];
    goopedCell.IsOnFire = true;
    if (!this.goopDefinition.ignitionChangesLifetime)
      return;
    goopedCell.remainingLifespan = Mathf.Min(this.m_goopedCells[cellIndex].remainingLifespan, this.goopDefinition.ignitedLifetime);
    goopedCell.lifespanOverridden = true;
  }

  public void IgniteGoopCircle(Vector2 center, float radius)
  {
    if (!this.goopDefinition.CanBeIgnited)
      return;
    int num1 = Mathf.CeilToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.FloorToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.CeilToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.FloorToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 intVector2 = new IntVector2(x, y);
        if (this.m_goopedCells.ContainsKey(intVector2))
          this.IgniteCell(intVector2);
      }
    }
  }

  public bool ProcessGameActor(GameActor actor)
  {
    if (this.IsPositionInGoop(actor.specRigidbody.UnitCenter))
    {
      IntVector2 intVector2 = (actor.specRigidbody.UnitCenter / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
      PlayerController playerController1 = actor as PlayerController;
      if ((bool) (Object) playerController1 && this.goopDefinition.playerStepsChangeLifetime && playerController1.IsGrounded && !playerController1.IsSlidingOverSurface)
      {
        for (int index1 = -2; index1 <= 2; ++index1)
        {
          for (int index2 = -2; index2 <= 2; ++index2)
          {
            if ((double) (Mathf.Abs(index1) + Mathf.Abs(index2)) <= 3.5)
            {
              IntVector2 key = new IntVector2(intVector2.x + index1, intVector2.y + index2);
              if (this.m_goopedCells.ContainsKey(key))
              {
                DeadlyDeadlyGoopManager.GoopPositionData goopedCell = this.m_goopedCells[key];
                if ((double) goopedCell.remainingLifespan > (double) this.goopDefinition.playerStepsLifetime)
                  goopedCell.remainingLifespan = this.goopDefinition.playerStepsLifetime;
              }
            }
          }
        }
      }
      if (actor.IsFlying && !this.m_goopedCells[intVector2].IsOnFire)
        return false;
      if (actor is PlayerController)
      {
        PlayerController playerController2 = actor as PlayerController;
        if ((bool) (Object) playerController2.CurrentGun && playerController2.CurrentGun.gunName == "Mermaid Gun")
          return false;
      }
      if (!this.m_containedActors.ContainsKey(actor))
      {
        this.m_containedActors.Add(actor, 0.0f);
        this.InitialGoopEffect(actor);
      }
      else
        this.m_containedActors[actor] = this.m_containedActors[actor] + BraveTime.DeltaTime;
      this.DoTimelessGoopEffect(actor, intVector2);
      switch (actor)
      {
        case AIActor _:
          this.DoGoopEffect(actor, intVector2);
          break;
        case PlayerController _:
          PlayerController playerController3 = actor as PlayerController;
          if (this.goopDefinition.damagesPlayers && playerController3.spriteAnimator.QueryGroundedFrame())
          {
            if ((double) playerController3.CurrentPoisonMeterValue >= 1.0)
            {
              this.DoGoopEffect(actor, intVector2);
              --playerController3.CurrentPoisonMeterValue;
            }
            playerController3.IncreasePoison(BraveTime.DeltaTime / this.goopDefinition.delayBeforeDamageToPlayers);
          }
          if (this.goopDefinition.DrainsAmmo && playerController3.spriteAnimator.QueryGroundedFrame())
          {
            if ((double) playerController3.CurrentDrainMeterValue >= 1.0)
            {
              playerController3.inventory.HandleAmmoDrain(this.goopDefinition.PercentAmmoDrainPerSecond * BraveTime.DeltaTime);
              break;
            }
            playerController3.CurrentDrainMeterValue += BraveTime.DeltaTime / this.goopDefinition.delayBeforeDamageToPlayers;
            break;
          }
          break;
      }
      return true;
    }
    if (this.m_containedActors.ContainsKey(actor))
    {
      this.m_containedActors.Remove(actor);
      this.EndGoopEffect(actor);
    }
    return false;
  }

  public bool IsPositionOnFire(Vector2 position)
  {
    DeadlyDeadlyGoopManager.GoopPositionData goopPositionData;
    return this.m_goopedCells.TryGetValue((position / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor), out goopPositionData) && (double) goopPositionData.remainingLifespan > (double) this.goopDefinition.fadePeriod && goopPositionData.IsOnFire;
  }

  public bool IsPositionFrozen(Vector2 position)
  {
    DeadlyDeadlyGoopManager.GoopPositionData goopPositionData;
    return this.m_goopedCells.TryGetValue((position / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor), out goopPositionData) && (double) goopPositionData.remainingLifespan > (double) this.goopDefinition.fadePeriod && goopPositionData.IsFrozen;
  }

  public bool IsPositionInGoop(Vector2 position)
  {
    DeadlyDeadlyGoopManager.GoopPositionData goopPositionData;
    return this.m_goopedCells.TryGetValue((position / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor), out goopPositionData) && (double) goopPositionData.remainingLifespan > (double) this.goopDefinition.fadePeriod;
  }

  public void InitialGoopEffect(GameActor actor)
  {
    if (!this.goopDefinition.AppliesSpeedModifier)
      return;
    actor.ApplyEffect((GameActorEffect) this.goopDefinition.SpeedModifierEffect);
  }

  public void DoTimelessGoopEffect(GameActor actor, IntVector2 goopPosition)
  {
    float num = 0.0f;
    CoreDamageTypes damageTypes = CoreDamageTypes.None;
    if (this.m_goopedCells[goopPosition].IsOnFire)
    {
      switch (actor)
      {
        case PlayerController _ when !this.goopDefinition.UsesGreenFire:
          PlayerController playerController = actor as PlayerController;
          if (playerController.IsGrounded && !playerController.IsSlidingOverSurface && (bool) (Object) playerController.healthHaver && playerController.healthHaver.IsVulnerable)
          {
            playerController.IsOnFire = true;
            playerController.CurrentFireMeterValue += BraveTime.DeltaTime * 0.5f;
            break;
          }
          break;
        case AIActor _:
          if (this.goopDefinition.fireBurnsEnemies)
          {
            if ((double) actor.GetResistanceForEffectType(EffectResistanceType.Fire) < 1.0)
              num += this.goopDefinition.fireDamagePerSecondToEnemies * BraveTime.DeltaTime;
            (actor as AIActor).ApplyEffect((GameActorEffect) this.goopDefinition.fireEffect);
            break;
          }
          num += this.goopDefinition.fireDamagePerSecondToEnemies * BraveTime.DeltaTime;
          break;
      }
      damageTypes |= CoreDamageTypes.Fire;
    }
    if (this.m_goopedCells[goopPosition].IsElectrified)
    {
      switch (actor)
      {
        case PlayerController _:
          num = Mathf.Max(num, this.goopDefinition.electrifiedDamageToPlayer);
          break;
        case AIActor _:
          num += this.goopDefinition.electrifiedDamagePerSecondToEnemies * BraveTime.DeltaTime;
          break;
      }
      damageTypes |= CoreDamageTypes.Electric;
    }
    if (this.goopDefinition.AppliesSpeedModifierContinuously)
      actor.ApplyEffect((GameActorEffect) this.goopDefinition.SpeedModifierEffect);
    if (this.goopDefinition.AppliesDamageOverTime)
      actor.ApplyEffect((GameActorEffect) this.goopDefinition.HealthModifierEffect);
    if (actor is AIActor && (actor as AIActor).IsNormalEnemy && this.goopDefinition.AppliesCharm)
      actor.ApplyEffect((GameActorEffect) this.goopDefinition.CharmModifierEffect);
    if (actor is AIActor && (actor as AIActor).IsNormalEnemy && this.goopDefinition.AppliesCheese)
    {
      AIActor aiActor = actor as AIActor;
      if (!aiActor.IsGone && aiActor.HasBeenEngaged)
        actor.ApplyEffect((GameActorEffect) this.goopDefinition.CheeseModifierEffect, BraveTime.DeltaTime * this.goopDefinition.CheeseModifierEffect.CheeseAmount);
    }
    if ((double) num <= 0.0)
      return;
    actor.healthHaver.ApplyDamage(num, Vector2.zero, StringTableManager.GetEnemiesString("#GOOP"), damageTypes, DamageCategory.Environment);
  }

  public void DoGoopEffect(GameActor actor, IntVector2 goopPosition)
  {
    float damage = 0.0f;
    if (this.goopDefinition.damagesPlayers && actor is PlayerController)
      damage = this.goopDefinition.damageToPlayers;
    else if (this.goopDefinition.damagesEnemies && actor is AIActor)
      damage = this.goopDefinition.damagePerSecondtoEnemies * BraveTime.DeltaTime;
    if ((double) damage <= 0.0)
      return;
    actor.healthHaver.ApplyDamage(damage, Vector2.zero, StringTableManager.GetEnemiesString("#GOOP"), this.goopDefinition.damageTypes, DamageCategory.Environment, true);
  }

  public void EndGoopEffect(GameActor actor)
  {
    if (!this.goopDefinition.AppliesSpeedModifier)
      return;
    actor.RemoveEffect((GameActorEffect) this.goopDefinition.SpeedModifierEffect);
  }

  private void SetColorDirty(IntVector2 goopPosition)
  {
    IntVector2 intVector2 = (goopPosition.ToVector2() * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
    int num = Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int index1 = Mathf.FloorToInt((float) intVector2.x / (float) this.CHUNK_SIZE);
    int index2 = Mathf.FloorToInt((float) intVector2.y / (float) this.CHUNK_SIZE);
    bool flag1 = index1 > 0 && goopPosition.x % num == 0;
    bool flag2 = index1 < this.m_colorDirtyFlags.GetLength(0) - 1 && goopPosition.x % num == num - 1;
    bool flag3 = index2 > 0 && goopPosition.y % num == 0;
    bool flag4 = index2 < this.m_colorDirtyFlags.GetLength(1) - 1 && goopPosition.y % num == num - 1;
    this.m_colorDirtyFlags[index1, index2] = true;
    if (flag1)
      this.m_colorDirtyFlags[index1 - 1, index2] = true;
    if (flag2)
      this.m_colorDirtyFlags[index1 + 1, index2] = true;
    if (flag3)
      this.m_colorDirtyFlags[index1, index2 - 1] = true;
    if (flag4)
      this.m_colorDirtyFlags[index1, index2 + 1] = true;
    if (flag1 && flag3)
      this.m_colorDirtyFlags[index1 - 1, index2 - 1] = true;
    if (flag1 && flag4)
      this.m_colorDirtyFlags[index1 - 1, index2 + 1] = true;
    if (flag2 && flag3)
      this.m_colorDirtyFlags[index1 + 1, index2 - 1] = true;
    if (!flag2 || !flag4)
      return;
    this.m_colorDirtyFlags[index1 + 1, index2 + 1] = true;
  }

  private void SetDirty(IntVector2 goopPosition)
  {
    int num1 = (int) ((double) goopPosition.x * (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = (int) ((double) goopPosition.y * (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int index1 = Mathf.FloorToInt((float) num1 / (float) this.CHUNK_SIZE);
    int index2 = Mathf.FloorToInt((float) num2 / (float) this.CHUNK_SIZE);
    if (index1 < 0 || index1 >= this.m_dirtyFlags.GetLength(0) || index2 < 0 || index2 >= this.m_dirtyFlags.GetLength(1))
      return;
    bool flag1 = index1 > 0 && goopPosition.x % num3 == 0;
    bool flag2 = index1 < this.m_dirtyFlags.GetLength(0) - 1 && goopPosition.x % num3 == num3 - 1;
    bool flag3 = index2 > 0 && goopPosition.y % num3 == 0;
    bool flag4 = index2 < this.m_dirtyFlags.GetLength(1) - 1 && goopPosition.y % num3 == num3 - 1;
    this.m_dirtyFlags[index1, index2] = true;
    if (flag1)
      this.m_dirtyFlags[index1 - 1, index2] = true;
    if (flag2)
      this.m_dirtyFlags[index1 + 1, index2] = true;
    if (flag3)
      this.m_dirtyFlags[index1, index2 - 1] = true;
    if (flag4)
      this.m_dirtyFlags[index1, index2 + 1] = true;
    if (flag1 && flag3)
      this.m_dirtyFlags[index1 - 1, index2 - 1] = true;
    if (flag1 && flag4)
      this.m_dirtyFlags[index1 - 1, index2 + 1] = true;
    if (flag2 && flag3)
      this.m_dirtyFlags[index1 + 1, index2 - 1] = true;
    if (!flag2 || !flag4)
      return;
    this.m_dirtyFlags[index1 + 1, index2 + 1] = true;
  }

  private void InitialzeUV2IfNecessary()
  {
    if (!this.goopDefinition.usesWorldTextureByDefault)
      return;
    int num = Mathf.RoundToInt((float) (4.0 * ((double) this.CHUNK_SIZE / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) * ((double) this.CHUNK_SIZE / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE)));
    for (int index = 0; index < num; ++index)
      this.m_uv2Array[index] = Vector2.right;
  }

  private void InitializeParticleSystems()
  {
    string name1 = !this.goopDefinition.UsesGreenFire ? "Gungeon_Fire_Main" : "Gungeon_Fire_Main_Green";
    string name2 = !this.goopDefinition.UsesGreenFire ? "Gungeon_Fire_Intro" : "Gungeon_Fire_Intro_Green";
    string name3 = !this.goopDefinition.UsesGreenFire ? "Gungeon_Fire_Outro" : "Gungeon_Fire_Outro_Green";
    GameObject gameObject1 = GameObject.Find(name1);
    if ((Object) gameObject1 == (Object) null)
    {
      gameObject1 = (GameObject) Object.Instantiate(BraveResources.Load(!this.goopDefinition.UsesGreenFire ? "Particles/Gungeon_Fire_Main_raw" : "Particles/Gungeon_Fire_Main_green"), Vector3.zero, Quaternion.identity);
      gameObject1.name = name1;
    }
    this.m_fireSystem = gameObject1.GetComponent<ParticleSystem>();
    GameObject gameObject2 = GameObject.Find(name2);
    if ((Object) gameObject2 == (Object) null)
    {
      gameObject2 = (GameObject) Object.Instantiate(BraveResources.Load(!this.goopDefinition.UsesGreenFire ? "Particles/Gungeon_Fire_Intro_raw" : "Particles/Gungeon_Fire_Intro_green"), Vector3.zero, Quaternion.identity);
      gameObject2.name = name2;
    }
    this.m_fireIntroSystem = gameObject2.GetComponent<ParticleSystem>();
    GameObject gameObject3 = GameObject.Find(name3);
    if ((Object) gameObject3 == (Object) null)
    {
      gameObject3 = (GameObject) Object.Instantiate(BraveResources.Load(!this.goopDefinition.UsesGreenFire ? "Particles/Gungeon_Fire_Outro_raw" : "Particles/Gungeon_Fire_Outro_green"), Vector3.zero, Quaternion.identity);
      gameObject3.name = name3;
    }
    this.m_fireOutroSystem = gameObject3.GetComponent<ParticleSystem>();
    GameObject gameObject4 = GameObject.Find("Gungeon_Elec");
    if ((Object) gameObject4 == (Object) null)
    {
      gameObject4 = (GameObject) Object.Instantiate(BraveResources.Load("Particles/Gungeon_Elec_raw"), Vector3.zero, Quaternion.identity);
      gameObject4.name = "Gungeon_Elec";
    }
    this.m_elecSystem = gameObject4.GetComponent<ParticleSystem>();
  }

  private void LateUpdate()
  {
    if ((double) UnityEngine.Time.timeScale <= 0.0 || GameManager.Instance.IsPaused)
      return;
    this.m_removalPositions.Clear();
    bool flag1 = false;
    bool flag2 = false;
    this.m_currentUpdateBin = (this.m_currentUpdateBin + 1) % 4;
    double num1 = (double) this.m_deltaTimes.Enqueue(BraveTime.DeltaTime);
    float num2 = 0.0f;
    for (int index = 0; index < this.m_deltaTimes.Count; ++index)
      num2 += this.m_deltaTimes[index];
    foreach (IntVector2 goopedPosition in this.m_goopedPositions)
    {
      DeadlyDeadlyGoopManager.GoopPositionData goopedCell = this.m_goopedCells[goopedPosition];
      if (goopedCell.GoopUpdateBin == this.m_currentUpdateBin)
      {
        goopedCell.unfrozeLastFrame = false;
        if (this.goopDefinition.usesAmbientGoopFX && (double) goopedCell.remainingLifespan > 0.0 && (double) Random.value < (double) this.goopDefinition.ambientGoopFXChance && goopedCell.SupportsAmbientVFX)
          this.goopDefinition.ambientGoopFX.SpawnAtPosition(goopedPosition.ToVector3((float) goopedPosition.y) * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
        if (goopedCell.IsOnFire || goopedCell.IsElectrified || this.goopDefinition.usesLifespan || goopedCell.lifespanOverridden || goopedCell.selfIgnites)
        {
          if (goopedCell.selfIgnites)
          {
            if ((double) goopedCell.remainingTimeTilSelfIgnition <= 0.0)
            {
              goopedCell.selfIgnites = false;
              this.IgniteCell(goopedPosition);
            }
            else
              goopedCell.remainingTimeTilSelfIgnition -= num2;
          }
          if ((double) goopedCell.remainingLifespan > 0.0)
          {
            if (!goopedCell.IsFrozen)
            {
              goopedCell.remainingLifespan -= num2;
            }
            else
            {
              goopedCell.remainingFreezeTimer -= num2;
              if ((double) goopedCell.remainingFreezeTimer <= 0.0)
              {
                goopedCell.hasBeenFrozen = 1;
                goopedCell.remainingLifespan = Mathf.Min(goopedCell.remainingLifespan, this.goopDefinition.fadePeriod);
                goopedCell.remainingLifespan -= num2;
              }
            }
            if (this.goopDefinition.usesAcidAudio)
              flag2 = true;
            if ((double) goopedCell.remainingLifespan < (double) this.goopDefinition.fadePeriod && goopedCell.IsElectrified)
              goopedCell.remainingLifespan = this.goopDefinition.fadePeriod;
            if ((double) goopedCell.remainingLifespan < (double) this.goopDefinition.fadePeriod || (double) goopedCell.remainingLifespan <= 0.0)
            {
              this.SetDirty(goopedPosition);
              goopedCell.IsOnFire = false;
              goopedCell.IsElectrified = false;
              goopedCell.HasPlayedFireIntro = false;
              goopedCell.HasPlayedFireOutro = false;
              if ((double) goopedCell.remainingLifespan <= 0.0)
              {
                this.m_removalPositions.Add(goopedPosition);
                continue;
              }
            }
            ParticleSystem.EmitParams emitParams;
            if (goopedCell.IsElectrified)
            {
              goopedCell.remainingElectrifiedTime -= num2;
              goopedCell.remainingElecTimer -= num2;
              if ((double) goopedCell.remainingElectrifiedTime <= 0.0)
              {
                goopedCell.IsElectrified = false;
                goopedCell.remainingElectrifiedTime = 0.0f;
              }
              if (goopedCell.IsElectrified && (Object) this.m_elecSystem != (Object) null && (double) goopedCell.remainingElecTimer <= 0.0 && goopedPosition.x % 2 == 0 && goopedPosition.y % 2 == 0)
              {
                Vector3 vector3 = goopedPosition.ToVector3((float) goopedPosition.y) * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + new Vector3(Random.Range(0.125f, 0.375f), Random.Range(0.125f, 0.375f), 0.125f).Quantize(1f / 16f);
                float num3 = Random.Range(0.75f, 1.5f);
                if ((double) Random.value < 0.10000000149011612)
                {
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = vector3;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num3;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) Random.value * 4294967296.0);
                  this.m_elecSystem.Emit(emitParams, 1);
                  if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
                    GlobalSparksDoer.DoRandomParticleBurst(GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.MEDIUM ? 10 : 4, vector3 + new Vector3(-0.625f, -0.625f, 0.0f), vector3 + new Vector3(0.625f, 0.625f, 0.0f), Vector3.up, 120f, 0.5f);
                }
                goopedCell.remainingElecTimer = num3 - 0.1f;
              }
            }
            if (goopedCell.IsFrozen)
            {
              if ((double) goopedCell.totalOnFireTime < 0.5 || (double) goopedCell.remainingLifespan < (double) this.goopDefinition.fadePeriod)
                this.SetColorDirty(goopedPosition);
              goopedCell.totalOnFireTime += num2;
              if ((double) goopedCell.totalOnFireTime >= (double) this.goopDefinition.freezeSpreadTime)
              {
                for (int index = 0; index < goopedCell.neighborGoopData.Length; ++index)
                {
                  if (goopedCell.neighborGoopData[index] != null && !goopedCell.neighborGoopData[index].IsFrozen && goopedCell.neighborGoopData[index].hasBeenFrozen == 0)
                  {
                    if ((double) Random.value < 0.20000000298023224)
                      this.FreezeCell(goopedCell.neighborGoopData[index].goopPosition);
                    else
                      goopedCell.totalFrozenTime = 0.0f;
                  }
                }
              }
            }
            if (goopedCell.IsOnFire)
            {
              flag1 = true;
              this.SetColorDirty(goopedPosition);
              goopedCell.remainingFireTimer -= num2;
              goopedCell.totalOnFireTime += num2;
              if ((double) goopedCell.totalOnFireTime >= (double) this.goopDefinition.igniteSpreadTime)
              {
                for (int index = 0; index < goopedCell.neighborGoopData.Length; ++index)
                {
                  if (goopedCell.neighborGoopData[index] != null && !goopedCell.neighborGoopData[index].IsOnFire)
                  {
                    if ((double) Random.value < 0.20000000298023224)
                      this.IgniteCell(goopedCell.neighborGoopData[index].goopPosition);
                    else
                      goopedCell.totalOnFireTime = 0.0f;
                  }
                }
              }
            }
            if ((Object) this.m_fireSystem != (Object) null && goopedCell.IsOnFire && (double) goopedCell.remainingFireTimer <= 0.0 && goopedPosition.x % 2 == 0 && goopedPosition.y % 2 == 0)
            {
              Vector3 vector3 = goopedPosition.ToVector3((float) goopedPosition.y) * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + new Vector3(Random.Range(0.125f, 0.375f), Random.Range(0.125f, 0.375f), 0.125f).Quantize(1f / 16f);
              float num4 = Random.Range(1f, 1.5f);
              float num5 = Random.Range(0.75f, 1f);
              if (!goopedCell.HasPlayedFireOutro)
              {
                if (!goopedCell.HasPlayedFireOutro && (double) goopedCell.remainingLifespan <= (double) num5 + (double) this.goopDefinition.fadePeriod && (Object) this.m_fireOutroSystem != (Object) null)
                {
                  num4 = num5;
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = vector3;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num5;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) Random.value * 4294967296.0);
                  this.m_fireOutroSystem.Emit(emitParams, 1);
                  goopedCell.HasPlayedFireOutro = true;
                }
                else if (!goopedCell.HasPlayedFireIntro && (Object) this.m_fireIntroSystem != (Object) null)
                {
                  num4 = Random.Range(0.75f, 1f);
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = vector3;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num4;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) Random.value * 4294967296.0);
                  this.m_fireIntroSystem.Emit(emitParams, 1);
                  goopedCell.HasPlayedFireIntro = true;
                }
                else
                {
                  if ((double) Random.value < 0.5)
                  {
                    emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = vector3;
                    emitParams.velocity = Vector3.zero;
                    emitParams.startSize = this.m_fireSystem.startSize;
                    emitParams.startLifetime = num4;
                    emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                    emitParams.randomSeed = (uint) ((double) Random.value * 4294967296.0);
                    this.m_fireSystem.Emit(emitParams, 1);
                  }
                  GlobalSparksDoer.DoRandomParticleBurst(Random.Range(3, 6), vector3, vector3, Vector3.up * 2f, 30f, 1f, startLifetime: new float?(Random.Range(0.5f, 1f)), startColor: new Color?(!this.goopDefinition.UsesGreenFire ? Color.red : Color.green));
                }
              }
              goopedCell.remainingFireTimer = num4 - 0.125f;
            }
          }
          else
            this.m_removalPositions.Add(goopedPosition);
        }
      }
    }
    if (flag1 && !this.m_isPlayingFireAudio)
    {
      this.m_isPlayingFireAudio = true;
      int num6 = (int) AkSoundEngine.PostEvent("Play_ENV_oilfire_ignite_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }
    else if (!flag1 && this.m_isPlayingFireAudio)
    {
      this.m_isPlayingFireAudio = false;
      int num7 = (int) AkSoundEngine.PostEvent("Stop_ENV_oilfire_loop_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }
    if (flag2 && !this.m_isPlayingAcidAudio)
    {
      this.m_isPlayingAcidAudio = true;
      int num8 = (int) AkSoundEngine.PostEvent("Play_ENV_acidsizzle_loop_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }
    else if (!flag2 && this.m_isPlayingAcidAudio)
    {
      this.m_isPlayingAcidAudio = false;
      int num9 = (int) AkSoundEngine.PostEvent("Stop_ENV_acidsizzle_loop_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }
    this.RemoveGoopedPosition(this.m_removalPositions);
    for (int chunkX = 0; chunkX < this.m_dirtyFlags.GetLength(0); ++chunkX)
    {
      for (int chunkY = 0; chunkY < this.m_dirtyFlags.GetLength(1); ++chunkY)
      {
        if (this.m_dirtyFlags[chunkX, chunkY])
        {
          if ((chunkY * this.m_dirtyFlags.GetLength(0) + chunkX) % 3 == UnityEngine.Time.frameCount % 3)
          {
            bool flag3 = this.HasGoopedPositionCountForChunk(chunkX, chunkY);
            if (flag3)
              this.RebuildMeshUvsAndColors(chunkX, chunkY);
            this.m_dirtyFlags[chunkX, chunkY] = false;
            this.m_colorDirtyFlags[chunkX, chunkY] = false;
            if ((Object) this.m_meshes[chunkX, chunkY] != (Object) null && !flag3)
            {
              Object.Destroy((Object) this.m_mrs[chunkX, chunkY].gameObject);
              Object.Destroy((Object) this.m_meshes[chunkX, chunkY]);
              this.m_mrs[chunkX, chunkY] = (MeshRenderer) null;
              this.m_meshes[chunkX, chunkY] = (Mesh) null;
            }
          }
        }
        else if (this.m_colorDirtyFlags[chunkX, chunkY] && (chunkY * this.m_dirtyFlags.GetLength(0) + chunkX) % 3 == UnityEngine.Time.frameCount % 3)
        {
          bool flag4 = this.HasGoopedPositionCountForChunk(chunkX, chunkY);
          if (flag4)
            this.RebuildMeshColors(chunkX, chunkY);
          this.m_colorDirtyFlags[chunkX, chunkY] = false;
          if ((Object) this.m_meshes[chunkX, chunkY] != (Object) null && !flag4)
          {
            Object.Destroy((Object) this.m_mrs[chunkX, chunkY].gameObject);
            Object.Destroy((Object) this.m_meshes[chunkX, chunkY]);
            this.m_mrs[chunkX, chunkY] = (MeshRenderer) null;
            this.m_meshes[chunkX, chunkY] = (Mesh) null;
          }
        }
      }
    }
  }

  private bool HasGoopedPositionCountForChunk(int chunkX, int chunkY)
  {
    int num = Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    IntVector2 intVector2 = new IntVector2(chunkX * num, chunkY * num);
    for (int x = 0; x < num; ++x)
    {
      for (int y = 0; y < num; ++y)
      {
        if (this.m_goopedPositions.Contains(intVector2 + new IntVector2(x, y)))
          return true;
      }
    }
    return false;
  }

  private void RebuildMeshUvsAndColors(int chunkX, int chunkY)
  {
    Mesh chunkMesh = this.GetChunkMesh(chunkX, chunkY);
    for (int index = 0; index < this.m_colorArray.Length; ++index)
      this.m_colorArray[index] = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) 0);
    int num1 = chunkX * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = num1 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = chunkY * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = num3 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    for (int index1 = num1; index1 < num2; ++index1)
    {
      for (int index2 = num3; index2 < num4; ++index2)
      {
        IntVector2 intVector2;
        intVector2.x = index1;
        intVector2.y = index2;
        DeadlyDeadlyGoopManager.GoopPositionData goopData;
        if (this.m_goopedCells.TryGetValue(intVector2, out goopData) && (double) goopData.remainingLifespan > 0.0)
        {
          if (goopData.baseIndex < 0)
            goopData.baseIndex = this.GetGoopBaseIndex(intVector2, chunkX, chunkY);
          this.AssignUvsAndColors(goopData, intVector2, chunkX, chunkY);
        }
      }
    }
    chunkMesh.uv = this.m_uvArray;
    chunkMesh.uv2 = this.m_uv2Array;
    chunkMesh.colors32 = this.m_colorArray;
  }

  private void RebuildMeshColors(int chunkX, int chunkY)
  {
    Mesh chunkMesh = this.GetChunkMesh(chunkX, chunkY);
    for (int index = 0; index < this.m_colorArray.Length; ++index)
      this.m_colorArray[index] = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) 0);
    int num1 = chunkX * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = num1 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = chunkY * Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = num3 + Mathf.RoundToInt((float) this.CHUNK_SIZE / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    DeadlyDeadlyGoopManager.VertexColorRebuildResult b = DeadlyDeadlyGoopManager.VertexColorRebuildResult.ALL_OK;
    foreach (IntVector2 goopedPosition in this.m_goopedPositions)
    {
      DeadlyDeadlyGoopManager.GoopPositionData goopedCell = this.m_goopedCells[goopedPosition];
      if ((double) goopedCell.remainingLifespan >= 0.0 && goopedPosition.x >= num1 && goopedPosition.x < num2 && goopedPosition.y >= num3 && goopedPosition.y < num4)
      {
        if (goopedCell.baseIndex < 0)
          goopedCell.baseIndex = this.GetGoopBaseIndex(goopedPosition, chunkX, chunkY);
        if (this.goopDefinition.CanBeFrozen)
        {
          int x = !goopedCell.IsFrozen ? 0 : 1;
          this.m_uv2Array[goopedCell.baseIndex] = new Vector2((float) x, 0.0f);
          this.m_uv2Array[goopedCell.baseIndex + 1] = new Vector2((float) x, 0.0f);
          this.m_uv2Array[goopedCell.baseIndex + 2] = new Vector2((float) x, 0.0f);
          this.m_uv2Array[goopedCell.baseIndex + 3] = new Vector2((float) x, 0.0f);
        }
        b = (DeadlyDeadlyGoopManager.VertexColorRebuildResult) Mathf.Max((int) this.AssignVertexColors(goopedCell, goopedPosition, chunkX, chunkY), (int) b);
      }
    }
    if (this.goopDefinition.CanBeFrozen)
      chunkMesh.uv2 = this.m_uv2Array;
    chunkMesh.colors32 = this.m_colorArray;
  }

  private void PostprocessRebuildResult(
    int chunkX,
    int chunkY,
    DeadlyDeadlyGoopManager.VertexColorRebuildResult rr)
  {
    Material sharedMaterial = this.m_mrs[chunkX, chunkY].sharedMaterial;
    switch (rr)
    {
      case DeadlyDeadlyGoopManager.VertexColorRebuildResult.ALL_OK:
        float num = !this.goopDefinition.usesOverrideOpaqueness ? 0.5f : this.goopDefinition.overrideOpaqueness;
        sharedMaterial.SetFloat(DeadlyDeadlyGoopManager.OpaquenessMultiplyPropertyID, num);
        sharedMaterial.SetFloat(DeadlyDeadlyGoopManager.BrightnessMultiplyPropertyID, 1f);
        break;
      case DeadlyDeadlyGoopManager.VertexColorRebuildResult.ELECTRIFIED:
        sharedMaterial.SetFloat(DeadlyDeadlyGoopManager.OpaquenessMultiplyPropertyID, 1f);
        sharedMaterial.SetFloat(DeadlyDeadlyGoopManager.BrightnessMultiplyPropertyID, 5f);
        break;
    }
  }

  private void InitMesh(IntVector2 goopPos, int chunkX, int chunkY)
  {
    int num1 = chunkX * this.CHUNK_SIZE;
    int num2 = chunkY * this.CHUNK_SIZE;
    int num3 = (int) ((double) num1 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + 0.5);
    int num4 = (int) ((double) num2 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + 0.5);
    int num5 = goopPos.x - num3;
    int index1 = (goopPos.y - num4) * (4 * (int) (1.0 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE)) * this.CHUNK_SIZE + num5 * 4;
    bool flag = false;
    IntVector2 intVector2 = new IntVector2((int) ((double) goopPos.x * (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE), (int) ((double) goopPos.y * (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE));
    if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2))
    {
      CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
      flag = cellData != null && !cellData.forceDisallowGoop && cellData.IsLowerFaceWall();
    }
    if (flag)
    {
      float num6 = (float) goopPos.x * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
      float f = (float) goopPos.y * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
      float num7 = Mathf.Floor(f) - f % 1f;
      Vector3 vector3 = new Vector3(num6 - (float) num1, f - (float) num2, num7 - (float) num2);
      this.m_vertexArray[index1] = vector3;
      this.m_vertexArray[index1 + 1] = vector3 + new Vector3(DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, 0.0f, 0.0f);
      this.m_vertexArray[index1 + 2] = vector3 + new Vector3(0.0f, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, -DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
      this.m_vertexArray[index1 + 3] = vector3 + new Vector3(DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, -DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    }
    else
    {
      Vector3 vector3 = new Vector3((float) goopPos.x * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE - (float) num1, (float) goopPos.y * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE - (float) num2, (float) goopPos.y * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE - (float) num2);
      this.m_vertexArray[index1] = vector3;
      this.m_vertexArray[index1 + 1] = vector3 + new Vector3(DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, 0.0f, 0.0f);
      this.m_vertexArray[index1 + 2] = vector3 + new Vector3(0.0f, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
      this.m_vertexArray[index1 + 3] = vector3 + new Vector3(DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    }
    int index2 = index1 / 4 * 6;
    this.m_triangleArray[index2] = index1;
    this.m_triangleArray[index2 + 1] = index1 + 1;
    this.m_triangleArray[index2 + 2] = index1 + 2;
    this.m_triangleArray[index2 + 3] = index1 + 3;
    this.m_triangleArray[index2 + 4] = index1 + 2;
    this.m_triangleArray[index2 + 5] = index1 + 1;
  }

  private int GetGoopBaseIndex(IntVector2 goopPos, int chunkX, int chunkY)
  {
    int num1 = chunkX * this.CHUNK_SIZE;
    int num2 = chunkY * this.CHUNK_SIZE;
    int num3 = (int) ((double) num1 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + 0.5);
    int num4 = (int) ((double) num2 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE + 0.5);
    int num5 = goopPos.x - num3;
    return (goopPos.y - num4) * (4 * (int) (1.0 / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE)) * this.CHUNK_SIZE + num5 * 4;
  }

  private void AssignUvsAndColors(
    DeadlyDeadlyGoopManager.GoopPositionData goopData,
    IntVector2 goopPos,
    int chunkX,
    int chunkY)
  {
    Vector2 vector2 = !this.m_uvMap.ContainsKey(goopData.NeighborsAsInt) ? (!this.m_uvMap.ContainsKey(goopData.NeighborsAsIntFuckDiagonals) ? this.m_uvMap[-1] : this.m_uvMap[goopData.NeighborsAsIntFuckDiagonals]) : this.m_uvMap[goopData.NeighborsAsInt];
    if (goopData.NeighborsAsInt == (int) byte.MaxValue)
      vector2 = this.m_centerUVOptions[Mathf.FloorToInt((float) this.m_centerUVOptions.Count * goopPos.GetHashedRandomValue())];
    this.m_uvArray[goopData.baseIndex] = vector2;
    this.m_uvArray[goopData.baseIndex + 1] = vector2 + new Vector2(0.125f, 0.0f);
    this.m_uvArray[goopData.baseIndex + 2] = vector2 + new Vector2(0.0f, 0.125f);
    this.m_uvArray[goopData.baseIndex + 3] = vector2 + new Vector2(0.125f, 0.125f);
    if (this.goopDefinition.CanBeFrozen)
    {
      int x = !goopData.IsFrozen ? 0 : 1;
      this.m_uv2Array[goopData.baseIndex] = new Vector2((float) x, 0.0f);
      this.m_uv2Array[goopData.baseIndex + 1] = new Vector2((float) x, 0.0f);
      this.m_uv2Array[goopData.baseIndex + 2] = new Vector2((float) x, 0.0f);
      this.m_uv2Array[goopData.baseIndex + 3] = new Vector2((float) x, 0.0f);
    }
    int num = (int) this.AssignVertexColors(goopData, goopPos, chunkX, chunkY);
  }

  private DeadlyDeadlyGoopManager.VertexColorRebuildResult AssignVertexColors(
    DeadlyDeadlyGoopManager.GoopPositionData goopData,
    IntVector2 goopPos,
    int chunkX,
    int chunkY)
  {
    DeadlyDeadlyGoopManager.VertexColorRebuildResult colorRebuildResult = DeadlyDeadlyGoopManager.VertexColorRebuildResult.ALL_OK;
    bool flag = false;
    Color32 b1 = this.goopDefinition.baseColor32;
    Color32 b2 = b1;
    Color32 b3 = b1;
    Color32 b4 = b1;
    if (goopData.IsOnFire)
      b1 = this.goopDefinition.fireColor32;
    else if (goopData.HasOnFireNeighbor)
    {
      flag = true;
      for (int index = 0; index < 8; ++index)
      {
        if (goopData.neighborGoopData[index] != null && goopData.neighborGoopData[index].IsOnFire)
        {
          switch (index)
          {
            case 0:
              b3 = this.goopDefinition.igniteColor32;
              b4 = this.goopDefinition.igniteColor32;
              continue;
            case 1:
              b4 = this.goopDefinition.igniteColor32;
              continue;
            case 2:
              b4 = this.goopDefinition.igniteColor32;
              b2 = this.goopDefinition.igniteColor32;
              continue;
            case 3:
              b2 = this.goopDefinition.igniteColor32;
              continue;
            case 4:
              b2 = this.goopDefinition.igniteColor32;
              b1 = this.goopDefinition.igniteColor32;
              continue;
            case 5:
              b1 = this.goopDefinition.igniteColor32;
              continue;
            case 6:
              b1 = this.goopDefinition.igniteColor32;
              b3 = this.goopDefinition.igniteColor32;
              continue;
            case 7:
              b3 = this.goopDefinition.igniteColor32;
              continue;
            default:
              continue;
          }
        }
      }
    }
    else if (goopData.IsFrozen)
      b1 = this.goopDefinition.frozenColor32;
    else if (goopData.HasFrozenNeighbor)
    {
      flag = true;
      for (int index = 0; index < 8; ++index)
      {
        if (goopData.neighborGoopData[index] != null && goopData.neighborGoopData[index].IsFrozen)
        {
          switch (index)
          {
            case 0:
              this.m_uv2Array[goopData.baseIndex + 2] = new Vector2(0.5f, 0.0f);
              b3 = this.goopDefinition.prefreezeColor32;
              this.m_uv2Array[goopData.baseIndex + 3] = new Vector2(0.5f, 0.0f);
              b4 = this.goopDefinition.prefreezeColor32;
              continue;
            case 1:
              this.m_uv2Array[goopData.baseIndex + 3] = new Vector2(0.5f, 0.0f);
              b4 = this.goopDefinition.prefreezeColor32;
              continue;
            case 2:
              this.m_uv2Array[goopData.baseIndex + 3] = new Vector2(0.5f, 0.0f);
              b4 = this.goopDefinition.prefreezeColor32;
              this.m_uv2Array[goopData.baseIndex + 1] = new Vector2(0.5f, 0.0f);
              b2 = this.goopDefinition.prefreezeColor32;
              continue;
            case 3:
              this.m_uv2Array[goopData.baseIndex + 1] = new Vector2(0.5f, 0.0f);
              b2 = this.goopDefinition.prefreezeColor32;
              continue;
            case 4:
              this.m_uv2Array[goopData.baseIndex + 1] = new Vector2(0.5f, 0.0f);
              b2 = this.goopDefinition.prefreezeColor32;
              this.m_uv2Array[goopData.baseIndex] = new Vector2(0.5f, 0.0f);
              b1 = this.goopDefinition.prefreezeColor32;
              continue;
            case 5:
              this.m_uv2Array[goopData.baseIndex] = new Vector2(0.5f, 0.0f);
              b1 = this.goopDefinition.prefreezeColor32;
              continue;
            case 6:
              this.m_uv2Array[goopData.baseIndex] = new Vector2(0.5f, 0.0f);
              b1 = this.goopDefinition.prefreezeColor32;
              this.m_uv2Array[goopData.baseIndex + 2] = new Vector2(0.5f, 0.0f);
              b3 = this.goopDefinition.prefreezeColor32;
              continue;
            case 7:
              this.m_uv2Array[goopData.baseIndex + 2] = new Vector2(0.5f, 0.0f);
              b3 = this.goopDefinition.prefreezeColor32;
              continue;
            default:
              continue;
          }
        }
      }
    }
    if ((double) goopData.remainingLifespan < (double) this.goopDefinition.fadePeriod)
    {
      b1 = Color32.Lerp(this.goopDefinition.fadeColor32, b1, goopData.remainingLifespan / this.goopDefinition.fadePeriod);
      if (flag)
      {
        b2 = Color32.Lerp(this.goopDefinition.fadeColor32, b2, goopData.remainingLifespan / this.goopDefinition.fadePeriod);
        b3 = Color32.Lerp(this.goopDefinition.fadeColor32, b3, goopData.remainingLifespan / this.goopDefinition.fadePeriod);
        b4 = Color32.Lerp(this.goopDefinition.fadeColor32, b4, goopData.remainingLifespan / this.goopDefinition.fadePeriod);
      }
    }
    if (flag)
    {
      this.m_colorArray[goopData.baseIndex] = b1;
      this.m_colorArray[goopData.baseIndex + 1] = b2;
      this.m_colorArray[goopData.baseIndex + 2] = b3;
      this.m_colorArray[goopData.baseIndex + 3] = b4;
    }
    else
    {
      this.m_colorArray[goopData.baseIndex] = b1;
      this.m_colorArray[goopData.baseIndex + 1] = b1;
      this.m_colorArray[goopData.baseIndex + 2] = b1;
      this.m_colorArray[goopData.baseIndex + 3] = b1;
    }
    return colorRebuildResult;
  }

  private void RemoveGoopedPosition(IntVector2 entry)
  {
    IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
    for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
    {
      DeadlyDeadlyGoopManager.GoopPositionData goopPositionData;
      if (this.m_goopedCells.TryGetValue(entry + cardinalsAndOrdinals[index], out goopPositionData))
      {
        goopPositionData.neighborGoopData[(index + 4) % 8] = (DeadlyDeadlyGoopManager.GoopPositionData) null;
        goopPositionData.SetNeighborGoop((index + 4) % 8, false);
      }
    }
    this.m_goopedPositions.Remove(entry);
    this.m_goopedCells.Remove(entry);
    DeadlyDeadlyGoopManager.allGoopPositionMap.Remove(entry);
    this.SetDirty(entry);
  }

  private void RemoveGoopedPosition(List<IntVector2> entries)
  {
    for (int index = 0; index < entries.Count; ++index)
      this.RemoveGoopedPosition(entries[index]);
  }

  public int CountGoopCircle(Vector2 center, float radius)
  {
    if (this.m_goopedCells == null || this.m_goopedCells.Count == 0)
      return 0;
    int num1 = Mathf.FloorToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 b = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    int num6 = 0;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 key = new IntVector2(x, y);
        if ((double) Vector2.Distance(key.ToVector2(), b) < (double) num5 && this.m_goopedCells.ContainsKey(key))
          ++num6;
      }
    }
    return num6;
  }

  public void RemoveGoopCircle(Vector2 center, float radius)
  {
    int num1 = Mathf.FloorToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 b = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        if ((double) Vector2.Distance(new Vector2((float) x, (float) y), b) < (double) num5)
          this.RemoveGoopedPosition(new IntVector2(x, y));
      }
    }
  }

  private void AddGoopedPosition(
    IntVector2 pos,
    float radiusFraction = 0.0f,
    bool suppressSplashes = false,
    int sourceId = -1,
    int sourceFrameCount = -1)
  {
    if (GameManager.Instance.IsLoadingLevel)
      return;
    Vector2 vector = pos.ToVector2() * DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    Vector2 vector2 = vector + new Vector2(DeadlyDeadlyGoopManager.GOOP_GRID_SIZE, DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) * 0.5f;
    for (int index = 0; index < DeadlyDeadlyGoopManager.m_goopExceptions.Count; ++index)
    {
      if (DeadlyDeadlyGoopManager.m_goopExceptions[index] != null)
      {
        Vector2 first = DeadlyDeadlyGoopManager.m_goopExceptions[index].First;
        float second = DeadlyDeadlyGoopManager.m_goopExceptions[index].Second;
        if ((double) (first - vector2).sqrMagnitude < (double) second)
          return;
      }
    }
    if (!this.m_goopedCells.ContainsKey(pos))
    {
      IntVector2 intVector2 = vector.ToIntVector2(VectorConversions.Floor);
      if (!GameManager.Instance.Dungeon.data.CheckInBounds(intVector2))
        return;
      CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
      if (cellData == null || cellData != null && cellData.forceDisallowGoop || cellData.cellVisualData.absorbsDebris && this.goopDefinition.CanBeFrozen)
        return;
      if (this.goopDefinition.CanBeFrozen)
        GameManager.Instance.Dungeon.data.SolidifyLavaInCell(intVector2);
      bool flag1 = cellData.IsLowerFaceWall();
      if (flag1 && (double) pos.GetHashedRandomValue() > 0.75)
        flag1 = false;
      if (cellData.type != CellType.FLOOR && !flag1 && !cellData.forceAllowGoop)
        return;
      bool flag2 = false;
      int num = sourceFrameCount == -1 ? UnityEngine.Time.frameCount : sourceFrameCount;
      DeadlyDeadlyGoopManager deadlyGoopManager;
      if (DeadlyDeadlyGoopManager.allGoopPositionMap.TryGetValue(pos, out deadlyGoopManager))
      {
        DeadlyDeadlyGoopManager.GoopPositionData goopedCell = deadlyGoopManager.m_goopedCells[pos];
        if (goopedCell.frameGooped > num || goopedCell.eternal)
          return;
        if (goopedCell.IsOnFire)
          flag2 = true;
        deadlyGoopManager.RemoveGoopedPosition(pos);
      }
      DeadlyDeadlyGoopManager.GoopPositionData goopPositionData = new DeadlyDeadlyGoopManager.GoopPositionData(pos, this.m_goopedCells, this.goopDefinition.GetLifespan(radiusFraction));
      goopPositionData.frameGooped = sourceFrameCount == -1 ? UnityEngine.Time.frameCount : sourceFrameCount;
      goopPositionData.lastSourceID = sourceId;
      if (!suppressSplashes && DeadlyDeadlyGoopManager.m_DoGoopSpawnSplashes && (double) Random.value < 0.019999999552965164)
      {
        if ((Object) this.m_genericSplashPrefab == (Object) null)
          this.m_genericSplashPrefab = ResourceCache.Acquire("Global VFX/Generic_Goop_Splash") as GameObject;
        GameObject gameObject = SpawnManager.SpawnVFX(this.m_genericSplashPrefab, vector.ToVector3ZUp(vector.y), Quaternion.identity);
        gameObject.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
        gameObject.GetComponent<Renderer>().material.SetColor(DeadlyDeadlyGoopManager.TintColorPropertyID, (Color) this.goopDefinition.baseColor32);
      }
      goopPositionData.eternal = this.goopDefinition.eternal;
      goopPositionData.selfIgnites = this.goopDefinition.SelfIgnites;
      goopPositionData.remainingTimeTilSelfIgnition = this.goopDefinition.selfIgniteDelay;
      this.m_goopedPositions.Add(pos);
      this.m_goopedCells.Add(pos, goopPositionData);
      DeadlyDeadlyGoopManager.allGoopPositionMap.Add(pos, this);
      GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector2).RegisterGoopManagerInRoom(this);
      if (cellData.OnCellGooped != null)
        cellData.OnCellGooped(cellData);
      if (cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Ice)
        this.FreezeCell(pos);
      if (flag2 && this.goopDefinition.CanBeIgnited)
        this.IgniteCell(pos);
      this.SetDirty(pos);
    }
    else
    {
      if ((double) this.m_goopedCells[pos].remainingLifespan < (double) this.goopDefinition.fadePeriod)
        this.SetDirty(pos);
      if (this.m_goopedCells[pos].IsOnFire && this.goopDefinition.ignitionChangesLifetime)
      {
        if ((double) this.m_goopedCells[pos].remainingLifespan > 0.0)
          this.m_goopedCells[pos].remainingLifespan = this.goopDefinition.ignitedLifetime;
      }
      else
      {
        if (!suppressSplashes && DeadlyDeadlyGoopManager.m_DoGoopSpawnSplashes && (this.m_goopedCells[pos].lastSourceID < 0 || this.m_goopedCells[pos].lastSourceID != sourceId) && (double) Random.value < 1.0 / 1000.0)
        {
          if ((Object) this.m_genericSplashPrefab == (Object) null)
            this.m_genericSplashPrefab = ResourceCache.Acquire("Global VFX/Generic_Goop_Splash") as GameObject;
          GameObject gameObject = SpawnManager.SpawnVFX(this.m_genericSplashPrefab, vector.ToVector3ZUp(vector.y), Quaternion.identity);
          gameObject.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
          gameObject.GetComponent<Renderer>().material.SetColor(DeadlyDeadlyGoopManager.TintColorPropertyID, (Color) this.goopDefinition.baseColor32);
        }
        this.m_goopedCells[pos].remainingLifespan = Mathf.Max(this.m_goopedCells[pos].remainingLifespan, this.goopDefinition.GetLifespan(radiusFraction));
        this.m_goopedCells[pos].lifespanOverridden = true;
        this.m_goopedCells[pos].HasPlayedFireOutro = false;
        this.m_goopedCells[pos].hasBeenFrozen = 0;
      }
      this.m_goopedCells[pos].lastSourceID = sourceId;
    }
  }

  public void TimedAddGoopArc(
    Vector2 origin,
    float radius,
    float arcDegrees,
    Vector2 direction,
    float duration = 0.5f,
    AnimationCurve goopCurve = null)
  {
    this.StartCoroutine(this.TimedAddGoopArc_CR(origin, radius, arcDegrees, direction, duration, goopCurve));
  }

  [DebuggerHidden]
  private IEnumerator TimedAddGoopArc_CR(
    Vector2 origin,
    float radius,
    float arcDegrees,
    Vector2 direction,
    float duration,
    AnimationCurve goopCurve)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DeadlyDeadlyGoopManager.\u003CTimedAddGoopArc_CR\u003Ec__Iterator1()
    {
      duration = duration,
      goopCurve = goopCurve,
      radius = radius,
      arcDegrees = arcDegrees,
      origin = origin,
      direction = direction,
      \u0024this = this
    };
  }

  public void TimedAddGoopCircle(
    Vector2 center,
    float radius,
    float duration = 0.5f,
    bool suppressSplashes = false)
  {
    this.StartCoroutine(this.TimedAddGoopCircle_CR(center, radius, duration, suppressSplashes));
  }

  [DebuggerHidden]
  private IEnumerator TimedAddGoopCircle_CR(
    Vector2 center,
    float radius,
    float duration,
    bool suppressSplashes = false)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DeadlyDeadlyGoopManager.\u003CTimedAddGoopCircle_CR\u003Ec__Iterator2()
    {
      duration = duration,
      radius = radius,
      center = center,
      suppressSplashes = suppressSplashes,
      \u0024this = this
    };
  }

  public void AddGoopCircle(
    Vector2 center,
    float radius,
    int sourceID = -1,
    bool suppressSplashes = false,
    int sourceFrameCount = -1)
  {
    int num1 = Mathf.FloorToInt((center.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((center.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((center.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((center.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 b = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    suppressSplashes |= (double) radius < 1.0;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 pos = new IntVector2(x, y);
        float num6 = Vector2.Distance(new Vector2((float) x, (float) y), b);
        if ((double) num6 < (double) num5)
        {
          float t = num6 / num5;
          if ((double) num6 < (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE * 2.0)
            t = 0.0f;
          float linearStepInterpolate = BraveMathCollege.SmoothStepToLinearStepInterpolate(0.0f, 1f, t);
          this.AddGoopedPosition(pos, linearStepInterpolate, suppressSplashes, sourceID, sourceFrameCount);
        }
      }
    }
  }

  public void AddGoopRing(
    Vector2 center,
    float minRadius,
    float maxRadius,
    int sourceID = -1,
    bool suppressSplashes = false,
    int sourceFrameCount = -1)
  {
    int num1 = Mathf.FloorToInt((center.x - maxRadius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((center.x + maxRadius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((center.y - maxRadius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((center.y + maxRadius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 b = center / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = minRadius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num6 = maxRadius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    suppressSplashes |= (double) num6 < 1.0;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 pos = new IntVector2(x, y);
        float num7 = Vector2.Distance(new Vector2((float) x, (float) y), b);
        if ((double) num7 >= (double) num5 && (double) num7 <= (double) num6)
        {
          float t = num7 / num6;
          if ((double) num7 < (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE * 2.0)
            t = 0.0f;
          float linearStepInterpolate = BraveMathCollege.SmoothStepToLinearStepInterpolate(0.0f, 1f, t);
          this.AddGoopedPosition(pos, linearStepInterpolate, suppressSplashes, sourceID, sourceFrameCount);
        }
      }
    }
  }

  public void TimedAddGoopLine(Vector2 p1, Vector2 p2, float radius, float duration)
  {
    this.StartCoroutine(this.TimedAddGoopLine_CR(p1, p2, radius, duration));
  }

  [DebuggerHidden]
  private IEnumerator TimedAddGoopLine_CR(Vector2 p1, Vector2 p2, float radius, float duration)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DeadlyDeadlyGoopManager.\u003CTimedAddGoopLine_CR\u003Ec__Iterator3()
    {
      p1 = p1,
      duration = duration,
      p2 = p2,
      radius = radius,
      \u0024this = this
    };
  }

  public void AddGoopLine(Vector2 p1, Vector2 p2, float radius)
  {
    Vector2 vector2_1 = Vector2.Min(p1, p2);
    Vector2 vector2_2 = Vector2.Max(p1, p2);
    int num1 = Mathf.FloorToInt((vector2_1.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((vector2_2.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((vector2_1.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((vector2_2.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    Vector2 vector2_3 = p1 / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    Vector2 vector2_4 = p2 / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num5 = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    float num6 = num5 * num5;
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        IntVector2 pos = new IntVector2(x, y);
        float num7 = (float) pos.x - vector2_3.x;
        float num8 = (float) pos.y - vector2_3.y;
        float num9 = vector2_4.x - vector2_3.x;
        float num10 = vector2_4.y - vector2_3.y;
        float num11 = (float) ((double) num7 * (double) num9 + (double) num8 * (double) num10);
        float num12 = (float) ((double) num9 * (double) num9 + (double) num10 * (double) num10);
        float num13 = -1f;
        if ((double) num12 != 0.0)
          num13 = num11 / num12;
        float num14;
        float num15;
        if ((double) num13 < 0.0)
        {
          num14 = vector2_3.x;
          num15 = vector2_3.y;
        }
        else if ((double) num13 > 1.0)
        {
          num14 = vector2_4.x;
          num15 = vector2_4.y;
        }
        else
        {
          num14 = vector2_3.x + num13 * num9;
          num15 = vector2_3.y + num13 * num10;
        }
        float sqrMagnitude = new Vector2((float) pos.x - num14, (float) pos.y - num15).sqrMagnitude;
        if ((double) sqrMagnitude < (double) num6)
        {
          float num16 = Mathf.Sqrt(sqrMagnitude);
          float t = num16 / num5;
          if ((double) num16 < (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE * 2.0)
            t = 0.0f;
          float linearStepInterpolate = BraveMathCollege.SmoothStepToLinearStepInterpolate(0.0f, 1f, t);
          this.AddGoopedPosition(pos, linearStepInterpolate);
        }
      }
    }
  }

  public void TimedAddGoopRect(Vector2 min, Vector2 max, float duration)
  {
    this.StartCoroutine(this.TimedAddGoopRect_CR(min, max, duration));
  }

  [DebuggerHidden]
  public IEnumerator TimedAddGoopRect_CR(Vector2 min, Vector2 max, float duration)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DeadlyDeadlyGoopManager.\u003CTimedAddGoopRect_CR\u003Ec__Iterator4()
    {
      duration = duration,
      min = min,
      max = max,
      \u0024this = this
    };
  }

  public void AddGoopRect(Vector2 min, Vector2 max)
  {
    int num1 = Mathf.FloorToInt(min.x / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt(max.x / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt(min.y / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt(max.y / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
        this.AddGoopedPosition(new IntVector2(x, y));
    }
  }

  public void AddGoopPoints(
    List<Vector2> points,
    float radius,
    Vector2 excludeCenter,
    float excludeRadius)
  {
    Vector2 lhs1 = Vector2Extensions.max;
    Vector2 lhs2 = Vector2Extensions.min;
    for (int index = 0; index < points.Count; ++index)
    {
      lhs1 = Vector2.Min(lhs1, points[index]);
      lhs2 = Vector2.Max(lhs2, points[index]);
    }
    int num1 = Mathf.FloorToInt((lhs1.x - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num2 = Mathf.CeilToInt((lhs2.x + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num3 = Mathf.FloorToInt((lhs1.y - radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int num4 = Mathf.CeilToInt((lhs2.y + radius) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    int width = num2 - num1 + 1;
    int height = num4 - num3 + 1;
    int num5 = Mathf.RoundToInt(radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    DeadlyDeadlyGoopManager.s_goopPointRadius = radius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE;
    DeadlyDeadlyGoopManager.s_goopPointRadiusSquare = DeadlyDeadlyGoopManager.s_goopPointRadius * DeadlyDeadlyGoopManager.s_goopPointRadius;
    DeadlyDeadlyGoopManager.m_pointsArray.ReinitializeWithDefault(width, height, false, 1f);
    for (int index = 0; index < points.Count; ++index)
    {
      DeadlyDeadlyGoopManager.s_goopPointCenter.x = (int) ((double) points[index].x / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) - num1;
      DeadlyDeadlyGoopManager.s_goopPointCenter.y = (int) ((double) points[index].y / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) - num3;
      BitArray2D pointsArray = DeadlyDeadlyGoopManager.m_pointsArray;
      int x = DeadlyDeadlyGoopManager.s_goopPointCenter.x;
      int y = DeadlyDeadlyGoopManager.s_goopPointCenter.y;
      int radius1 = num5;
      // ISSUE: reference to a compiler-generated field
      if (DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache0 = new SetBackingFloatFunc(DeadlyDeadlyGoopManager.GetRadiusFraction);
      }
      // ISSUE: reference to a compiler-generated field
      SetBackingFloatFunc fMgCache0 = DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache0;
      pointsArray.SetCircle(x, y, radius1, true, fMgCache0);
    }
    int num6 = (int) ((double) excludeCenter.x / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) - num1;
    int num7 = (int) ((double) excludeCenter.y / (double) DeadlyDeadlyGoopManager.GOOP_GRID_SIZE) - num3;
    int num8 = Mathf.RoundToInt(excludeRadius / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE);
    BitArray2D pointsArray1 = DeadlyDeadlyGoopManager.m_pointsArray;
    int x0 = num6;
    int y0 = num7;
    int radius2 = num8;
    // ISSUE: reference to a compiler-generated field
    if (DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache1 == null)
    {
      // ISSUE: reference to a compiler-generated field
      DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache1 = new SetBackingFloatFunc(DeadlyDeadlyGoopManager.GetRadiusFraction);
    }
    // ISSUE: reference to a compiler-generated field
    SetBackingFloatFunc fMgCache1 = DeadlyDeadlyGoopManager.\u003C\u003Ef__mg\u0024cache1;
    pointsArray1.SetCircle(x0, y0, radius2, false, fMgCache1);
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {
        if (DeadlyDeadlyGoopManager.m_pointsArray[x, y])
          this.AddGoopedPosition(new IntVector2(num1 + x, num3 + y), DeadlyDeadlyGoopManager.m_pointsArray.GetFloat(x, y));
      }
    }
  }

  private static float GetRadiusFraction(int x, int y, bool value, float currentFloatValue)
  {
    if (!value)
      return currentFloatValue;
    float num1 = (float) (DeadlyDeadlyGoopManager.s_goopPointCenter.x - x);
    float num2 = (float) (DeadlyDeadlyGoopManager.s_goopPointCenter.y - y);
    float f = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
    if ((double) f >= (double) DeadlyDeadlyGoopManager.s_goopPointRadiusSquare)
      return currentFloatValue;
    float num3 = Mathf.Sqrt(f);
    float t = num3 / DeadlyDeadlyGoopManager.s_goopPointRadius;
    if ((double) num3 < 0.5)
      t = 0.0f;
    return Mathf.Min(BraveMathCollege.SmoothStepToLinearStepInterpolate(0.0f, 1f, t), currentFloatValue);
  }

  private class GoopPositionData
  {
    public IntVector2 goopPosition;
    public DeadlyDeadlyGoopManager.GoopPositionData[] neighborGoopData;
    public bool IsOnFire;
    public bool IsElectrified;
    public bool IsFrozen;
    public bool HasPlayedFireIntro;
    public bool HasPlayedFireOutro;
    public bool lifespanOverridden;
    public int lastSourceID = -1;
    public int frameGooped = -1;
    public float remainingLifespan;
    public float remainingFreezeTimer;
    public int hasBeenFrozen;
    public bool unfrozeLastFrame;
    public bool eternal;
    public bool selfIgnites;
    public float remainingTimeTilSelfIgnition;
    public float remainingElectrifiedTime;
    public float remainingElecTimer;
    public uint elecTriggerSemaphore;
    public float remainingFireTimer;
    public float totalOnFireTime;
    public float totalFrozenTime;
    public int baseIndex = -1;
    public int NeighborsAsInt;
    public int NeighborsAsIntFuckDiagonals;
    public int GoopUpdateBin;

    public GoopPositionData(
      IntVector2 position,
      Dictionary<IntVector2, DeadlyDeadlyGoopManager.GoopPositionData> goopData,
      float lifespan)
    {
      this.goopPosition = position;
      this.neighborGoopData = new DeadlyDeadlyGoopManager.GoopPositionData[8];
      this.remainingLifespan = lifespan;
      IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
      for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
      {
        IntVector2 key = position + cardinalsAndOrdinals[index];
        DeadlyDeadlyGoopManager.GoopPositionData goopPositionData;
        if (goopData.TryGetValue(key, out goopPositionData))
        {
          goopPositionData.neighborGoopData[(index + 4) % 8] = this;
          this.neighborGoopData[index] = goopData[key];
          goopPositionData.SetNeighborGoop((index + 4) % 8, true);
          this.SetNeighborGoop(index, true);
        }
      }
      this.GoopUpdateBin = Random.Range(0, 4);
    }

    public bool SupportsAmbientVFX
    {
      get => this.NeighborsAsInt == (int) byte.MaxValue && (double) this.remainingLifespan > 2.0;
    }

    public bool HasOnFireNeighbor
    {
      get
      {
        for (int index = 0; index < 8; ++index)
        {
          if (this.neighborGoopData[index] != null && this.neighborGoopData[index].IsOnFire)
            return true;
        }
        return false;
      }
    }

    public bool HasFrozenNeighbor
    {
      get
      {
        for (int index = 0; index < 8; ++index)
        {
          if (this.neighborGoopData[index] != null && this.neighborGoopData[index].IsFrozen)
            return true;
        }
        return false;
      }
    }

    public bool HasNonFrozenNeighbor
    {
      get
      {
        for (int index = 0; index < 8; ++index)
        {
          if (this.neighborGoopData[index] == null || !this.neighborGoopData[index].IsFrozen && !this.neighborGoopData[index].unfrozeLastFrame)
            return true;
        }
        return false;
      }
    }

    public void SetNeighborGoop(int index, bool value)
    {
      int num = 1 << 7 - index;
      if (value)
        this.NeighborsAsInt |= num;
      else
        this.NeighborsAsInt &= ~num;
      this.NeighborsAsIntFuckDiagonals = this.NeighborsAsInt & 170;
    }
  }

  private enum VertexColorRebuildResult
  {
    ALL_OK,
    ELECTRIFIED,
  }
}
