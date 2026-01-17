// Decompiled with JetBrains decompiler
// Type: CardboardBoxItem
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

namespace ETG.Core.Items.Active
{
    public class CardboardBoxItem : PlayerItem
    {
      public GameObject prefabToAttachToPlayer;
      public float SneakAttackDamageMultiplier = 2f;
      private GameObject instanceBox;
      private tk2dSprite instanceBoxSprite;
      private PlayerController m_player;

      protected override void OnPreDrop(PlayerController user)
      {
        if (this.IsCurrentlyActive)
          this.BreakStealth(user);
        base.OnPreDrop(user);
      }

      protected override void DoEffect(PlayerController user)
      {
        this.m_player = user;
        if ((bool) (UnityEngine.Object) this.m_player && (bool) (UnityEngine.Object) this.m_player.CurrentGun)
          this.m_player.CurrentGun.CeaseAttack(false);
        this.IsCurrentlyActive = true;
        bool flag = this.CanAnyBossSee(user);
        this.m_player.OnDidUnstealthyAction += new Action<PlayerController>(this.BreakStealth);
        this.m_player.PostProcessProjectile += new Action<Projectile, float>(this.SneakAttackProcessor);
        this.m_player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
        if (!flag)
        {
          user.SetIsStealthed(true, "box");
          user.SetCapableOfStealing(true, nameof (CardboardBoxItem));
        }
        this.instanceBox = user.RegisterAttachedObject(this.prefabToAttachToPlayer, string.Empty);
        this.instanceBoxSprite = this.instanceBox.GetComponent<tk2dSprite>();
        this.instanceBoxSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.LowerLeft);
        this.instanceBoxSprite.spriteAnimator.Play("cardboard_on");
        user.StartCoroutine(this.HandlePutOn(user, (tk2dBaseSprite) this.instanceBoxSprite));
      }

      private void SneakAttackProcessor(Projectile arg1, float arg2)
      {
        if (!(bool) (UnityEngine.Object) this.m_player || !this.m_player.IsStealthed)
          return;
        arg1.baseData.damage *= this.SneakAttackDamageMultiplier;
      }

      private bool CanAnyBossSee(PlayerController user)
      {
        Vector2 centerPosition = user.CenterPosition;
        for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
        {
          if (StaticReferenceManager.AllNpcs[index].ParentRoom == user.CurrentRoom)
          {
            Vector2 unitCenter = StaticReferenceManager.AllNpcs[index].specRigidbody.UnitCenter;
            int enemyVisibilityMask = CollisionMask.StandardEnemyVisibilityMask;
            RaycastResult result;
            if (!PhysicsEngine.Instance.Raycast(unitCenter, centerPosition - unitCenter, (centerPosition - unitCenter).magnitude, out result, rayMask: enemyVisibilityMask, ignoreRigidbody: StaticReferenceManager.AllNpcs[index].specRigidbody))
              RaycastResult.Pool.Free(ref result);
            else if ((UnityEngine.Object) result.SpeculativeRigidbody == (UnityEngine.Object) null || (UnityEngine.Object) result.SpeculativeRigidbody.gameObject != (UnityEngine.Object) user.gameObject)
            {
              RaycastResult.Pool.Free(ref result);
            }
            else
            {
              RaycastResult.Pool.Free(ref result);
              return true;
            }
          }
        }
        if (user.CurrentRoom != null)
        {
          List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies != null)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].specRigidbody && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
              {
                Vector2 unitCenter = activeEnemies[index].specRigidbody.UnitCenter;
                int enemyVisibilityMask = CollisionMask.StandardEnemyVisibilityMask;
                RaycastResult result;
                if (!PhysicsEngine.Instance.Raycast(unitCenter, centerPosition - unitCenter, (centerPosition - unitCenter).magnitude, out result, rayMask: enemyVisibilityMask, ignoreRigidbody: activeEnemies[index].specRigidbody))
                  RaycastResult.Pool.Free(ref result);
                else if ((UnityEngine.Object) result.SpeculativeRigidbody == (UnityEngine.Object) null || (UnityEngine.Object) result.SpeculativeRigidbody.gameObject != (UnityEngine.Object) user.gameObject)
                {
                  RaycastResult.Pool.Free(ref result);
                }
                else
                {
                  RaycastResult.Pool.Free(ref result);
                  return true;
                }
              }
            }
          }
        }
        return false;
      }

      [DebuggerHidden]
      private IEnumerator HandlePutOn(PlayerController user, tk2dBaseSprite instanceBoxSprite)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CardboardBoxItem.\u003CHandlePutOn\u003Ec__Iterator0()
        {
          user = user,
          instanceBoxSprite = instanceBoxSprite,
          \u0024this = this
        };
      }

      private void OnDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        this.BreakStealth(this.m_player);
      }

      private void BreakStealth(PlayerController obj)
      {
        this.m_player.OnDidUnstealthyAction -= new Action<PlayerController>(this.BreakStealth);
        this.m_player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
        this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.SneakAttackProcessor);
        this.IsCurrentlyActive = false;
        obj.IsVisible = true;
        obj.SetIsStealthed(false, "box");
        obj.SetCapableOfStealing(false, nameof (CardboardBoxItem));
        obj.DeregisterAttachedObject(this.instanceBox, false);
        this.instanceBoxSprite.spriteAnimator.PlayAndDestroyObject("cardboard_off");
        this.instanceBoxSprite = (tk2dSprite) null;
      }

      protected override void DoActiveEffect(PlayerController user) => this.BreakStealth(user);

      public void LateUpdate()
      {
        if (!this.IsCurrentlyActive)
          return;
        if (this.instanceBoxSprite.FlipX != this.m_player.sprite.FlipX)
          this.instanceBoxSprite.FlipX = this.m_player.sprite.FlipX;
        this.instanceBoxSprite.PlaceAtPositionByAnchor(this.m_player.SpriteBottomCenter + (!this.m_player.sprite.FlipX ? 1f : -1f) * new Vector3(-0.5f, 0.0f, 0.0f), tk2dBaseSprite.Anchor.LowerCenter);
        if (this.instanceBoxSprite.spriteAnimator.IsPlaying("cardboard_on"))
          return;
        if (this.m_player.specRigidbody.Velocity == Vector2.zero)
        {
          if (this.m_player.spriteAnimator.CurrentClip.name.Contains("backward") || this.m_player.spriteAnimator.CurrentClip.name.Contains("_bw"))
            this.instanceBoxSprite.spriteAnimator.Play("idle_back");
          else
            this.instanceBoxSprite.spriteAnimator.Play("idle");
        }
        else if (this.m_player.spriteAnimator.CurrentClip.name.Contains("run_up") || this.m_player.spriteAnimator.CurrentClip.name.Contains("_bw"))
          this.instanceBoxSprite.spriteAnimator.Play("move_right_backwards");
        else
          this.instanceBoxSprite.spriteAnimator.Play("move_right_forward");
      }

      public override void OnItemSwitched(PlayerController user)
      {
        if (!this.IsCurrentlyActive)
          return;
        this.DoActiveEffect(user);
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
