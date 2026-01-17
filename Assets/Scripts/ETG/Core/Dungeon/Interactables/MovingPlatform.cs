// Decompiled with JetBrains decompiler
// Type: MovingPlatform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class MovingPlatform : DungeonPlaceableBehaviour, IDwarfDrawable, IPlaceConfigurable
    {
      public IntVector2 Size;
      [DwarfConfigurable]
      public bool UsesDwarfConfigurableSize;
      [DwarfConfigurable]
      public float DwarfConfigurableWidth = 3f;
      [DwarfConfigurable]
      public float DwarfConfigurableHeight = 3f;
      public Transform StencilQuad;
      public bool StaticForPitfall;
      public bool AllowsForGoop;
      private tk2dSlicedSprite m_sprite;
      private RoomHandler m_room;

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MovingPlatform.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      public void ForceUpdateSize()
      {
        tk2dSlicedSprite component = this.GetComponent<tk2dSlicedSprite>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.dimensions = new Vector2((float) (16 /*0x10*/ * this.Size.x), (float) (16 /*0x10*/ * this.Size.y));
        PixelCollider pixelCollider = this.specRigidbody.PixelColliders.Find((Predicate<PixelCollider>) (c => c.CollisionLayer == CollisionLayer.MovingPlatform));
        if (pixelCollider == null)
        {
          pixelCollider = new PixelCollider();
          pixelCollider.CollisionLayer = CollisionLayer.MovingPlatform;
          this.specRigidbody.PixelColliders.Add(pixelCollider);
        }
        pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
        pixelCollider.ManualOffsetX = 0;
        pixelCollider.ManualOffsetY = 0;
        pixelCollider.ManualWidth = this.Size.x * 16 /*0x10*/;
        pixelCollider.ManualHeight = this.Size.y * 16 /*0x10*/;
        pixelCollider.Regenerate(this.specRigidbody.transform);
        this.specRigidbody.Reinitialize();
        if (!((UnityEngine.Object) this.StencilQuad != (UnityEngine.Object) null))
          return;
        float x = component.dimensions.x / 16f;
        float y = (float) (((double) component.dimensions.y + 7.0) / 16.0);
        float num = 7f / 16f;
        this.StencilQuad.localScale = new Vector3(x, y, 1f);
        this.StencilQuad.transform.localPosition = new Vector3(x / 2f, y / 2f - num, 0.0f);
      }

      protected override void OnDestroy()
      {
        if (this.m_room != null && (bool) (UnityEngine.Object) this.specRigidbody && this.m_room.RoomMovingPlatforms != null)
          this.m_room.RoomMovingPlatforms.Remove(this.specRigidbody);
        base.OnDestroy();
      }

      private void OnEnterTrigger(
        SpeculativeRigidbody obj,
        SpeculativeRigidbody source,
        CollisionData collisionData)
      {
        if ((bool) (UnityEngine.Object) obj.gameActor && !obj.gameActor.SupportingPlatforms.Contains(this))
          obj.gameActor.SupportingPlatforms.Add(this);
        this.specRigidbody.RegisterCarriedRigidbody(obj);
      }

      private void OnTriggerCollision(
        SpeculativeRigidbody obj,
        SpeculativeRigidbody source,
        CollisionData collisionData)
      {
      }

      private void OnExitTrigger(SpeculativeRigidbody obj, SpeculativeRigidbody source)
      {
        if ((bool) (UnityEngine.Object) obj && (bool) (UnityEngine.Object) obj.gameActor)
          obj.gameActor.SupportingPlatforms.Remove(this);
        if (!(bool) (UnityEngine.Object) this)
          return;
        this.specRigidbody.DeregisterCarriedRigidbody(obj);
      }

      public void ClearCells()
      {
        PixelCollider primaryPixelCollider = this.specRigidbody.PrimaryPixelCollider;
        IntVector2 intVector2_1 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
        for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
        {
          for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
          {
            GameManager.Instance.Dungeon.data.cellData[x][y].platforms?.Remove(this.specRigidbody);
            if (this.AllowsForGoop)
            {
              DeadlyDeadlyGoopManager.ForceClearGoopsInCell(new IntVector2(x, y));
              GameManager.Instance.Dungeon.data.cellData[x][y].forceAllowGoop = false;
            }
          }
        }
      }

      public void MarkCells()
      {
        PixelCollider primaryPixelCollider = this.specRigidbody.PrimaryPixelCollider;
        IntVector2 intVector2_1 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
        for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
        {
          for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
          {
            if (GameManager.Instance.Dungeon.data.cellData[x][y] != null)
            {
              List<SpeculativeRigidbody> speculativeRigidbodyList = GameManager.Instance.Dungeon.data.cellData[x][y].platforms;
              if (speculativeRigidbodyList == null)
              {
                speculativeRigidbodyList = new List<SpeculativeRigidbody>();
                GameManager.Instance.Dungeon.data.cellData[x][y].platforms = speculativeRigidbodyList;
              }
              if (!speculativeRigidbodyList.Contains(this.specRigidbody))
                speculativeRigidbodyList.Add(this.specRigidbody);
              if (this.AllowsForGoop)
                GameManager.Instance.Dungeon.data.cellData[x][y].forceAllowGoop = true;
            }
          }
        }
      }

      private void OnPostRigidbodyMovement(
        SpeculativeRigidbody specRigidbody,
        Vector2 unitDelta,
        IntVector2 pixelDelta)
      {
        if (pixelDelta == IntVector2.Zero)
          return;
        PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
        IntVector2 intVector2_1 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft - pixelDelta).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_3 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight - pixelDelta).ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_4 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
        Dungeon dungeon = GameManager.Instance.Dungeon;
        if (intVector2_1 != intVector2_2 || intVector2_3 != intVector2_4)
        {
          for (int x = intVector2_1.x; x <= intVector2_3.x; ++x)
          {
            for (int y = intVector2_1.y; y <= intVector2_3.y; ++y)
            {
              if (dungeon.CellExists(x, y) && dungeon.data[x, y] != null)
                dungeon.data[x, y].platforms?.Remove(specRigidbody);
            }
          }
          for (int x = intVector2_2.x; x <= intVector2_4.x; ++x)
          {
            for (int y = intVector2_2.y; y <= intVector2_4.y; ++y)
            {
              if (dungeon.CellExists(x, y) && dungeon.data[x, y] != null)
              {
                List<SpeculativeRigidbody> speculativeRigidbodyList = dungeon.data[x, y].platforms;
                if (speculativeRigidbodyList == null)
                {
                  speculativeRigidbodyList = new List<SpeculativeRigidbody>();
                  GameManager.Instance.Dungeon.data.cellData[x][y].platforms = speculativeRigidbodyList;
                }
                if (!speculativeRigidbodyList.Contains(specRigidbody))
                  speculativeRigidbodyList.Add(specRigidbody);
              }
            }
          }
        }
        if (!((UnityEngine.Object) this.m_sprite != (UnityEngine.Object) null))
          return;
        this.m_sprite.UpdateZDepth();
      }

      public IntVector2 GetOverrideDwarfDimensions(PrototypePlacedObjectData objectData)
      {
        return objectData.GetBoolFieldValueByName("UsesDwarfConfigurableSize") ? new IntVector2((int) objectData.GetFieldValueByName("DwarfConfigurableWidth"), (int) objectData.GetFieldValueByName("DwarfConfigurableHeight")) : new IntVector2(this.placeableWidth, this.placeableHeight);
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_room = room;
        if (room.RoomMovingPlatforms == null || !(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        room.RoomMovingPlatforms.Add(this.specRigidbody);
      }
    }

}
