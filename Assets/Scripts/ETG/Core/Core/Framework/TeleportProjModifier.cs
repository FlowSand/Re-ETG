// Decompiled with JetBrains decompiler
// Type: TeleportProjModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TeleportProjModifier : BraveBehaviour
    {
      public TeleportProjModifier.TeleportTrigger trigger = TeleportProjModifier.TeleportTrigger.AngleToTarget;
      [ShowInInspectorIf("ShowMMinAngleToTeleport", true)]
      public float minAngleToTeleport = 70f;
      [ShowInInspectorIf("ShowDistToTeleport", true)]
      public float distToTeleport = 3f;
      public TeleportProjModifier.TeleportType type = TeleportProjModifier.TeleportType.BackToSpawn;
      [ShowInInspectorIf("ShowBehindTargetDistance", true)]
      public float behindTargetDistance = 5f;
      public int numTeleports;
      public float teleportPauseTime;
      public float leadAmount;
      public float teleportCooldown;
      public VFXPool teleportVfx;
      private SpeculativeRigidbody m_targetRigidbody;
      private Vector3 m_startingPos;
      private bool m_isTeleporting;
      private float m_cooldown;

      public event System.Action OnTeleport;

      private bool ShowMMinAngleToTeleport()
      {
        return this.trigger == TeleportProjModifier.TeleportTrigger.AngleToTarget;
      }

      private bool ShowDistToTeleport()
      {
        return this.trigger == TeleportProjModifier.TeleportTrigger.DistanceFromTarget;
      }

      private bool ShowBehindTargetDistance()
      {
        return this.type == TeleportProjModifier.TeleportType.BehindTarget;
      }

      public void Start()
      {
        if (!(bool) (UnityEngine.Object) this.sprite)
          this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
        if ((bool) (UnityEngine.Object) this.projectile && this.projectile.Owner is AIActor)
          this.m_targetRigidbody = (this.projectile.Owner as AIActor).TargetRigidbody;
        if (!(bool) (UnityEngine.Object) this.m_targetRigidbody)
          this.enabled = false;
        else
          this.m_startingPos = this.transform.position;
      }

      public void Update()
      {
        if (this.m_isTeleporting)
          return;
        if ((double) this.m_cooldown > 0.0)
        {
          this.m_cooldown -= BraveTime.DeltaTime;
        }
        else
        {
          if (this.numTeleports <= 0 || !this.ShouldTeleport())
            return;
          this.StartCoroutine(this.DoTeleport());
        }
      }

      protected override void OnDestroy()
      {
        this.StopAllCoroutines();
        base.OnDestroy();
      }

      private bool ShouldTeleport()
      {
        Vector2 unitCenter = this.m_targetRigidbody.GetUnitCenter(ColliderType.HitBox);
        if (this.trigger == TeleportProjModifier.TeleportTrigger.AngleToTarget)
          return (double) BraveMathCollege.AbsAngleBetween((unitCenter - this.specRigidbody.UnitCenter).ToAngle(), this.specRigidbody.Velocity.ToAngle()) > (double) this.minAngleToTeleport;
        return this.trigger == TeleportProjModifier.TeleportTrigger.DistanceFromTarget && (double) Vector2.Distance(unitCenter, this.specRigidbody.UnitCenter) < (double) this.distToTeleport;
      }

      private Vector2 GetTeleportPosition()
      {
        if (this.type == TeleportProjModifier.TeleportType.BackToSpawn)
          return (Vector2) this.m_startingPos;
        if (this.type == TeleportProjModifier.TeleportType.BehindTarget && (bool) (UnityEngine.Object) this.m_targetRigidbody && (bool) (UnityEngine.Object) this.m_targetRigidbody.gameActor)
        {
          Vector2 unitCenter = this.m_targetRigidbody.GetUnitCenter(ColliderType.HitBox);
          float facingDirection = this.m_targetRigidbody.gameActor.FacingDirection;
          Dungeon dungeon = GameManager.Instance.Dungeon;
          for (int index = 0; index < 18; ++index)
          {
            Vector2 pos1 = unitCenter + BraveMathCollege.DegreesToVector(facingDirection + 180f + (float) (index * 20), this.behindTargetDistance);
            if (!dungeon.CellExists(pos1) || !dungeon.data.isWall((int) pos1.x, (int) pos1.y))
              return pos1;
            Vector2 pos2 = unitCenter + BraveMathCollege.DegreesToVector(facingDirection + 180f + (float) (index * -20), this.behindTargetDistance);
            if (!dungeon.CellExists(pos2) || !dungeon.data.isWall((int) pos2.x, (int) pos2.y))
              return pos2;
          }
        }
        return (Vector2) this.m_startingPos;
      }

      [DebuggerHidden]
      private IEnumerator DoTeleport()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TeleportProjModifier.\u003CDoTeleport\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public enum TeleportTrigger
      {
        AngleToTarget = 10, // 0x0000000A
        DistanceFromTarget = 20, // 0x00000014
      }

      public enum TeleportType
      {
        BackToSpawn = 10, // 0x0000000A
        BehindTarget = 20, // 0x00000014
      }
    }

}
