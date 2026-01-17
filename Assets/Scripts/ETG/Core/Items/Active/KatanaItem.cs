// Decompiled with JetBrains decompiler
// Type: KatanaItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class KatanaItem : PlayerItem
    {
      public float dashDistance = 10f;
      public float dashSpeed = 30f;
      public float collisionDamage = 50f;
      public float stunDuration = 1f;
      public float momentaryPause = 0.25f;
      public float finalDelay = 0.5f;
      public int sequentialValidUses = 3;
      public GameObject trailVFXPrefab;
      public GameObject poofVFX;
      private bool m_isDashing;
      private int m_useCount;
      private List<AIActor> actorsPassed = new List<AIActor>();
      private List<MajorBreakable> breakablesPassed = new List<MajorBreakable>();

      protected override void DoEffect(PlayerController user)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_CHR_ninja_dash_01", this.gameObject);
        if (this.m_isDashing)
          return;
        ++this.m_useCount;
        this.StartCoroutine(this.HandleDash(user));
      }

      protected override void AfterCooldownApplied(PlayerController user)
      {
        if (this.m_useCount >= this.sequentialValidUses)
          this.m_useCount = 0;
        else
          this.ClearCooldowns();
      }

      private float CalculateAdjustedDashDistance(PlayerController user, Vector2 dashDirection)
      {
        return this.dashDistance;
      }

      [DebuggerHidden]
      private IEnumerator HandleDash(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new KatanaItem.\u003CHandleDash\u003Ec__Iterator0()
        {
          user = user,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator EndAndDamage(
        List<AIActor> actors,
        List<MajorBreakable> breakables,
        PlayerController user,
        Vector2 dashDirection,
        Vector2 startPosition,
        Vector2 endPosition)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new KatanaItem.\u003CEndAndDamage\u003Ec__Iterator1()
        {
          user = user,
          startPosition = startPosition,
          actors = actors,
          dashDirection = dashDirection,
          breakables = breakables,
          \u0024this = this
        };
      }

      private void KatanaPreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if ((Object) otherRigidbody.projectile != (Object) null)
          PhysicsEngine.SkipCollision = true;
        if ((Object) otherRigidbody.aiActor != (Object) null)
        {
          PhysicsEngine.SkipCollision = true;
          if (!this.actorsPassed.Contains(otherRigidbody.aiActor))
          {
            otherRigidbody.aiActor.DelayActions(1f);
            this.actorsPassed.Add(otherRigidbody.aiActor);
          }
        }
        if (!((Object) otherRigidbody.majorBreakable != (Object) null))
          return;
        PhysicsEngine.SkipCollision = true;
        if (this.breakablesPassed.Contains(otherRigidbody.majorBreakable))
          return;
        this.breakablesPassed.Add(otherRigidbody.majorBreakable);
      }

      public override void OnItemSwitched(PlayerController user)
      {
        base.OnItemSwitched(user);
        if (this.m_useCount <= 0)
          return;
        this.m_useCount = 0;
        this.ApplyCooldown(user);
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
