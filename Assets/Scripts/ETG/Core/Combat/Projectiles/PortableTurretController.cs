using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PortableTurretController : MonoBehaviour
  {
    [NonSerialized]
    public PlayerController sourcePlayer;
    public float maxDuration = 20f;
    private AIActor actor;
    private GameObject m_fallbackProjectile;

    private void Awake()
    {
      this.actor = this.GetComponent<AIActor>();
      this.actor.PreventFallingInPitsEver = true;
    }

    private void Start()
    {
      this.actor.CanTargetEnemies = true;
      this.actor.CanTargetPlayers = false;
      this.actor.ParentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      this.actor.HasBeenEngaged = true;
      this.actor.ParentRoom.OnEnemiesCleared += new System.Action(this.HandleRoomCleared);
      this.actor.aiShooter.PostProcessProjectile += new Action<Projectile>(this.PostProcessProjectile);
      this.GetComponent<tk2dSpriteAnimator>().QueueAnimation("portable_turret_fire");
      this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider));
      this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox));
      this.StartCoroutine(this.HandleTimedDestroy());
    }

    private void Update()
    {
      if (!(bool) (UnityEngine.Object) this.actor || !this.actor.IsFalling)
        return;
      this.GetComponent<tk2dSpriteAnimator>().Play("portable_turret_undeploy");
      this.GetComponent<tk2dSpriteAnimator>().AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.Disappear);
    }

    private void PostProcessProjectile(Projectile obj)
    {
      if (!(bool) (UnityEngine.Object) this.sourcePlayer)
        return;
      this.sourcePlayer.DoPostProcessProjectile(obj);
      if (this.sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.TURRET_RANDOMIZER))
      {
        if ((UnityEngine.Object) this.m_fallbackProjectile == (UnityEngine.Object) null)
          this.m_fallbackProjectile = this.actor.bulletBank.Bullets[0].BulletObject;
        this.actor.bulletBank.Bullets[0].BulletObject = ProjectileRandomizerItem.GetRandomizerProjectileFromPlayer(this.sourcePlayer, this.m_fallbackProjectile.GetComponent<Projectile>(), 800).gameObject;
      }
      if (!this.sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.CAPTAINPLANTIT))
        ;
    }

    public void NotifyDropped() => this.HandleRoomCleared();

    [DebuggerHidden]
    private IEnumerator HandleTimedDestroy()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PortableTurretController__HandleTimedDestroyc__Iterator0()
      {
        _this = this
      };
    }

    private void HandleRoomCleared()
    {
      if (!(bool) (UnityEngine.Object) this.actor)
        return;
      this.GetComponent<tk2dSpriteAnimator>().Play("portable_turret_undeploy");
      this.GetComponent<tk2dSpriteAnimator>().AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.Disappear);
    }

    private void Disappear(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

