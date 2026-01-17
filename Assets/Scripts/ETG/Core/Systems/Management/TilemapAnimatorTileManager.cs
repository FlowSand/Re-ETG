// Decompiled with JetBrains decompiler
// Type: TilemapAnimatorTileManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class TilemapAnimatorTileManager
    {
      public tk2dSpriteCollectionData associatedCollection;
      public int associatedSpriteId;
      public TilesetIndexMetadata associatedMetadata;
      public SimpleTilesetAnimationSequence associatedSequence;
      public SimpleTilesetAnimationSequence loopceptionSequence;
      private bool m_isLoopcepting;
      public IntVector2 worldPosition;
      public TK2DTilemapChunkAnimator animator;
      public List<TilemapAnimatorTileManager> linkedManagers = new List<TilemapAnimatorTileManager>();
      public int startUVIndex;
      public int uvCount;
      private float m_delayRemaining;
      private float m_elapsed;
      private float m_cachedSequenceLength;
      private float m_cachedLoopceptionLength;
      private int m_lastTargetEntry;
      private bool m_triggered;
      private bool m_forceNextUpdate;
      private int m_loopceptionLoopsRemaining;

      public TilemapAnimatorTileManager(
        tk2dSpriteCollectionData sourceCollection,
        int sourceSpriteId,
        TilesetIndexMetadata metadata,
        int uvStart,
        int numUV,
        tk2dTileMap tilemap)
      {
        this.associatedCollection = sourceCollection;
        this.associatedSpriteId = sourceSpriteId;
        this.associatedMetadata = metadata;
        this.associatedSequence = this.associatedCollection.GetAnimationSequence(this.associatedSpriteId);
        if (this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.LOOPCEPTION)
          this.loopceptionSequence = tilemap.SpriteCollectionInst.GetAnimationSequence(this.associatedSequence.loopceptionTarget);
        this.startUVIndex = uvStart;
        this.uvCount = numUV;
        this.m_elapsed = 0.0f;
        this.m_cachedSequenceLength = 0.0f;
        for (int index = 0; index < this.associatedSequence.entries.Count; ++index)
          this.m_cachedSequenceLength += this.associatedSequence.entries[index].frameTime;
        if (this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.LOOPCEPTION)
        {
          for (int index = 0; index < this.loopceptionSequence.entries.Count; ++index)
            this.m_cachedLoopceptionLength += this.loopceptionSequence.entries[index].frameTime;
        }
        if (!this.associatedSequence.randomStartFrame)
          return;
        this.m_elapsed = Mathf.Lerp(0.0f, this.m_cachedSequenceLength, Random.value);
      }

      public int CurrentFrame => this.m_lastTargetEntry;

      protected void UpdateChildSectionInternal(
        Vector2[] refMeshUVs,
        tk2dTileMap tileMap,
        int targetEntryIndex)
      {
        SimpleTilesetAnimationSequence animationSequence = !this.m_isLoopcepting ? this.associatedSequence : this.loopceptionSequence;
        this.m_lastTargetEntry = targetEntryIndex;
        SimpleTilesetAnimationSequenceEntry entry = animationSequence.entries[targetEntryIndex];
        tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[entry.entryIndex];
        for (int startUvIndex = this.startUVIndex; startUvIndex < this.startUVIndex + this.uvCount; ++startUvIndex)
          refMeshUVs[startUvIndex] = spriteDefinition.uvs[startUvIndex - this.startUVIndex];
      }

      public void TriggerAnimationSequence()
      {
        if (this.m_triggered)
          return;
        this.m_elapsed = 0.0f;
        this.m_triggered = true;
      }

      public void UntriggerAnimationSequence()
      {
        if (!this.m_triggered)
          return;
        this.m_elapsed = 0.0f;
        this.m_triggered = false;
        this.m_forceNextUpdate = true;
      }

      public bool UpdateRelevantSection(
        ref Vector2[] refMeshUVs,
        Mesh refMesh,
        tk2dTileMap tileMap,
        float deltaTime)
      {
        if (!this.m_forceNextUpdate && (this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.TRIGGERED_ONCE && !this.m_triggered || this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.TRIGGERED_ONCE && (double) this.m_elapsed > (double) this.m_cachedSequenceLength))
          return false;
        this.m_forceNextUpdate = false;
        float num1 = deltaTime;
        if ((double) this.m_delayRemaining > 0.0)
        {
          if ((double) deltaTime <= (double) this.m_delayRemaining)
          {
            this.m_delayRemaining -= deltaTime;
            return false;
          }
          num1 = deltaTime - this.m_delayRemaining;
          this.m_delayRemaining = 0.0f;
        }
        if (this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.TRIGGERED_ONCE && !this.m_triggered)
          this.m_elapsed = 0.0f;
        else
          this.m_elapsed += num1;
        if ((double) this.m_elapsed >= (double) this.m_cachedSequenceLength && this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.DELAYED_LOOP)
        {
          this.m_delayRemaining = Mathf.Lerp(this.associatedSequence.loopDelayMin, this.associatedSequence.loopDelayMax, Random.value) - this.m_elapsed % this.m_cachedSequenceLength;
          this.m_elapsed = 0.0f;
          return false;
        }
        if (this.associatedSequence.playstyle == SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.LOOPCEPTION)
        {
          if (!this.m_isLoopcepting && (double) this.m_elapsed >= (double) this.m_cachedSequenceLength)
          {
            if (this.m_loopceptionLoopsRemaining > 0)
            {
              --this.m_loopceptionLoopsRemaining;
              this.m_elapsed %= this.m_cachedSequenceLength;
            }
            else
            {
              this.m_isLoopcepting = true;
              this.m_elapsed %= this.m_cachedSequenceLength;
              this.m_loopceptionLoopsRemaining = Random.Range(this.associatedSequence.loopceptionMin, this.associatedSequence.loopceptionMax);
            }
          }
          else if (this.m_isLoopcepting && (double) this.m_elapsed >= (double) this.m_cachedLoopceptionLength)
          {
            if (this.m_loopceptionLoopsRemaining > 0)
            {
              --this.m_loopceptionLoopsRemaining;
              this.m_elapsed %= this.m_cachedLoopceptionLength;
            }
            else
            {
              this.m_isLoopcepting = false;
              this.m_elapsed %= this.m_cachedLoopceptionLength;
              this.m_loopceptionLoopsRemaining = Random.Range(this.associatedSequence.coreceptionMin, this.associatedSequence.coreceptionMax);
            }
          }
        }
        else if (this.associatedSequence.playstyle != SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.TRIGGERED_ONCE)
          this.m_elapsed %= this.m_cachedSequenceLength;
        this.m_elapsed = Mathf.Clamp(this.m_elapsed, 0.0f, this.m_cachedSequenceLength);
        SimpleTilesetAnimationSequence animationSequence = !this.m_isLoopcepting ? this.associatedSequence : this.loopceptionSequence;
        float num2 = 0.0f;
        int num3;
        for (num3 = 0; num3 < animationSequence.entries.Count; ++num3)
        {
          num2 += animationSequence.entries[num3].frameTime;
          if ((double) num2 >= (double) this.m_elapsed)
            break;
        }
        if (num3 == this.m_lastTargetEntry)
          return false;
        this.m_lastTargetEntry = num3;
        SimpleTilesetAnimationSequenceEntry entry = animationSequence.entries[num3];
        tk2dSpriteDefinition spriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[entry.entryIndex];
        for (int startUvIndex = this.startUVIndex; startUvIndex < this.startUVIndex + this.uvCount; ++startUvIndex)
          refMeshUVs[startUvIndex] = spriteDefinition.uvs[startUvIndex - this.startUVIndex];
        for (int index = 0; index < this.linkedManagers.Count; ++index)
          this.linkedManagers[index].UpdateChildSectionInternal(refMeshUVs, tileMap, num3);
        return true;
      }
    }

}
