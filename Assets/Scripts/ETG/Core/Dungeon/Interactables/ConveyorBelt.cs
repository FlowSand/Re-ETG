// Decompiled with JetBrains decompiler
// Type: ConveyorBelt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ConveyorBelt : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
      [DwarfConfigurable]
      public float ConveyorWidth = 4f;
      [DwarfConfigurable]
      public float ConveyorHeight = 3f;
      [DwarfConfigurable]
      public float VelocityX;
      [DwarfConfigurable]
      public float VelocityY;
      public bool IsHorizontal;
      public List<tk2dBaseSprite> ShadowObjects;
      public List<tk2dSpriteAnimator> ModuleAnimators;
      public List<string> NegativeVelocityAnims;
      public List<string> PositiveVelocityAnims;
      private Vector2 Velocity;
      private List<SpeculativeRigidbody> m_rigidbodiesOnPlatform = new List<SpeculativeRigidbody>();

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ConveyorBelt.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      public void Update()
      {
        for (int index = 0; index < this.ModuleAnimators.Count; ++index)
        {
          tk2dSpriteAnimator moduleAnimator = this.ModuleAnimators[index];
          string positiveVelocityAnim = this.PositiveVelocityAnims[index];
          string negativeVelocityAnim = this.NegativeVelocityAnims[index];
          if ((bool) (Object) moduleAnimator)
          {
            if (moduleAnimator.CurrentClip != null && moduleAnimator.CurrentClip.frames != null && moduleAnimator.CurrentClip.frames.Length > 0)
            {
              float num = this.Velocity.magnitude / 8f / (float) (1.0 / ((double) (moduleAnimator.CurrentClip.frames.Length * 2) / (double) moduleAnimator.CurrentClip.fps));
              moduleAnimator.ClipFps = moduleAnimator.CurrentClip.fps * num;
            }
            if ((double) this.Velocity.x != 0.0)
            {
              if ((double) this.Velocity.x > 0.0 && !moduleAnimator.IsPlaying(positiveVelocityAnim))
                moduleAnimator.Play(positiveVelocityAnim);
              else if ((double) this.Velocity.x < 0.0 && !moduleAnimator.IsPlaying(negativeVelocityAnim))
                moduleAnimator.Play(negativeVelocityAnim);
            }
            else if ((double) this.Velocity.y > 0.0 && !moduleAnimator.IsPlaying(positiveVelocityAnim))
              moduleAnimator.Play(positiveVelocityAnim);
            else if ((double) this.Velocity.y < 0.0 && !moduleAnimator.IsPlaying(negativeVelocityAnim))
              moduleAnimator.Play(negativeVelocityAnim);
          }
        }
        tk2dSpriteAnimator moduleAnimator1 = this.ModuleAnimators[0];
        int clipTime = (int) moduleAnimator1.clipTime;
        int num1 = ((int) ((double) moduleAnimator1.clipTime + (double) moduleAnimator1.ClipFps * (double) BraveTime.DeltaTime) - clipTime) * 2;
        IntVector2 intVector2 = IntVector2.Zero;
        if ((double) this.Velocity.x != 0.0)
          intVector2 = new IntVector2((double) this.Velocity.x <= 0.0 ? -num1 : num1, 0);
        else if ((double) this.Velocity.y != 0.0)
          intVector2 = new IntVector2(0, (double) this.Velocity.y <= 0.0 ? -num1 : num1);
        for (int index = 0; index < this.m_rigidbodiesOnPlatform.Count; ++index)
        {
          if ((bool) (Object) this.m_rigidbodiesOnPlatform[index] && (GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) this.m_rigidbodiesOnPlatform[index].UnitCenter) || this.specRigidbody.ContainsPoint(this.m_rigidbodiesOnPlatform[index].UnitCenter, collideWithTriggers: true)))
          {
            if ((bool) (Object) this.m_rigidbodiesOnPlatform[index].gameActor)
            {
              if (this.m_rigidbodiesOnPlatform[index].gameActor.IsGrounded)
                this.m_rigidbodiesOnPlatform[index].specRigidbody.ImpartedPixelsToMove = intVector2;
            }
            else
              this.m_rigidbodiesOnPlatform[index].Velocity += this.Velocity;
          }
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnEnterTrigger(
        SpeculativeRigidbody obj,
        SpeculativeRigidbody source,
        CollisionData collisionData)
      {
        if (this.m_rigidbodiesOnPlatform.Contains(obj) || (bool) (Object) obj.gameActor && obj.gameActor is PlayerController && PassiveItem.IsFlagSetForCharacter(obj.gameActor as PlayerController, typeof (HeavyBootsItem)))
          return;
        this.m_rigidbodiesOnPlatform.Add(obj);
        this.specRigidbody.RegisterCarriedRigidbody(obj);
      }

      private void OnExitTrigger(SpeculativeRigidbody obj, SpeculativeRigidbody source)
      {
        if (!this.m_rigidbodiesOnPlatform.Contains(obj))
          return;
        this.m_rigidbodiesOnPlatform.Remove(obj);
        if (!(bool) (Object) this)
          return;
        this.specRigidbody.DeregisterCarriedRigidbody(obj);
      }

      public void PostFieldConfiguration(RoomHandler room)
      {
        IntVector2 intVector2 = this.transform.position.IntXY();
        for (int x = 0; (double) x < (double) this.ConveyorWidth; ++x)
        {
          for (int y = 0; (double) y < (double) this.ConveyorHeight; ++y)
          {
            CellData cellData = GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(x, y)];
            if (cellData != null)
            {
              cellData.containsTrap = true;
              cellData.cellVisualData.RequiresPitBordering = true;
            }
          }
        }
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
      }
    }

}
