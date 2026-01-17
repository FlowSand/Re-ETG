// Decompiled with JetBrains decompiler
// Type: GuidedBulletsPassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class GuidedBulletsPassiveItem : PassiveItem
    {
      public float trackingSpeed = 45f;
      public float trackingTime = 6f;
      [CurveRange(0.0f, 0.0f, 1f, 1f)]
      public AnimationCurve trackingCurve;
      private PlayerController m_player;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_player = player;
        base.Pickup(player);
        player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
        player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
      }

      private void PostProcessBeam(BeamController obj)
      {
      }

      private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
      {
        obj.PreMoveModifiers += new Action<Projectile>(this.PreMoveProjectileModifier);
      }

      private void PreMoveProjectileModifier(Projectile p)
      {
        if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) p || !(p.Owner is PlayerController))
          return;
        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_owner.PlayerIDX);
        if ((UnityEngine.Object) instanceForPlayer == (UnityEngine.Object) null)
          return;
        Vector2 zero = Vector2.zero;
        Vector2 vector;
        if (instanceForPlayer.IsKeyboardAndMouse())
        {
          vector = (p.Owner as PlayerController).unadjustedAimPoint.XY() - p.specRigidbody.UnitCenter;
        }
        else
        {
          if (instanceForPlayer.ActiveActions == null)
            return;
          vector = instanceForPlayer.ActiveActions.Aim.Vector;
        }
        float angle = vector.ToAngle();
        float current = BraveMathCollege.Atan2Degrees(p.Direction);
        float num = 0.0f;
        if ((double) p.ElapsedTime < (double) this.trackingTime)
          num = this.trackingCurve.Evaluate(p.ElapsedTime / this.trackingTime) * this.trackingSpeed;
        float target = Mathf.MoveTowardsAngle(current, angle, num * BraveTime.DeltaTime);
        Vector2 vector2 = (Vector2) (Quaternion.Euler(0.0f, 0.0f, Mathf.DeltaAngle(current, target)) * (Vector3) p.Direction);
        if (p is HelixProjectile)
          (p as HelixProjectile).AdjustRightVector(Mathf.DeltaAngle(current, target));
        if (p.OverrideMotionModule != null)
          p.OverrideMotionModule.AdjustRightVector(Mathf.DeltaAngle(current, target));
        p.Direction = vector2.normalized;
        if (!p.shouldRotate)
          return;
        p.transform.eulerAngles = new Vector3(0.0f, 0.0f, p.Direction.ToAngle());
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_player = (PlayerController) null;
        debrisObject.GetComponent<GuidedBulletsPassiveItem>().m_pickedUpThisRun = true;
        player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      }
    }

}
