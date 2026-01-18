using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CaterpillarDevourHeartBehavior : OverrideBehaviorBase
  {
    public string MunchAnimName = "munch";
    public int RequiredHearts = 3;
    public GameObject NoticedHeartVFX;
    [PickupIdentifier]
    public int SourceCompanionItemId = -1;
    [PickupIdentifier]
    public int WingsItemIdToGive = -1;
    public GameObject TransformVFX;
    [NonSerialized]
    private int m_heartsMunched;
    private PickupObject m_targetHeart;
    private float m_repathTimer = 0.25f;
    private float m_cachedSpeed;

    public override void Start()
    {
      base.Start();
      this.m_cachedSpeed = this.m_aiActor.MovementSpeed;
      this.m_heartsMunched = 0;
    }

    private bool IsHeartInRoom()
    {
      PlayerController companionOwner = this.m_aiActor.CompanionOwner;
      if (!(bool) (UnityEngine.Object) companionOwner || companionOwner.CurrentRoom == null)
        return false;
      List<HealthPickup> componentsAbsoluteInRoom = companionOwner.CurrentRoom.GetComponentsAbsoluteInRoom<HealthPickup>();
      for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
      {
        HealthPickup healthPickup = componentsAbsoluteInRoom[index];
        if ((bool) (UnityEngine.Object) healthPickup && healthPickup.armorAmount == 0)
        {
          componentsAbsoluteInRoom.RemoveAt(index);
          --index;
        }
      }
      HealthPickup closestToPosition = BraveUtility.GetClosestToPosition<HealthPickup>(componentsAbsoluteInRoom, this.m_aiActor.CenterPosition);
      if (!((UnityEngine.Object) closestToPosition != (UnityEngine.Object) null))
        return false;
      this.m_targetHeart = (PickupObject) closestToPosition;
      return true;
    }

    private void MunchHeart(PickupObject targetHeart)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) targetHeart.gameObject);
      ++this.m_heartsMunched;
      this.m_aiAnimator.PlayUntilFinished("munch");
      if (this.m_heartsMunched < this.RequiredHearts)
        return;
      this.DoTransformation();
    }

    private void DoTransformation()
    {
      if (!((UnityEngine.Object) this.m_aiActor.CompanionOwner != (UnityEngine.Object) null))
        return;
      if ((bool) (UnityEngine.Object) this.TransformVFX)
        SpawnManager.SpawnVFX(this.TransformVFX, (Vector3) this.m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
      GameManager.Instance.StartCoroutine(this.DelayedGiveItem(this.m_aiActor.CompanionOwner));
      if (this.SourceCompanionItemId < 0)
        return;
      this.m_aiActor.CompanionOwner.RemovePassiveItem(this.SourceCompanionItemId);
    }

    [DebuggerHidden]
    private IEnumerator DelayedGiveItem(PlayerController targetPlayer)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CaterpillarDevourHeartBehavior__DelayedGiveItemc__Iterator0()
      {
        targetPlayer = targetPlayer,
        _this = this
      };
    }

    public override void Upkeep()
    {
      this.DecrementTimer(ref this.m_repathTimer);
      base.Upkeep();
    }

    public override BehaviorResult Update()
    {
      if ((bool) (UnityEngine.Object) this.m_targetHeart)
      {
        this.m_aiActor.PathfindToPosition(this.m_targetHeart.sprite.WorldCenter, new Vector2?(this.m_targetHeart.sprite.WorldCenter));
        if (this.m_aiActor.Path != null && this.m_aiActor.Path.WillReachFinalGoal)
        {
          this.m_aiActor.MovementSpeed = 1f;
          if ((double) Vector2.Distance(this.m_targetHeart.sprite.WorldCenter, this.m_aiActor.CenterPosition) < 1.25)
          {
            this.MunchHeart(this.m_targetHeart);
            this.m_targetHeart = (PickupObject) null;
          }
          return BehaviorResult.SkipAllRemainingBehaviors;
        }
        if ((double) Vector2.Distance(this.m_targetHeart.sprite.WorldCenter, this.m_aiActor.CenterPosition) >= 1.25)
          return base.Update();
        this.MunchHeart(this.m_targetHeart);
        this.m_targetHeart = (PickupObject) null;
        return BehaviorResult.SkipAllRemainingBehaviors;
      }
      this.m_aiActor.MovementSpeed = this.m_cachedSpeed;
      this.m_targetHeart = (PickupObject) null;
      if ((double) this.m_repathTimer <= 0.0)
      {
        this.m_repathTimer = 1f;
        if (this.IsHeartInRoom())
          return BehaviorResult.SkipAllRemainingBehaviors;
      }
      return base.Update();
    }
  }

