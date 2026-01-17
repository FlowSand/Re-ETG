// Decompiled with JetBrains decompiler
// Type: GrappleModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class GrappleModule
    {
      public GameObject GrapplePrefab;
      public float GrappleSpeed = 10f;
      public float GrappleRetractSpeed = 10f;
      public float DamageToEnemies = 10f;
      public float EnemyKnockbackForce = 10f;
      public GameObject sourceGameObject;
      public System.Action FinishedCallback;
      private GameObject m_extantGrapple;
      private bool m_hasImpactedTile;
      private bool m_hasImpactedEnemy;
      private bool m_hasImpactedShopItem;
      private bool m_hasImpactedItem;
      private bool m_tileImpactFake;
      private AIActor m_impactedEnemy;
      private PickupObject m_impactedItem;
      private ShopItemController m_impactedShopItem;
      private bool m_isDone;
      private PlayerController m_lastUser;
      private Coroutine m_lastCoroutine;

      public void Trigger(PlayerController user)
      {
        this.m_lastUser = user;
        this.m_extantGrapple = UnityEngine.Object.Instantiate<GameObject>(this.GrapplePrefab);
        this.m_hasImpactedEnemy = false;
        this.m_hasImpactedTile = false;
        this.m_hasImpactedItem = false;
        this.m_hasImpactedShopItem = false;
        this.m_tileImpactFake = false;
        this.m_isDone = false;
        tk2dTiledSprite componentInChildren = this.m_extantGrapple.GetComponentInChildren<tk2dTiledSprite>();
        componentInChildren.dimensions = new Vector2(3f, componentInChildren.dimensions.y);
        this.m_extantGrapple.transform.position = user.CenterPosition.ToVector3ZUp();
        this.m_lastCoroutine = user.StartCoroutine(this.HandleGrappleEffect(user));
      }

      public void MarkDone() => this.m_isDone = true;

      public void ForceEndGrapple()
      {
        if (this.m_isDone)
          return;
        if ((UnityEngine.Object) this.m_lastUser != (UnityEngine.Object) null)
        {
          this.m_lastUser.healthHaver.IsVulnerable = true;
          this.m_lastUser.SetIsFlying(false, "grapple", false);
          this.m_lastUser.CurrentInputState = PlayerInputState.AllInput;
        }
        this.m_isDone = true;
        this.m_lastUser = (PlayerController) null;
        PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.PostMovementUpdate);
      }

      public void ForceEndGrappleImmediate()
      {
        if (this.m_isDone)
          return;
        if ((UnityEngine.Object) this.m_lastUser != (UnityEngine.Object) null)
        {
          if (this.m_lastCoroutine != null)
            this.m_lastUser.StopCoroutine(this.m_lastCoroutine);
          this.m_lastUser.healthHaver.IsVulnerable = true;
          this.m_lastUser.SetIsFlying(false, "grapple", false);
          this.m_lastUser.CurrentInputState = PlayerInputState.AllInput;
        }
        this.m_isDone = true;
        this.m_lastUser = (PlayerController) null;
        PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.PostMovementUpdate);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantGrapple);
        this.m_extantGrapple = (GameObject) null;
        if (this.FinishedCallback == null)
          return;
        this.FinishedCallback();
      }

      public void ClearExtantGrapple()
      {
        if (!((UnityEngine.Object) this.m_extantGrapple != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantGrapple);
        this.m_extantGrapple = (GameObject) null;
      }

      [DebuggerHidden]
      protected IEnumerator HandleGrappleEffect(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GrappleModule.<HandleGrappleEffect>c__Iterator0()
        {
          user = user,
          $this = this
        };
      }

      private void HandleNinjaToolsSynergy(Projectile sourceProjectile, float beamEffectPercentage)
      {
        HomingModifier homingModifier = sourceProjectile.GetComponent<HomingModifier>();
        if ((UnityEngine.Object) homingModifier == (UnityEngine.Object) null)
        {
          homingModifier = sourceProjectile.gameObject.AddComponent<HomingModifier>();
          homingModifier.HomingRadius = 0.0f;
          homingModifier.AngularVelocity = 0.0f;
        }
        homingModifier.HomingRadius = Mathf.Max(12f, homingModifier.HomingRadius);
        homingModifier.AngularVelocity = Mathf.Max(720f, homingModifier.HomingRadius);
      }

      protected virtual void OnPreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherCollider)
      {
        if (this.m_hasImpactedItem || this.m_hasImpactedEnemy)
          PhysicsEngine.SkipCollision = true;
        else if ((UnityEngine.Object) otherRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null)
        {
          PhysicsEngine.SkipCollision = true;
        }
        else
        {
          if (!((UnityEngine.Object) otherRigidbody.GetComponent<MinorBreakable>() != (UnityEngine.Object) null))
            return;
          if (!otherRigidbody.GetComponent<MinorBreakable>().IsBroken)
            otherRigidbody.GetComponent<MinorBreakable>().Break();
          PhysicsEngine.SkipCollision = true;
        }
      }

      private void ImpactedRigidbody(CollisionData rigidbodyCollision)
      {
        if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.aiActor)
        {
          this.m_impactedEnemy = rigidbodyCollision.OtherRigidbody.aiActor;
          this.m_hasImpactedEnemy = true;
          rigidbodyCollision.MyRigidbody.Velocity = Vector2.zero;
        }
        else
        {
          ShopItemController component = rigidbodyCollision.OtherRigidbody.GetComponent<ShopItemController>();
          if ((bool) (UnityEngine.Object) component)
          {
            int num = (int) AkSoundEngine.PostEvent("Play_WPN_metalbullet_impact_01", this.sourceGameObject);
            this.m_impactedShopItem = component.Locked ? (ShopItemController) null : component;
            this.m_hasImpactedShopItem = true;
            component.specRigidbody.enabled = false;
            rigidbodyCollision.MyRigidbody.Velocity = Vector2.zero;
          }
          else
          {
            this.m_hasImpactedTile = true;
            rigidbodyCollision.MyRigidbody.Velocity = Vector2.zero;
          }
        }
      }

      private void ImpactedTile(CollisionData tileCollision)
      {
        this.m_hasImpactedTile = true;
        tileCollision.MyRigidbody.Velocity = Vector2.zero;
      }

      private void PostMovementUpdate()
      {
        if (!(bool) (UnityEngine.Object) this.m_lastUser || !(bool) (UnityEngine.Object) this.m_extantGrapple)
          return;
        SpeculativeRigidbody component = this.m_extantGrapple.GetComponent<SpeculativeRigidbody>();
        tk2dTiledSprite componentInChildren = component.GetComponentInChildren<tk2dTiledSprite>();
        Vector2 v = component.UnitCenter - this.m_lastUser.CenterPosition;
        int x = Mathf.RoundToInt(v.magnitude / (1f / 16f));
        componentInChildren.dimensions = new Vector2((float) x, componentInChildren.dimensions.y);
        float z = BraveMathCollege.Atan2Degrees(v);
        component.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
      }
    }

}
