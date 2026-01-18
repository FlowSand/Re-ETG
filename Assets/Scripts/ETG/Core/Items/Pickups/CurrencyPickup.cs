// Decompiled with JetBrains decompiler
// Type: CurrencyPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CurrencyPickup : PickupObject
  {
    public int currencyValue = 1;
    public string overrideBloopSpriteName = string.Empty;
    public bool IsMetaCurrency;
    private bool m_hasBeenPickedUp;
    private Transform m_transform;
    private SpeculativeRigidbody m_srb;
    [NonSerialized]
    public bool PreventPickup;

    private void Start()
    {
      this.m_transform = this.transform;
      this.m_transform.position = this.m_transform.position.Quantize(1f / 16f);
      this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      this.m_srb.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision);
      this.m_srb.Reinitialize();
      if (!(bool) (UnityEngine.Object) this.debris)
        return;
      if (this.IsMetaCurrency)
        this.debris.FlagAsPickup();
      this.debris.OnGrounded += (Action<DebrisObject>) (a => this.specRigidbody.Reinitialize());
      this.debris.ForceUpdateIfDisabled = true;
    }

    private void Update()
    {
      if (!this.IsMetaCurrency)
      {
        if (!((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null) || this.spriteAnimator.DefaultClip == null)
          return;
        if (this.spriteAnimator.IsPlaying(this.spriteAnimator.CurrentClip))
          this.spriteAnimator.Stop();
        this.spriteAnimator.SetFrame(Mathf.FloorToInt(UnityEngine.Time.time * this.spriteAnimator.DefaultClip.fps % (float) this.spriteAnimator.DefaultClip.frames.Length));
      }
      else
      {
        if (GameManager.Instance.IsLoadingLevel || this.m_hasBeenPickedUp)
          return;
        if (!this.m_hasBeenPickedUp && (bool) (UnityEngine.Object) this.debris && (bool) (UnityEngine.Object) this.debris.specRigidbody && this.debris.Static && PhysicsEngine.Instance.OverlapCast(this.debris.specRigidbody, overrideCollisionMask: new int?(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle))))
          this.debris.ForceDestroyAndMaybeRespawn();
        if ((bool) (UnityEngine.Object) this && !GameManager.Instance.IsAnyPlayerInRoom(this.transform.position.GetAbsoluteRoom()))
        {
          PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
          if (!(bool) (UnityEngine.Object) bestActivePlayer || bestActivePlayer.IsGhost || !bestActivePlayer.AcceptingAnyInput)
            return;
          this.m_hasBeenPickedUp = true;
          this.Pickup(bestActivePlayer);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer || !(bool) (UnityEngine.Object) this.sprite)
            return;
          CellData cellData = (CellData) null;
          IntVector2 intVector2 = this.sprite.WorldCenter.ToIntVector2(VectorConversions.Floor);
          if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
            cellData = GameManager.Instance.Dungeon.data[intVector2];
          if (cellData != null && cellData.type != CellType.WALL)
            return;
          this.m_hasBeenPickedUp = true;
          this.Pickup(GameManager.Instance.PrimaryPlayer);
        }
      }
    }

    private void OnPreCollision(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if (this.PreventPickup || this.m_hasBeenPickedUp)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.m_hasBeenPickedUp = true;
      this.Pickup(component);
    }

    [DebuggerHidden]
    private IEnumerator InitialBounce()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CurrencyPickup__InitialBouncec__Iterator0()
      {
        _this = this
      };
    }

    public void ForceSetPickedUp() => this.m_hasBeenPickedUp = true;

    public override void Pickup(PlayerController player)
    {
      if (this.IsMetaCurrency)
      {
        this.spriteAnimator.StopAndResetFrame();
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, (float) this.currencyValue);
        tk2dBaseSprite targetAutomaticSprite = (tk2dBaseSprite) this.GetComponent<HologramDoer>().TargetAutomaticSprite;
        targetAutomaticSprite.spriteAnimator.StopAndResetFrame();
        player.BloopItemAboveHead(targetAutomaticSprite, this.overrideBloopSpriteName);
        tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Pickup")).GetComponent<tk2dSprite>();
        component.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
        component.UpdateZDepth();
        if ((bool) (UnityEngine.Object) this.encounterTrackable)
        {
          this.encounterTrackable.DoNotificationOnEncounter = false;
          this.encounterTrackable.HandleEncounter();
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metacoin_collect_01", this.gameObject);
        }
        if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY) == 1.0)
          GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetItemsString("#HEGEMONYCREDIT_ENCNAME"), StringTableManager.GetItemsString("#HEGEMONYCREDIT_SHORTDESC"), targetAutomaticSprite.Collection, targetAutomaticSprite.spriteId, UINotificationController.NotificationColor.GOLD);
      }
      else
      {
        this.HandleEncounterable(player);
        player.BloopItemAboveHead(this.sprite, this.overrideBloopSpriteName);
        if (this.sprite.name == "Coin_Copper(Clone)")
        {
          int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_small_01", this.gameObject);
        }
        else if (this.sprite.name == "Coin_Silver(Clone)")
        {
          int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_medium_01", this.gameObject);
        }
        else
        {
          int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_large_01", this.gameObject);
        }
        GameManager.Instance.PrimaryPlayer.carriedConsumables.Currency += this.currencyValue;
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

