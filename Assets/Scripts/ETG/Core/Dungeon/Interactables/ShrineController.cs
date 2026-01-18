// Decompiled with JetBrains decompiler
// Type: ShrineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ShrineController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
  {
    public string displayTextKey;
    public string acceptOptionKey;
    public string declineOptionKey;
    public Transform talkPoint;
    public int healthToGive;
    public int armorToGive;
    public int moneyToGive;
    public int ammoPercentageToReplenish;
    public bool takesCurrentGun;
    public bool appliesStatChanges;
    public List<StatModifier> statModifiers;
    public bool cleansesCurse;
    public GameObject onPlayerVFX;
    public Vector3 playerVFXOffset = Vector3.zero;
    private int m_useCount;
    private RoomHandler m_parentRoom;
    private GameObject m_instanceMinimapIcon;

    public void ConfigureOnPlacement(RoomHandler room)
    {
      this.m_parentRoom = room;
      room.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject;
      this.RegisterMinimapIcon();
    }

    public void RegisterMinimapIcon()
    {
      this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject) BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon"));
    }

    public void GetRidOfMinimapIcon()
    {
      if (!((Object) this.m_instanceMinimapIcon != (Object) null))
        return;
      Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
      this.m_instanceMinimapIcon = (GameObject) null;
    }

    private void DoShrineEffect(PlayerController player)
    {
      if (this.takesCurrentGun && ((Object) player.CurrentGun == (Object) null || !player.CurrentGun.CanActuallyBeDropped(player)))
      {
        --this.m_useCount;
        this.m_parentRoom.RegisterInteractable((IPlayerInteractable) this);
      }
      else
      {
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", this.gameObject);
        if (this.healthToGive > 0)
        {
          int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
          player.healthHaver.ApplyHealing((float) this.healthToGive);
        }
        else if (this.healthToGive < 0)
          player.healthHaver.ApplyDamage((float) (this.healthToGive * -1), Vector2.zero, StringTableManager.GetEnemiesString("#SHRINE"), damageCategory: DamageCategory.Environment, ignoreInvulnerabilityFrames: true);
        if (this.armorToGive > 0)
          player.healthHaver.Armor += (float) this.armorToGive;
        if (this.moneyToGive > 0)
        {
          int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_item_purchase_01", this.gameObject);
          player.carriedConsumables.Currency += this.moneyToGive;
        }
        if (this.ammoPercentageToReplenish > 0)
        {
          for (int index = 0; index < player.inventory.AllGuns.Count; ++index)
          {
            int amt = player.inventory.AllGuns[index].AdjustedMaxAmmo * this.ammoPercentageToReplenish;
            if (amt <= 0)
            {
              int num4 = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
              amt = player.inventory.AllGuns[index].ammo * this.ammoPercentageToReplenish;
            }
            if (amt <= 0)
            {
              UnityEngine.Debug.LogError((object) "Shrine is attempting to give negative ammo!");
              amt = 1;
            }
            player.inventory.AllGuns[index].GainAmmo(amt);
          }
        }
        if (this.takesCurrentGun && (Object) player.CurrentGun != (Object) null && player.CurrentGun.CanActuallyBeDropped(player))
          player.inventory.DestroyCurrentGun();
        if (this.appliesStatChanges)
        {
          for (int index = 0; index < this.statModifiers.Count; ++index)
          {
            if (player.ownerlessStatModifiers == null)
              player.ownerlessStatModifiers = new List<StatModifier>();
            player.ownerlessStatModifiers.Add(this.statModifiers[index]);
          }
          player.stats.RecalculateStats(player);
        }
        if (this.cleansesCurse)
        {
          player.ownerlessStatModifiers.Add(new StatModifier()
          {
            amount = player.stats.GetStatValue(PlayerStats.StatType.Curse) * -1f,
            modifyType = StatModifier.ModifyMethod.ADDITIVE,
            statToBoost = PlayerStats.StatType.Curse
          });
          player.stats.RecalculateStats(player);
        }
        if ((Object) this.onPlayerVFX != (Object) null)
          player.PlayEffectOnActor(this.onPlayerVFX, this.playerVFXOffset);
        if ((Object) this.transform.parent != (Object) null)
        {
          EncounterTrackable component = this.transform.parent.gameObject.GetComponent<EncounterTrackable>();
          if ((Object) component != (Object) null)
            component.ForceDoNotification(this.m_instanceMinimapIcon.GetComponent<tk2dBaseSprite>());
        }
        this.GetRidOfMinimapIcon();
      }
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if ((Object) this.sprite == (Object) null)
        return 100f;
      Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
      return Vector2.Distance(point, (Vector2) b) / 1.5f;
    }

    public float GetOverrideMaxDistance() => -1f;

    public void OnEnteredRange(PlayerController interactor)
    {
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
    }

    public void OnExitRange(PlayerController interactor)
    {
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    }

    [DebuggerHidden]
    private IEnumerator HandleShrineConversation(PlayerController interactor)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ShrineController__HandleShrineConversationc__Iterator0()
      {
        interactor = interactor,
        _this = this
      };
    }

    public void Interact(PlayerController interactor)
    {
      if (this.m_useCount > 0)
        return;
      ++this.m_useCount;
      this.m_parentRoom.DeregisterInteractable((IPlayerInteractable) this);
      this.StartCoroutine(this.HandleShrineConversation(interactor));
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

