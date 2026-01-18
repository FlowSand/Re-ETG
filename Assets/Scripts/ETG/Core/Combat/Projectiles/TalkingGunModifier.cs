using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class TalkingGunModifier : MonoBehaviour, IGunInheritable
  {
    public Transform talkPoint;
    public int roomsToRankUp = 10;
    public float ChanceToGainFriendship = 0.5f;
    private Gun m_gun;
    private int m_friendship;
    private int m_enmityCounter;
    private int m_begrudgingCounter;
    private int m_friendCounter;
    private PlayerController m_owner;
    private float m_destroyTimer;

    private void Start()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.AddAdditionalFlipTransform(this.talkPoint);
      this.m_gun.PostProcessProjectile += new Action<Projectile>(this.PostprocessFriendship);
      this.m_gun.OnInitializedWithOwner += new Action<GameActor>(this.OnGunReinitialized);
      if ((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null)
        this.OnGunReinitialized(this.m_gun.CurrentOwner);
      this.m_gun.OnDropped += new System.Action(this.HandleDropped);
    }

    private void OnGunReinitialized(GameActor newOwner)
    {
      this.m_owner = this.m_gun.CurrentOwner as PlayerController;
      this.m_owner.OnRoomClearEvent += new Action<PlayerController>(this.HandleRoomCleared);
    }

    private void HandleDropped()
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
        this.m_owner.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
      this.m_owner = (PlayerController) null;
    }

    private void PostprocessFriendship(Projectile obj)
    {
      BounceProjModifier component1 = obj.GetComponent<BounceProjModifier>();
      PierceProjModifier component2 = obj.GetComponent<PierceProjModifier>();
      if (this.m_friendship < this.roomsToRankUp)
        return;
      if (this.m_friendship < this.roomsToRankUp * 2)
      {
        obj.baseData.damage += 3f;
        obj.baseData.speed += 6f;
        if ((bool) (UnityEngine.Object) component1)
          obj.GetComponent<BounceProjModifier>().numberOfBounces += 2;
        if (!(bool) (UnityEngine.Object) component2)
          return;
        obj.GetComponent<PierceProjModifier>().penetration += 3;
      }
      else
      {
        obj.baseData.damage += 6f;
        obj.baseData.speed += 6f;
        if ((bool) (UnityEngine.Object) component2)
          obj.GetComponent<PierceProjModifier>().BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
        HomingModifier homingModifier = obj.gameObject.AddComponent<HomingModifier>();
        homingModifier.HomingRadius = 8f;
        homingModifier.AngularVelocity = 360f;
      }
    }

    private void ClearTextBoxForReal()
    {
      TextBoxManager.ClearTextBox(this.talkPoint);
      if (!(bool) (UnityEngine.Object) this.talkPoint || this.talkPoint.childCount <= 0)
        return;
      for (int index = this.talkPoint.childCount - 1; index >= 0; --index)
      {
        Transform child = this.talkPoint.GetChild(index);
        if ((bool) (UnityEngine.Object) child)
          UnityEngine.Object.Destroy((UnityEngine.Object) child.gameObject);
      }
    }

    private void Update()
    {
      if ((bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.sprite)
      {
        this.talkPoint.transform.localPosition = new Vector3(0.875f, !this.m_gun.sprite.FlipY ? 21f / 16f : -21f / 16f, 0.0f);
        if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.CurrentRoom != null && this.m_owner.CurrentRoom.IsSealed && this.m_owner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && this.talkPoint.childCount > 0)
        {
          if ((double) this.m_destroyTimer < 0.25)
            this.m_destroyTimer += BraveTime.DeltaTime;
          else
            this.ClearTextBoxForReal();
        }
        else
          this.m_destroyTimer = 0.0f;
      }
      this.talkPoint.rotation = Quaternion.identity;
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedTalk(PlayerController obj)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TalkingGunModifier__HandleDelayedTalkc__Iterator0()
      {
        obj = obj,
        _this = this
      };
    }

    private void HandleRoomCleared(PlayerController obj)
    {
      if (!(bool) (UnityEngine.Object) this || !this.gameObject.activeSelf || !((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null) || (double) UnityEngine.Random.value >= (double) this.ChanceToGainFriendship)
        return;
      obj.StartCoroutine(this.HandleDelayedTalk(obj));
      ++this.m_friendship;
    }

    private void OnDestroy()
    {
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      this.m_owner.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
    }

    private void OnDisable() => this.ClearTextBoxForReal();

    public void DoAmbientTalk(
      Transform baseTransform,
      Vector3 offset,
      string stringKey,
      float duration,
      int index)
    {
      TextBoxManager.ShowTextBox(baseTransform.position + offset, baseTransform, duration, StringTableManager.GetStringSequential(stringKey, ref index, true), string.Empty, false);
    }

    public void InheritData(Gun sourceGun)
    {
      TalkingGunModifier component = sourceGun.GetComponent<TalkingGunModifier>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      this.m_friendship = component.m_friendship;
      this.m_enmityCounter = component.m_enmityCounter;
      this.m_begrudgingCounter = component.m_begrudgingCounter;
      this.m_friendCounter = component.m_friendCounter;
    }

    public void MidGameSerialize(List<object> data, int dataIndex)
    {
      data.Add((object) this.m_friendship);
      data.Add((object) this.m_enmityCounter);
      data.Add((object) this.m_begrudgingCounter);
      data.Add((object) this.m_friendCounter);
    }

    public void MidGameDeserialize(List<object> data, ref int dataIndex)
    {
      this.m_friendship = (int) data[dataIndex];
      this.m_enmityCounter = (int) data[dataIndex + 1];
      this.m_begrudgingCounter = (int) data[dataIndex + 2];
      this.m_friendCounter = (int) data[dataIndex + 3];
      dataIndex += 4;
    }
  }

