// Decompiled with JetBrains decompiler
// Type: MatterAntimatterProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class MatterAntimatterProjectileModifier : BraveBehaviour
    {
      public bool isAntimatter;
      private bool m_hasAnnihilated;
      public ExplosionData antimatterExplosion;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MatterAntimatterProjectileModifier.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void OnPreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if (this.m_hasAnnihilated || !(bool) (Object) otherRigidbody.projectile)
          return;
        MatterAntimatterProjectileModifier component = otherRigidbody.GetComponent<MatterAntimatterProjectileModifier>();
        if ((bool) (Object) component && component.isAntimatter != this.isAntimatter)
        {
          this.m_hasAnnihilated = true;
          component.m_hasAnnihilated = true;
          otherRigidbody.projectile.DieInAir();
          this.projectile.DieInAir();
          Vector3 vector3 = (Vector3) ((myRigidbody.UnitCenter + otherRigidbody.UnitCenter) / 2f);
          Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.05f);
          GameManager.Instance.BestActivePlayer.ForceBlank(breaksWalls: false, overrideCenter: new Vector2?(vector3.XY()));
          if (this.isAntimatter)
            Exploder.Explode(vector3, this.antimatterExplosion, Vector2.zero);
          else
            Exploder.Explode(vector3, component.antimatterExplosion, Vector2.zero);
        }
        PhysicsEngine.SkipCollision = true;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
