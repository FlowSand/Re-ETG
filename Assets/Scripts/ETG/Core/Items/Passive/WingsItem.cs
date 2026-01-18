// Decompiled with JetBrains decompiler
// Type: WingsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class WingsItem : PassiveItem
  {
    public GameObject prefabToAttachToPlayer;
    public string animPrefix = "white_wing";
    public bool usesCardinalAnimations;
    public bool GoopsOnRoll;
    public GoopDefinition RollGoop;
    public float RollGoopRadius = 1f;
    public bool DoesRadialBurstOnDodgeRoll;
    public RadialBurstInterface RadialBurstOnDodgeRoll;
    public bool IsCatThrone;
    [EnemyIdentifier]
    public List<string> CatThroneCharmGuids;
    public float RadialBurstCooldown = 2f;
    private float m_radialBurstCooldown;
    public GameActorCharmEffect CatCharmEffect;
    private GameObject instanceWings;
    private tk2dSprite instanceWingsSprite;
    private bool m_isCurrentlyActive;
    private bool m_hiddenForFall;
    private bool wasRolling;

    private Vector2 GetLocalOffsetForCharacter(PlayableCharacters character)
    {
      switch (character)
      {
        case PlayableCharacters.Pilot:
          return new Vector2(-9f / 16f, -0.5f);
        case PlayableCharacters.Convict:
          return new Vector2(-0.625f, -0.5f);
        case PlayableCharacters.Robot:
          return new Vector2(-9f / 16f, -0.5f);
        case PlayableCharacters.Soldier:
          return new Vector2(-0.5f, -0.5f);
        case PlayableCharacters.Guide:
          return new Vector2(-9f / 16f, -0.5f);
        case PlayableCharacters.Bullet:
          return new Vector2(-9f / 16f, -15f / 32f);
        default:
          return new Vector2(-9f / 16f, -0.5f);
      }
    }

    protected override void Update()
    {
      base.Update();
      if (!((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null) || !this.m_pickedUp)
        return;
      this.m_radialBurstCooldown -= BraveTime.DeltaTime;
      if (this.IsCatThrone && this.wasRolling && !this.m_owner.IsDodgeRolling)
      {
        this.m_owner.IsVisible = true;
        this.wasRolling = false;
      }
      if (this.m_isCurrentlyActive)
      {
        if (this.m_owner.IsFalling)
        {
          this.m_hiddenForFall = true;
          this.instanceWingsSprite.renderer.enabled = false;
        }
        else
        {
          if (this.m_hiddenForFall)
          {
            this.m_hiddenForFall = false;
            this.instanceWingsSprite.renderer.enabled = true;
          }
          string name = this.animPrefix + this.m_owner.GetBaseAnimationSuffix(this.usesCardinalAnimations);
          if (!this.instanceWingsSprite.spriteAnimator.IsPlaying(name) && (!this.IsCatThrone || !this.m_owner.IsDodgeRolling))
            this.instanceWingsSprite.spriteAnimator.Play(name);
          if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
            this.DisengageEffect(this.m_owner);
        }
      }
      else if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
        this.EngageEffect(this.m_owner);
      if (!this.IsCatThrone || !(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(CustomSynergyType.TRUE_CAT_KING) || this.m_owner.CurrentRoom == null)
        return;
      this.ProcessEnemies(this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All));
    }

    private void DoGoop()
    {
      DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.RollGoop).AddGoopCircle(this.m_owner.specRigidbody.UnitCenter, this.RollGoopRadius);
    }

    private void OnRollFrame(PlayerController obj)
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        return;
      if (this.GoopsOnRoll)
        this.DoGoop();
      if (!this.IsCatThrone)
        return;
      this.wasRolling = true;
      obj.IsVisible = false;
      this.instanceWingsSprite.renderer.enabled = true;
      if (this.instanceWingsSprite.spriteAnimator.IsPlaying("cat_throne_spin"))
        return;
      this.instanceWingsSprite.spriteAnimator.Play("cat_throne_spin");
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
        PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
      else
        PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
      if (this.GoopsOnRoll || this.IsCatThrone)
        player.OnIsRolling += new Action<PlayerController>(this.OnRollFrame);
      if (this.DoesRadialBurstOnDodgeRoll)
        player.OnRollStarted += new Action<PlayerController, Vector2>(this.HandleRollStarted);
      this.EngageEffect(player);
      base.Pickup(player);
    }

    private void HandleRollStarted(PlayerController p, Vector2 rollDirection)
    {
      if (!this.DoesRadialBurstOnDodgeRoll || (double) this.m_radialBurstCooldown > 0.0)
        return;
      this.m_radialBurstCooldown = this.RadialBurstCooldown;
      this.RadialBurstOnDodgeRoll.DoBurst(p, spawnPointOffset: new Vector2?(Vector2.up * 0.625f));
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      if (PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
      }
      player.OnIsRolling -= new Action<PlayerController>(this.OnRollFrame);
      player.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
      this.DisengageEffect(player);
      debrisObject.GetComponent<WingsItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if (this.m_pickedUp)
      {
        if (PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
        {
          PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
          if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
            PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
        }
        this.m_owner.OnIsRolling -= new Action<PlayerController>(this.OnRollFrame);
        this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
        this.DisengageEffect(this.m_owner);
      }
      base.OnDestroy();
    }

    protected void EngageEffect(PlayerController user)
    {
      if (Dungeon.IsGenerating || !(bool) (UnityEngine.Object) user || !(bool) (UnityEngine.Object) user.sprite || !(bool) (UnityEngine.Object) user.sprite.GetComponent<tk2dSpriteAttachPoint>())
        return;
      this.m_isCurrentlyActive = true;
      user.SetIsFlying(true, "wings");
      this.instanceWings = user.RegisterAttachedObject(this.prefabToAttachToPlayer, "jetpack", 0.1f);
      this.instanceWingsSprite = this.instanceWings.GetComponent<tk2dSprite>();
      if (!(bool) (UnityEngine.Object) this.instanceWingsSprite)
        this.instanceWingsSprite = this.instanceWings.GetComponentInChildren<tk2dSprite>();
      if (!this.usesCardinalAnimations)
        return;
      this.instanceWingsSprite.transform.localPosition = this.GetLocalOffsetForCharacter(user.characterIdentity).ToVector3ZUp();
    }

    private void ProcessEnemies(List<AIActor> enemies)
    {
      if (enemies == null)
        return;
      for (int index = 0; index < enemies.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) enemies[index] && enemies[index].GetEffect(this.CatCharmEffect.effectIdentifier) == null && this.CatThroneCharmGuids.Contains(enemies[index].EnemyGuid))
          enemies[index].ApplyEffect((GameActorEffect) this.CatCharmEffect);
      }
    }

    protected void DisengageEffect(PlayerController user)
    {
      this.m_isCurrentlyActive = false;
      user.SetIsFlying(false, "wings");
      user.DeregisterAttachedObject(this.instanceWings);
      this.instanceWingsSprite = (tk2dSprite) null;
      if (!this.IsCatThrone || !this.wasRolling)
        return;
      user.IsVisible = true;
      this.wasRolling = false;
    }
  }

