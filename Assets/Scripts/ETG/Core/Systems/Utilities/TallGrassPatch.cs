using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class TallGrassPatch : MonoBehaviour
  {
    [NonSerialized]
    public List<IntVector2> cells;
    private const int INDEX_TOP = 124;
    private const int INDEX_MIDDLE = 147;
    private const int INDEX_MIDDLE_BOTTOM = 146;
    private const int INDEX_BOTTOM = 168;
    private Dictionary<IntVector2, TallGrassPatch.EnflamedGrassData> m_fireData = new Dictionary<IntVector2, TallGrassPatch.EnflamedGrassData>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
    private ParticleSystem m_fireSystem;
    private ParticleSystem m_fireIntroSystem;
    private ParticleSystem m_fireOutroSystem;
    private List<tk2dTiledSprite> m_tiledSpritePool = new List<tk2dTiledSprite>();
    private bool m_isPlayingFireAudio;
    private GameObject m_stripPrefab;

    private void InitializeParticleSystem()
    {
      if ((UnityEngine.Object) this.m_fireSystem != (UnityEngine.Object) null)
        return;
      GameObject gameObject1 = GameObject.Find("Gungeon_Fire_Main");
      if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
      {
        gameObject1 = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Particles/Gungeon_Fire_Main_raw"), Vector3.zero, Quaternion.identity);
        gameObject1.name = "Gungeon_Fire_Main";
      }
      this.m_fireSystem = gameObject1.GetComponent<ParticleSystem>();
      GameObject gameObject2 = GameObject.Find("Gungeon_Fire_Intro");
      if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
      {
        gameObject2 = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Particles/Gungeon_Fire_Intro_raw"), Vector3.zero, Quaternion.identity);
        gameObject2.name = "Gungeon_Fire_Intro";
      }
      this.m_fireIntroSystem = gameObject2.GetComponent<ParticleSystem>();
      GameObject gameObject3 = GameObject.Find("Gungeon_Fire_Outro");
      if ((UnityEngine.Object) gameObject3 == (UnityEngine.Object) null)
      {
        gameObject3 = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Particles/Gungeon_Fire_Outro_raw"), Vector3.zero, Quaternion.identity);
        gameObject3.name = "Gungeon_Fire_Outro";
      }
      this.m_fireOutroSystem = gameObject3.GetComponent<ParticleSystem>();
    }

    private int GetTargetIndexForPosition(IntVector2 current)
    {
      bool flag1 = this.cells.Contains(current + IntVector2.North);
      bool flag2 = this.cells.Contains(current + IntVector2.South);
      bool flag3 = this.cells.Contains(current + IntVector2.South + IntVector2.South);
      return !flag1 || !flag2 || !flag3 ? (!flag1 || !flag2 ? (!flag1 || flag2 ? (flag1 || !flag2 ? 168 : 124) : 168) : 146) : 147;
    }

    public void IgniteCircle(Vector2 center, float radius)
    {
      for (int x = Mathf.FloorToInt(center.x - radius); x < Mathf.CeilToInt(center.x + radius); ++x)
      {
        for (int y = Mathf.FloorToInt(center.y - radius); y < Mathf.CeilToInt(center.y + radius); ++y)
        {
          if ((double) Vector2.Distance(new Vector2((float) x, (float) y), center) < (double) radius)
            this.IgniteCell(new IntVector2(x, y));
        }
      }
    }

    public void IgniteCell(IntVector2 cellPosition)
    {
      if (!this.cells.Contains(cellPosition) || this.m_fireData.ContainsKey(cellPosition))
        return;
      TallGrassPatch.EnflamedGrassData enflamedGrassData = new TallGrassPatch.EnflamedGrassData();
      this.m_fireData.Add(cellPosition, enflamedGrassData);
    }

    private TallGrassPatch.EnflamedGrassData DoParticleAtPosition(
      IntVector2 worldPos,
      TallGrassPatch.EnflamedGrassData fireData)
    {
      if ((UnityEngine.Object) this.m_fireSystem != (UnityEngine.Object) null && (double) fireData.ParticleTimer <= 0.0)
      {
        bool flag = this.cells.Contains(worldPos + IntVector2.South);
        for (int index1 = 0; index1 < 2; ++index1)
        {
          for (int index2 = 0; index2 < 2; ++index2)
          {
            if (flag || index2 != 0)
            {
              float num1 = UnityEngine.Random.Range(1f, 1.5f);
              float num2 = UnityEngine.Random.Range(0.75f, 1f);
              Vector2 vector2 = (Vector2) (worldPos.ToVector3() + new Vector3((float) (0.33000001311302185 + 0.33000001311302185 * (double) index1), (float) (0.33000001311302185 + 0.33000001311302185 * (double) index2), 0.0f)) + UnityEngine.Random.insideUnitCircle / 5f;
              ParticleSystem.EmitParams emitParams;
              if (!fireData.HasPlayedFireOutro)
              {
                if (!fireData.HasPlayedFireOutro && (double) fireData.fireTime > 3.0 && (UnityEngine.Object) this.m_fireOutroSystem != (UnityEngine.Object) null)
                {
                  num1 = num2;
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = (Vector3) vector2;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num2;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) UnityEngine.Random.value * 4294967296.0);
                  this.m_fireOutroSystem.Emit(emitParams, 1);
                  if (index1 == 1 && index2 == 1)
                    fireData.HasPlayedFireOutro = true;
                }
                else if (!fireData.HasPlayedFireIntro && (UnityEngine.Object) this.m_fireIntroSystem != (UnityEngine.Object) null)
                {
                  num1 = UnityEngine.Random.Range(0.75f, 1f);
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = (Vector3) vector2;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num1;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) UnityEngine.Random.value * 4294967296.0);
                  this.m_fireIntroSystem.Emit(emitParams, 1);
                  if (index1 == 1 && index2 == 1)
                    fireData.HasPlayedFireIntro = true;
                }
                else if ((double) UnityEngine.Random.value < 0.5)
                {
                  emitParams = new ParticleSystem.EmitParams();
                  emitParams.position = (Vector3) vector2;
                  emitParams.velocity = Vector3.zero;
                  emitParams.startSize = this.m_fireSystem.startSize;
                  emitParams.startLifetime = num1;
                  emitParams.startColor = (Color32) this.m_fireSystem.startColor;
                  emitParams.randomSeed = (uint) ((double) UnityEngine.Random.value * 4294967296.0);
                  this.m_fireSystem.Emit(emitParams, 1);
                }
              }
              if (index1 == 1 && index2 == 1)
                fireData.ParticleTimer = num1 - 0.125f;
            }
          }
        }
      }
      return fireData;
    }

    private void LateUpdate()
    {
      bool flag = false;
      for (int index = 0; index < this.cells.Count; ++index)
      {
        if (this.m_fireData.ContainsKey(this.cells[index]))
        {
          TallGrassPatch.EnflamedGrassData fireData = this.m_fireData[this.cells[index]];
          fireData.fireTime += BraveTime.DeltaTime;
          fireData.ParticleTimer -= BraveTime.DeltaTime;
          if (!this.m_fireData[this.cells[index]].hasEnflamedNeighbors && (double) this.m_fireData[this.cells[index]].fireTime > 0.10000000149011612)
          {
            this.IgniteCell(this.cells[index] + IntVector2.North);
            this.IgniteCell(this.cells[index] + IntVector2.East);
            this.IgniteCell(this.cells[index] + IntVector2.South);
            this.IgniteCell(this.cells[index] + IntVector2.West);
            fireData.hasEnflamedNeighbors = true;
          }
          if (fireData.HasPlayedFireOutro && (double) fireData.ParticleTimer <= 0.0)
          {
            this.RemovePosition(this.cells[index]);
            --index;
          }
          else
          {
            fireData = this.DoParticleAtPosition(this.cells[index], fireData);
            this.m_fireData[this.cells[index]] = fireData;
          }
        }
      }
      if (!flag || this.m_isPlayingFireAudio)
        return;
      this.m_isPlayingFireAudio = true;
      int num = (int) AkSoundEngine.PostEvent("Play_ENV_oilfire_ignite_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }

    private void RemovePosition(IntVector2 pos)
    {
      if (!this.cells.Contains(pos))
        return;
      this.cells.Remove(pos);
      this.BuildPatch();
    }

    public void BuildPatch()
    {
      for (int index = 0; index < this.m_tiledSpritePool.Count; index = index - 1 + 1)
      {
        SpawnManager.Despawn(this.m_tiledSpritePool[index].gameObject);
        this.m_tiledSpritePool.RemoveAt(index);
      }
      if ((UnityEngine.Object) this.m_stripPrefab == (UnityEngine.Object) null)
        this.m_stripPrefab = (GameObject) BraveResources.Load("Global Prefabs/TallGrassStrip");
      HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
      for (int index = 0; index < this.cells.Count; ++index)
      {
        IntVector2 cell = this.cells[index];
        if (!intVector2Set.Contains(cell))
        {
          intVector2Set.Add(cell);
          int num = 1;
          int indexForPosition = this.GetTargetIndexForPosition(cell);
          IntVector2 current = cell;
          while (true)
          {
            current += IntVector2.Right;
            if (!intVector2Set.Contains(current) && this.cells.Contains(current) && indexForPosition == this.GetTargetIndexForPosition(current))
            {
              ++num;
              intVector2Set.Add(current);
            }
            else
              break;
          }
          GameObject gameObject = SpawnManager.SpawnVFX(this.m_stripPrefab);
          tk2dTiledSprite component = gameObject.GetComponent<tk2dTiledSprite>();
          component.SetSprite(GameManager.Instance.Dungeon.tileIndices.dungeonCollection, indexForPosition);
          component.dimensions = new Vector2((float) (16 /*0x10*/ * num), 16f);
          gameObject.transform.position = new Vector3((float) cell.x, (float) cell.y, (float) cell.y);
          this.m_tiledSpritePool.Add(component);
          switch (indexForPosition)
          {
            case 124:
              component.IsPerpendicular = true;
              break;
            case 168:
              component.HeightOffGround = -2f;
              component.IsPerpendicular = true;
              component.transform.position += new Vector3(0.0f, 11f / 16f, 0.0f);
              break;
            default:
              component.IsPerpendicular = false;
              break;
          }
          component.UpdateZDepth();
        }
      }
      if (!StaticReferenceManager.AllGrasses.Contains(this))
        StaticReferenceManager.AllGrasses.Add(this);
      this.InitializeParticleSystem();
    }

    internal struct EnflamedGrassData
    {
      public float fireTime;
      public bool hasEnflamedNeighbors;
      public bool HasPlayedFireOutro;
      public bool HasPlayedFireIntro;
      public float ParticleTimer;
    }
  }

