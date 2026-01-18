using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class TileVFXManager : MonoBehaviour
  {
    private static TileVFXManager m_instance;
    private List<IntVector2> m_registeredCells = new List<IntVector2>();
    private List<TilesetIndexMetadata> m_registeredMetadata = new List<TilesetIndexMetadata>();
    private List<RuntimeTileVFXData> m_runtimeData = new List<RuntimeTileVFXData>();
    private Vector2 m_frameCameraPosition;

    public static TileVFXManager Instance
    {
      get
      {
        if ((Object) TileVFXManager.m_instance == (Object) null)
          TileVFXManager.m_instance = GameManager.Instance.Dungeon.gameObject.GetOrAddComponent<TileVFXManager>();
        return TileVFXManager.m_instance;
      }
      set => TileVFXManager.m_instance = value;
    }

    public void RegisterCellVFX(IntVector2 cellPosition, TilesetIndexMetadata metadata)
    {
      if (this.m_registeredCells.Contains(cellPosition))
      {
        Debug.Log((object) "registering a cell twice!!!!!!");
      }
      else
      {
        this.m_registeredCells.Add(cellPosition);
        this.m_registeredMetadata.Add(metadata);
        RuntimeTileVFXData runtimeTileVfxData = new RuntimeTileVFXData();
        if (metadata.tileVFXPlaystyle == TilesetIndexMetadata.VFXPlaystyle.TIMED_REPEAT)
          runtimeTileVfxData.cooldownRemaining = Random.Range(0.0f, Random.Range(metadata.tileVFXDelayTime - metadata.tileVFXDelayVariance, metadata.tileVFXDelayTime + metadata.tileVFXDelayVariance));
        this.m_runtimeData.Add(runtimeTileVfxData);
      }
    }

    private void CreateVFX(
      IntVector2 cellPosition,
      TilesetIndexMetadata cellMetadata,
      RuntimeTileVFXData runtimeData,
      bool ignoreCulling = false)
    {
      Vector3 vector3Zup = (cellPosition.ToVector2() + cellMetadata.tileVFXOffset).ToVector3ZUp();
      vector3Zup.z = vector3Zup.y;
      if (ignoreCulling)
      {
        SpawnManager.SpawnVFX(cellMetadata.tileVFXPrefab, vector3Zup, Quaternion.identity);
      }
      else
      {
        Vector2 vector2 = this.m_frameCameraPosition - vector3Zup.XY();
        vector2.y *= 1.7f;
        if ((double) vector2.sqrMagnitude > 420.0)
          return;
        SpawnManager.SpawnVFX(cellMetadata.tileVFXPrefab, vector3Zup, Quaternion.identity);
      }
    }

    private void Update()
    {
      if ((double) UnityEngine.Time.timeScale == 0.0)
        return;
      this.m_frameCameraPosition = GameManager.Instance.MainCameraController.transform.PositionVector2();
      if (this.m_registeredCells.Count != this.m_registeredMetadata.Count || this.m_registeredCells.Count != this.m_runtimeData.Count)
      {
        Debug.LogError((object) "MISMATCH IN TILE VFX MANAGER, THIS IS NOT GOOD.");
      }
      else
      {
        for (int index1 = 0; index1 < this.m_registeredCells.Count; ++index1)
        {
          IntVector2 registeredCell = this.m_registeredCells[index1];
          TilesetIndexMetadata cellMetadata = this.m_registeredMetadata[index1];
          RuntimeTileVFXData runtimeData = this.m_runtimeData[index1];
          if (cellMetadata.tileVFXPlaystyle == TilesetIndexMetadata.VFXPlaystyle.CONTINUOUS)
          {
            if (!runtimeData.vfxHasEverBeenInstantiated)
            {
              this.CreateVFX(registeredCell, cellMetadata, runtimeData, true);
              runtimeData.vfxHasEverBeenInstantiated = true;
            }
          }
          else if (cellMetadata.tileVFXPlaystyle == TilesetIndexMetadata.VFXPlaystyle.TIMED_REPEAT)
          {
            runtimeData.cooldownRemaining = Mathf.Max(0.0f, runtimeData.cooldownRemaining - BraveTime.DeltaTime);
            if ((double) runtimeData.cooldownRemaining <= 0.0)
            {
              this.CreateVFX(registeredCell, cellMetadata, runtimeData);
              runtimeData.vfxHasEverBeenInstantiated = true;
              runtimeData.cooldownRemaining = Random.Range(cellMetadata.tileVFXDelayTime - cellMetadata.tileVFXDelayVariance, cellMetadata.tileVFXDelayTime + cellMetadata.tileVFXDelayVariance);
            }
          }
          else if (cellMetadata.tileVFXPlaystyle == TilesetIndexMetadata.VFXPlaystyle.ON_ANIMATION_FRAME && TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(registeredCell))
          {
            for (int index2 = 0; index2 < TK2DTilemapChunkAnimator.PositionToAnimatorMap[registeredCell].Count; ++index2)
            {
              if (TK2DTilemapChunkAnimator.PositionToAnimatorMap[registeredCell][index2].associatedMetadata == cellMetadata)
              {
                if (TK2DTilemapChunkAnimator.PositionToAnimatorMap[registeredCell][index2].CurrentFrame == cellMetadata.tileVFXAnimFrame)
                {
                  if (!runtimeData.vfxHasEverBeenInstantiated)
                  {
                    this.CreateVFX(registeredCell, cellMetadata, runtimeData);
                    runtimeData.vfxHasEverBeenInstantiated = true;
                  }
                }
                else
                  runtimeData.vfxHasEverBeenInstantiated = false;
              }
            }
          }
          this.m_runtimeData[index1] = runtimeData;
        }
      }
    }
  }

