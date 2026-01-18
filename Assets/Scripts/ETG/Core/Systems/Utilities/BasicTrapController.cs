using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class BasicTrapController : TrapController, IPlaceConfigurable
  {
    public BasicTrapController.TriggerMethod triggerMethod;
    [DwarfConfigurable]
    [ShowInInspectorIf("triggerMethod", 2, false)]
    public float triggerTimerDelay = 1f;
    [ShowInInspectorIf("triggerMethod", 2, false)]
    [DwarfConfigurable]
    public float triggerTimerDelay1;
    [ShowInInspectorIf("triggerMethod", 2, false)]
    [DwarfConfigurable]
    public float triggerTimerOffset;
    public BasicTrapController.PlaceableFootprintBuffer footprintBuffer;
    public bool damagesFlyingPlayers;
    public bool triggerOnBlank;
    public bool triggerOnExplosion;
    [Header("Animations")]
    public bool animateChildren;
    [CheckAnimation(null)]
    public string triggerAnimName;
    public float triggerDelay;
    [CheckAnimation(null)]
    public string activeAnimName;
    public List<SpriteAnimatorKiller> activeVfx;
    public float activeTime;
    [CheckAnimation(null)]
    public string resetAnimName;
    public float resetDelay;
    [Header("Damage")]
    public BasicTrapController.DamageMethod damageMethod;
    [FormerlySerializedAs("activeDamage")]
    public float damage;
    public CoreDamageTypes damageTypes;
    [Header("Goop Interactions")]
    public bool IgnitesGoop;
    [NonSerialized]
    public float LocalTimeScale = 1f;
    private RoomHandler m_parentRoom;
    private BasicTrapController.State m_state;
    protected float m_stateTimer;
    protected float m_triggerTimer;
    protected float m_disabledTimer;
    protected IntVector2 m_cachedPosition;
    protected IntVector2 m_cachedPixelMin;
    protected IntVector2 m_cachedPixelMax;
    protected tk2dSpriteAnimator[] m_childrenAnimators;
    protected List<float> m_triggerTimerDelayArray;
    protected int m_triggerTimerDelayIndex;

    protected BasicTrapController.State state
    {
      get => this.m_state;
      set
      {
        if (this.m_state == value)
          return;
        this.EndState(this.m_state);
        this.m_state = value;
        this.BeginState(this.m_state);
      }
    }

    public virtual void Awake()
    {
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
      if (this.animateChildren)
        this.m_childrenAnimators = this.GetComponentsInChildren<tk2dSpriteAnimator>();
      if (!this.triggerOnBlank && !this.triggerOnExplosion)
        return;
      StaticReferenceManager.AllTriggeredTraps.Add(this);
    }

    public override void Start()
    {
      this.m_parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
      this.m_cachedPosition = this.transform.position.IntXY(VectorConversions.Floor);
      this.m_cachedPixelMin = this.m_cachedPosition * PhysicsEngine.Instance.PixelsPerUnit + new IntVector2(this.footprintBuffer.left, this.footprintBuffer.bottom);
      this.m_cachedPixelMax = (this.m_cachedPosition + new IntVector2(this.placeableWidth, this.placeableHeight)) * PhysicsEngine.Instance.PixelsPerUnit - IntVector2.One - new IntVector2(this.footprintBuffer.right, this.footprintBuffer.top);
      if (this.triggerMethod == BasicTrapController.TriggerMethod.Timer)
      {
        this.m_triggerTimerDelayArray = new List<float>();
        if ((double) this.triggerTimerDelay != 0.0)
          this.m_triggerTimerDelayArray.Add(this.triggerTimerDelay);
        if ((double) this.triggerTimerDelay1 != 0.0)
          this.m_triggerTimerDelayArray.Add(this.triggerTimerDelay1);
        if (this.m_triggerTimerDelayArray.Count == 0)
          this.m_triggerTimerDelayArray.Add(0.0f);
        this.m_triggerTimer = this.triggerTimerOffset;
      }
      for (int index = 0; index < this.activeVfx.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) this.activeVfx[index])
        {
          this.activeVfx[index].onlyDisable = true;
          this.activeVfx[index].Disable();
        }
      }
      base.Start();
    }

    public virtual void Update()
    {
      if ((double) UnityEngine.Time.timeScale == 0.0 || !GameManager.Instance.PlayerIsNearRoom(this.m_parentRoom))
        return;
      this.m_stateTimer = Mathf.Max(0.0f, this.m_stateTimer - BraveTime.DeltaTime) * this.LocalTimeScale;
      this.m_triggerTimer -= BraveTime.DeltaTime * this.LocalTimeScale;
      this.m_disabledTimer = Mathf.Max(0.0f, this.m_disabledTimer - BraveTime.DeltaTime * this.LocalTimeScale);
      if (this.triggerMethod == BasicTrapController.TriggerMethod.Timer && (double) this.m_triggerTimer < 0.0)
        this.TriggerTrap((SpeculativeRigidbody) null);
      this.UpdateState();
    }

    protected override void OnDestroy()
    {
      if (this.triggerOnBlank || this.triggerOnExplosion)
        StaticReferenceManager.AllTriggeredTraps.Remove(this);
      base.OnDestroy();
    }

    public override GameObject InstantiateObject(
      RoomHandler targetRoom,
      IntVector2 loc,
      bool deferConfiguration = false)
    {
      return base.InstantiateObject(targetRoom, loc, deferConfiguration);
    }

    private void OnTriggerCollision(
      SpeculativeRigidbody rigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      PlayerController component = rigidbody.GetComponent<PlayerController>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      bool flag = component.spriteAnimator.QueryGroundedFrame() && !component.IsFlying;
      if (this.triggerMethod == BasicTrapController.TriggerMethod.SpecRigidbody && this.m_state == BasicTrapController.State.Ready && flag)
        this.TriggerTrap(rigidbody);
      if (this.damageMethod != BasicTrapController.DamageMethod.SpecRigidbody || this.m_state != BasicTrapController.State.Active || !flag && !this.damagesFlyingPlayers)
        return;
      this.Damage(rigidbody);
    }

    public void Trigger() => this.TriggerTrap((SpeculativeRigidbody) null);

    protected virtual void TriggerTrap(SpeculativeRigidbody target)
    {
      if ((double) this.m_disabledTimer > 0.0 || this.m_state != BasicTrapController.State.Ready)
        return;
      this.state = BasicTrapController.State.Triggered;
      if (this.damageMethod != BasicTrapController.DamageMethod.OnTrigger)
        return;
      this.Damage(target);
    }

    protected bool ArePlayersNearby()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].CurrentRoom == this.m_parentRoom)
          return true;
      }
      return false;
    }

    protected bool ArePlayersSortOfNearby()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].CurrentRoom != null && GameManager.Instance.AllPlayers[index].CurrentRoom.connectedRooms != null && GameManager.Instance.AllPlayers[index].CurrentRoom.connectedRooms.Contains(this.m_parentRoom))
          return true;
      }
      return false;
    }

    protected virtual void BeginState(BasicTrapController.State newState)
    {
      bool flag1 = this.ArePlayersNearby();
      bool flag2 = flag1 || this.ArePlayersSortOfNearby();
      if (this.m_state == BasicTrapController.State.Triggered)
      {
        this.PlayAnimation(this.triggerAnimName);
        this.m_stateTimer = this.triggerDelay;
        if (this.triggerMethod == BasicTrapController.TriggerMethod.Timer)
          this.m_triggerTimer += this.GetNextTriggerTimerDelay();
        if ((double) this.m_stateTimer == 0.0)
          this.state = BasicTrapController.State.Active;
        if (!flag1)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_trigger", this.gameObject);
      }
      else if (this.m_state == BasicTrapController.State.Active)
      {
        this.PlayAnimation(this.activeAnimName);
        if (flag2)
          this.SpawnVfx(this.activeVfx);
        this.m_stateTimer = this.activeTime;
        if ((double) this.m_stateTimer == 0.0)
          this.state = BasicTrapController.State.Resetting;
        if (!flag1)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_active", this.gameObject);
      }
      else
      {
        if (this.m_state != BasicTrapController.State.Resetting)
          return;
        this.PlayAnimation(this.resetAnimName);
        this.m_stateTimer = this.resetDelay;
        if ((double) this.m_stateTimer == 0.0)
          this.state = BasicTrapController.State.Ready;
        if (!flag1)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_ENV_trap_reset", this.gameObject);
      }
    }

    protected virtual void UpdateState()
    {
      if (this.m_state == BasicTrapController.State.Ready)
      {
        if (this.triggerMethod != BasicTrapController.TriggerMethod.PlaceableFootprint)
          return;
        SpeculativeRigidbody rigidbodyInFootprint = this.GetPlayerRigidbodyInFootprint();
        if (!(bool) (UnityEngine.Object) rigidbodyInFootprint)
          return;
        bool flag = rigidbodyInFootprint.spriteAnimator.QueryGroundedFrame();
        if ((UnityEngine.Object) rigidbodyInFootprint.gameActor != (UnityEngine.Object) null)
          flag = flag && !rigidbodyInFootprint.gameActor.IsFlying;
        if (!flag)
          return;
        this.TriggerTrap((SpeculativeRigidbody) null);
      }
      else if (this.m_state == BasicTrapController.State.Triggered)
      {
        if ((double) this.m_stateTimer != 0.0)
          return;
        this.state = BasicTrapController.State.Active;
      }
      else if (this.m_state == BasicTrapController.State.Active)
      {
        if (this.damageMethod == BasicTrapController.DamageMethod.PlaceableFootprint)
        {
          SpeculativeRigidbody rigidbodyInFootprint = this.GetPlayerRigidbodyInFootprint();
          if ((bool) (UnityEngine.Object) rigidbodyInFootprint)
          {
            bool flag = rigidbodyInFootprint.spriteAnimator.QueryGroundedFrame();
            if ((UnityEngine.Object) rigidbodyInFootprint.gameActor != (UnityEngine.Object) null)
              flag = flag && !rigidbodyInFootprint.gameActor.IsFlying;
            if (flag || this.damagesFlyingPlayers)
              this.Damage(rigidbodyInFootprint);
          }
        }
        if (this.IgnitesGoop)
          DeadlyDeadlyGoopManager.IgniteGoopsCircle(this.sprite.WorldCenter, 1f);
        if ((double) this.m_stateTimer != 0.0)
          return;
        this.state = BasicTrapController.State.Resetting;
      }
      else
      {
        if (this.m_state != BasicTrapController.State.Resetting || (double) this.m_stateTimer != 0.0)
          return;
        this.state = BasicTrapController.State.Ready;
      }
    }

    protected virtual void EndState(BasicTrapController.State newState)
    {
    }

    public void TemporarilyDisableTrap(float disableTime)
    {
      this.m_disabledTimer = Mathf.Max(disableTime, this.m_disabledTimer);
    }

    public Vector2 CenterPoint()
    {
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        return this.specRigidbody.UnitCenter;
      return this.triggerMethod == BasicTrapController.TriggerMethod.PlaceableFootprint ? new Vector2((float) (this.m_cachedPixelMin.x + this.m_cachedPixelMax.x), (float) (this.m_cachedPixelMin.y + this.m_cachedPixelMax.y)) / 32f : (Vector2) this.transform.position;
    }

    protected virtual void PlayAnimation(string animationName)
    {
      if (string.IsNullOrEmpty(animationName))
        return;
      if (this.animateChildren)
      {
        if (this.m_childrenAnimators == null)
          return;
        for (int index = 0; index < this.m_childrenAnimators.Length; ++index)
        {
          if ((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) this.m_childrenAnimators[index])
            this.m_childrenAnimators[index].Play(animationName);
        }
      }
      else
        this.spriteAnimator.Play(animationName);
    }

    protected virtual void SpawnVfx(List<SpriteAnimatorKiller> vfx)
    {
      for (int index = 0; index < vfx.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) vfx[index])
          vfx[index].Restart();
      }
    }

    protected virtual SpeculativeRigidbody GetPlayerRigidbodyInFootprint()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if (!((UnityEngine.Object) allPlayer == (UnityEngine.Object) null))
        {
          PixelCollider primaryPixelCollider = allPlayer.specRigidbody.PrimaryPixelCollider;
          if (primaryPixelCollider != null && this.m_cachedPixelMin.x <= primaryPixelCollider.MaxX && this.m_cachedPixelMax.x >= primaryPixelCollider.MinX && this.m_cachedPixelMin.y <= primaryPixelCollider.MaxY && this.m_cachedPixelMax.y >= primaryPixelCollider.MinY)
            return allPlayer.specRigidbody;
        }
      }
      return (SpeculativeRigidbody) null;
    }

    protected virtual void Damage(SpeculativeRigidbody rigidbody)
    {
      if ((double) this.damage <= 0.0 || !(bool) (UnityEngine.Object) rigidbody || !(bool) (UnityEngine.Object) rigidbody.healthHaver || !rigidbody.healthHaver.IsVulnerable || (bool) (UnityEngine.Object) rigidbody.gameActor && rigidbody.gameActor.IsFalling)
        return;
      rigidbody.healthHaver.ApplyDamage(this.damage, Vector2.zero, StringTableManager.GetEnemiesString("#TRAP"), this.damageTypes);
    }

    protected float GetNextTriggerTimerDelay()
    {
      float triggerTimerDelay = this.m_triggerTimerDelayArray[this.m_triggerTimerDelayIndex];
      this.m_triggerTimerDelayIndex = (this.m_triggerTimerDelayIndex + 1) % this.m_triggerTimerDelayArray.Count;
      return triggerTimerDelay;
    }

    public void ConfigureOnPlacement(RoomHandler room)
    {
      IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
      for (int x = 0; x < this.placeableWidth; ++x)
      {
        for (int y = 0; y < this.placeableHeight; ++y)
        {
          IntVector2 key = new IntVector2(x, y) + intVector2;
          GameManager.Instance.Dungeon.data[key].cellVisualData.containsObjectSpaceStamp = true;
          GameManager.Instance.Dungeon.data[key].cellVisualData.containsWallSpaceStamp = true;
        }
      }
      room.ForcePreventChannels = true;
    }

    public enum TriggerMethod
    {
      SpecRigidbody,
      PlaceableFootprint,
      Timer,
      Script,
    }

    public enum DamageMethod
    {
      SpecRigidbody,
      PlaceableFootprint,
      OnTrigger,
    }

    protected enum State
    {
      Ready,
      Triggered,
      Active,
      Resetting,
    }

    [Serializable]
    public class PlaceableFootprintBuffer
    {
      public int left;
      public int bottom;
      public int right;
      public int top;
    }
  }

