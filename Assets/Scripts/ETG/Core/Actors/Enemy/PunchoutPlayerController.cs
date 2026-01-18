using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PunchoutPlayerController : PunchoutGameActor
  {
    public dfSprite PlayerUiSprite;
    public AIAnimator CoopAnimator;
    public dfSprite starRemovedAnimationPrefab;
    public Vector3 starRemovedOffset;
    [Header("Constants")]
    public float InputBufferTime = 0.1f;
    public float MaxExhaust = 2.9f;
    public float ExhauseRecoveryRate = 0.2f;
    public float BlockStickyFriction = 0.1f;
    public float DuckCameraSway = 0.4f;
    public float DodgeCameraSway = 0.7f;
    public float PunchCameraSway = 0.4f;
    public float SuperBackCameraSway = 0.4f;
    public float SuperForwardCameraSway = 0.4f;
    [Header("Visuals")]
    public Texture2D CosmicTex;
    private PunchoutPlayerController.Action[] m_actions;
    private float[] m_inputLastPressed;
    private int m_playerId;
    private static readonly string[] PlayerNames = new string[7]
    {
      "convict",
      "hunter",
      "marine",
      "pilot",
      "bullet",
      "robot",
      "slinger"
    };
    private static readonly string[] PlayerUiNames = new string[7]
    {
      "punch_player_health_convict_00",
      "punch_player_health_hunter_00",
      "punch_player_health_marine_00",
      "punch_player_health_pilot_00",
      "punch_player_health_bullet_00",
      "punch_player_health_robot_00",
      "punch_player_health_slinger_00"
    };

    public float CurrentExhaust { get; set; }

    public int PlayerID => this.m_playerId;

    public override bool IsDead => this.state is PunchoutPlayerController.DeathState;

    public bool IsSlinger => this.m_playerId == 6;

    public bool IsEevee { get; private set; }

    public bool VfxIsAboveCharacter
    {
      set
      {
        tk2dBaseSprite sprite = this.aiAnimator.ChildAnimator.ChildAnimator.sprite;
        sprite.HeightOffGround = !value ? -0.05f : 0.05f;
        sprite.UpdateZDepth();
      }
    }

    public override void Start()
    {
      base.Start();
      this.m_actions = (PunchoutPlayerController.Action[]) Enum.GetValues(typeof (PunchoutPlayerController.Action));
      this.m_inputLastPressed = new float[this.m_actions.Length];
      for (int index = 0; index < this.m_actions.Length; ++index)
        this.m_inputLastPressed[index] = 100f;
      this.spriteAnimator.AnimationCompleted += new System.Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.HandleAnimationCompletedSwap);
    }

    public override void ManualUpdate()
    {
      base.ManualUpdate();
      this.UpdateInput();
      this.CurrentExhaust = Mathf.Max(0.0f, this.CurrentExhaust - this.ExhauseRecoveryRate * BraveTime.DeltaTime);
      if (this.state != null)
      {
        this.state.Update();
        if (this.state.IsDone)
          this.state = (PunchoutGameActor.State) null;
      }
      this.UpdateState();
    }

    public void UpdateState()
    {
      if (this.state == null && (double) this.CurrentExhaust >= (double) this.MaxExhaust)
      {
        this.state = (PunchoutGameActor.State) new PunchoutPlayerController.ExhaustState();
      }
      else
      {
        if (this.state == null || this.state is PunchoutGameActor.BlockState)
        {
          if (this.WasPressed(PunchoutPlayerController.Action.PunchLeft))
          {
            this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerPunchState(true);
            return;
          }
          if (this.WasPressed(PunchoutPlayerController.Action.PunchRight))
          {
            this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerPunchState(false);
            return;
          }
          if (this.WasPressed(PunchoutPlayerController.Action.Super) && this.Stars > 0)
          {
            this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerSuperState(this.Stars);
            this.Stars = 0;
            return;
          }
        }
        if (this.state is PunchoutGameActor.DuckState && this.CurrentFrame >= 6)
        {
          if (this.WasPressed(PunchoutPlayerController.Action.PunchLeft))
          {
            this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerPunchState(true);
            return;
          }
          if (this.WasPressed(PunchoutPlayerController.Action.PunchRight))
          {
            this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerPunchState(false);
            return;
          }
        }
        if (this.state != null)
          return;
        if (this.WasPressed(PunchoutPlayerController.Action.DodgeLeft))
          this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerDodgeState(true);
        else if (this.WasPressed(PunchoutPlayerController.Action.DodgeRight))
          this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerDodgeState(false);
        else if (this.WasPressed(PunchoutPlayerController.Action.Block))
        {
          this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerBlockState();
        }
        else
        {
          if (!this.WasPressed(PunchoutPlayerController.Action.Duck))
            return;
          this.state = (PunchoutGameActor.State) new PunchoutPlayerController.PlayerDuckState();
        }
      }
    }

    public override void Hit(bool isLeft, float damage, int starsUsed = 0, bool skipProcessing = false)
    {
      if (this.state is PunchoutPlayerController.DeathState)
        return;
      if (this.Stars > 0 && (double) damage >= 4.0)
        this.RemoveStars();
      bool preventDamage = false;
      if (this.state != null)
        this.state.OnHit(ref preventDamage, isLeft, starsUsed);
      int num = (int) AkSoundEngine.PostEvent("Play_CHR_general_hurt_01", this.gameObject);
      if (!this.CoopAnimator.IsPlaying("alarm"))
        this.CoopAnimator.PlayUntilFinished("alarm");
      if ((double) this.Health - (double) damage <= 0.0)
      {
        this.Health = 0.0f;
        this.UpdateUI();
        BraveTime.RegisterTimeScaleMultiplier(0.25f, this.gameObject);
        this.FlashDamage(this.m_playerId != 5 ? 0.66f : 0.25f);
        this.aiAnimator.PlayVfx("death");
        this.state = (PunchoutGameActor.State) new PunchoutPlayerController.DeathState(isLeft);
      }
      else
      {
        this.state = (PunchoutGameActor.State) new PunchoutGameActor.HitState(isLeft);
        this.aiAnimator.PlayVfx("normal_hit");
        this.FlashDamage();
        this.Health -= damage;
        GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Normal, Vibration.Strength.Hard);
        this.UpdateUI();
        this.CurrentExhaust = 0.0f;
      }
    }

    public void SwapPlayer(int? newPlayerIndex = null, bool keepEevee = false)
    {
      if (!newPlayerIndex.HasValue)
        newPlayerIndex = !this.IsEevee || keepEevee ? new int?((this.m_playerId + 1) % (PunchoutPlayerController.PlayerNames.Length + 1)) : new int?(0);
      if (!keepEevee)
      {
        bool flag = newPlayerIndex.Value == 7;
        if (flag && !this.IsEevee)
        {
          this.IsEevee = true;
          this.sprite.usesOverrideMaterial = true;
          this.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
          this.sprite.renderer.sharedMaterial.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
          this.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
          this.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
        }
        else if (!flag && this.IsEevee)
        {
          this.IsEevee = false;
          this.sprite.usesOverrideMaterial = false;
        }
      }
      if (this.IsEevee)
        newPlayerIndex = new int?(UnityEngine.Random.Range(0, PunchoutPlayerController.PlayerNames.Length));
      string playerName1 = PunchoutPlayerController.PlayerNames[this.m_playerId];
      string playerName2 = PunchoutPlayerController.PlayerNames[newPlayerIndex.Value];
      this.m_playerId = newPlayerIndex.Value;
      this.SwapAnim(this.aiAnimator.IdleAnimation, playerName1, playerName2);
      this.SwapAnim(this.aiAnimator.HitAnimation, playerName1, playerName2);
      for (int index = 0; index < this.aiAnimator.OtherAnimations.Count; ++index)
        this.SwapAnim(this.aiAnimator.OtherAnimations[index].anim, playerName1, playerName2);
      this.UpdateUI();
      List<AIAnimator.NamedDirectionalAnimation> otherAnimations = this.aiAnimator.ChildAnimator.OtherAnimations;
      otherAnimations[0].anim.Type = DirectionalAnimation.DirectionType.None;
      otherAnimations[1].anim.Type = DirectionalAnimation.DirectionType.None;
      otherAnimations[2].anim.Type = DirectionalAnimation.DirectionType.None;
      if (this.m_playerId == 4)
      {
        otherAnimations[0].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[0].anim.Prefix = "bullet_super_vfx";
        otherAnimations[1].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[1].anim.Prefix = "bullet_super_final_vfx";
      }
      else if (this.m_playerId == 5)
      {
        otherAnimations[0].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[0].anim.Prefix = "robot_super_vfx";
        otherAnimations[1].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[1].anim.Prefix = "robot_super_final_vfx";
        otherAnimations[2].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[2].anim.Prefix = "robot_knockout_vfx";
      }
      else
      {
        if (this.m_playerId != 6)
          return;
        otherAnimations[0].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[0].anim.Prefix = "slinger_super_vfx";
        otherAnimations[1].anim.Type = DirectionalAnimation.DirectionType.Single;
        otherAnimations[1].anim.Prefix = "slinger_super_final_vfx";
      }
    }

    private void SwapAnim(DirectionalAnimation directionalAnim, string oldName, string newName)
    {
      directionalAnim.Prefix = directionalAnim.Prefix.Replace(oldName, newName);
      for (int index = 0; index < directionalAnim.AnimNames.Length; ++index)
        directionalAnim.AnimNames[index] = directionalAnim.AnimNames[index].Replace(oldName, newName);
    }

    public void Win()
    {
      this.state = (PunchoutGameActor.State) new PunchoutPlayerController.WinState();
      UnityEngine.Object.FindObjectOfType<PunchoutController>().DoWinFade(false);
    }

    public void AddStar()
    {
      this.Stars = Mathf.Min(this.Stars + 1, 3);
      AIAnimator childAnimator = this.aiAnimator.ChildAnimator.ChildAnimator;
      this.VfxIsAboveCharacter = true;
      if (this.Stars == 3)
        childAnimator.PlayUntilFinished("get_star_three");
      else if (this.Stars == 2)
        childAnimator.PlayUntilFinished("get_star_two");
      else
        childAnimator.PlayUntilFinished("get_star_one");
    }

    public void RemoveStars()
    {
      for (int index = 0; index < this.Stars; ++index)
      {
        dfSprite dfSprite = this.StarsUI[index];
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.starRemovedAnimationPrefab.gameObject);
        gameObject.transform.parent = dfSprite.transform.parent;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.layer = dfSprite.gameObject.layer;
        dfSprite component = gameObject.GetComponent<dfSprite>();
        component.BringToFront();
        dfSprite.Parent.AddControl((dfControl) component);
        dfSprite.Parent.BringToFront();
        component.ZOrder = dfSprite.ZOrder - 1;
        component.RelativePosition = dfSprite.RelativePosition + this.starRemovedOffset;
      }
      this.Stars = 0;
      AIAnimator childAnimator = this.aiAnimator.ChildAnimator.ChildAnimator;
      this.VfxIsAboveCharacter = true;
      childAnimator.PlayUntilFinished("lose_stars");
    }

    public void UpdateUI()
    {
      string playerUiName = PunchoutPlayerController.PlayerUiNames[this.m_playerId];
      this.HealthBarUI.SpriteName = "punch_health_bar_001";
      this.PlayerUiSprite.SpriteName = (double) this.Health <= 66.0 ? ((double) this.Health <= 33.0 ? playerUiName + "3" : playerUiName + "2") : playerUiName + "1";
      if (this.IsEevee && (UnityEngine.Object) this.PlayerUiSprite.OverrideMaterial == (UnityEngine.Object) null)
      {
        Material material = UnityEngine.Object.Instantiate<Material>(this.PlayerUiSprite.Atlas.Material);
        material.shader = Shader.Find("Brave/Internal/GlitchEevee");
        material.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
        material.SetFloat("_WaveIntensity", 0.1f);
        material.SetFloat("_ColorIntensity", 0.015f);
        this.PlayerUiSprite.OverrideMaterial = material;
      }
      else
      {
        if (this.IsEevee || !((UnityEngine.Object) this.PlayerUiSprite.OverrideMaterial != (UnityEngine.Object) null))
          return;
        this.PlayerUiSprite.OverrideMaterial = (Material) null;
      }
    }

    public void Exhaust(float? time = null)
    {
      this.state = (PunchoutGameActor.State) new PunchoutPlayerController.ExhaustState(time);
    }

    private void HandleAnimationCompletedSwap(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
    {
      if (!this.IsEevee)
        return;
      this.SwapPlayer(new int?(UnityEngine.Random.Range(0, PunchoutPlayerController.PlayerNames.Length)), true);
      this.sprite.usesOverrideMaterial = true;
      this.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
      this.sprite.renderer.sharedMaterial.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
      this.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
      this.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
    }

    private void UpdateInput()
    {
      if (this.m_inputLastPressed == null)
        return;
      foreach (PunchoutPlayerController.Action action in Enum.GetValues(typeof (PunchoutPlayerController.Action)))
      {
        if (GameManager.HasInstance && !GameManager.Instance.IsPaused && this.WasPressedRaw(action))
          this.m_inputLastPressed[(int) action] = 0.0f;
        else
          this.m_inputLastPressed[(int) action] += BraveTime.DeltaTime;
      }
    }

    private bool WasPressed(PunchoutPlayerController.Action action)
    {
      int index = (int) action;
      bool flag = (double) this.m_inputLastPressed[index] < (double) this.InputBufferTime;
      if (flag)
      {
        this.m_inputLastPressed[index] = 100f;
        if (PunchoutController.InTutorial)
          PunchoutController.InputWasPressed((int) action);
      }
      return flag;
    }

    private bool WasPressedRaw(PunchoutPlayerController.Action action)
    {
      BraveInput instanceForPlayer = !BraveInput.HasInstanceForPlayer(0) ? (BraveInput) null : BraveInput.GetInstanceForPlayer(0);
      if ((bool) (UnityEngine.Object) instanceForPlayer)
      {
        switch (action)
        {
          case PunchoutPlayerController.Action.DodgeLeft:
            return instanceForPlayer.ActiveActions.PunchoutDodgeLeft.WasPressed;
          case PunchoutPlayerController.Action.DodgeRight:
            return instanceForPlayer.ActiveActions.PunchoutDodgeRight.WasPressed;
          case PunchoutPlayerController.Action.Block:
            return instanceForPlayer.ActiveActions.PunchoutBlock.WasPressed;
          case PunchoutPlayerController.Action.Duck:
            return instanceForPlayer.ActiveActions.PunchoutDuck.WasPressed;
          case PunchoutPlayerController.Action.PunchLeft:
            return instanceForPlayer.ActiveActions.PunchoutPunchLeft.WasPressed;
          case PunchoutPlayerController.Action.PunchRight:
            return instanceForPlayer.ActiveActions.PunchoutPunchRight.WasPressed;
          case PunchoutPlayerController.Action.Super:
            return instanceForPlayer.ActiveActions.PunchoutSuper.WasPressed;
        }
      }
      return false;
    }

    public class PlayerBlockState : PunchoutGameActor.BlockState
    {
      public string HitAnimName = "block_hit";
      private bool m_hasBonk;

      public override string AnimName => "block";

      public override void Bonk()
      {
        this.ActorPlayer.VfxIsAboveCharacter = false;
        this.Actor.Play(this.HitAnimName);
        StickyFrictionManager.Instance.RegisterCustomStickyFriction(this.ActorPlayer.BlockStickyFriction, 0.0f, false);
        this.Actor.aiAnimator.PlayVfx("block_ss");
        GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
        if (this.Actor.Opponent.state == null)
          return;
        this.Actor.Opponent.state.WasBlocked = true;
      }

      public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
      {
        base.OnHit(ref preventDamage, isLeft, starsUsed);
        this.ActorPlayer.VfxIsAboveCharacter = false;
        this.Actor.aiAnimator.ChildAnimator.ChildAnimator.PlayUntilFinished("block_break");
      }
    }

    public class PlayerDuckState : PunchoutGameActor.DuckState
    {
      public override void Start()
      {
        base.Start();
        this.Actor.MoveCamera(new Vector2(0.0f, -this.ActorPlayer.DuckCameraSway), 0.2f);
      }

      public override void Stop()
      {
        base.Stop();
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.2f);
      }
    }

    public class PlayerDodgeState : PunchoutGameActor.DodgeState
    {
      public PlayerDodgeState(bool isLeft)
        : base(isLeft)
      {
      }

      public override void Start()
      {
        base.Start();
        this.Actor.MoveCamera(new Vector2(this.ActorPlayer.DodgeCameraSway * (!this.IsLeft ? 1f : -1f), 0.0f), 0.15f);
      }

      public override void OnFrame(int currentFrame)
      {
        base.OnFrame(currentFrame);
        if (currentFrame != 3)
          return;
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.25f);
      }

      public override void Stop()
      {
        base.Stop();
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.15f);
      }
    }

    public class PlayerPunchState : PunchoutGameActor.BasicAttackState
    {
      private bool m_missed;

      public PlayerPunchState(bool isLeft)
        : base(isLeft)
      {
      }

      public override string AnimName => "punch";

      public override int DamageFrame => 1;

      public override float Damage => 5f;

      public override void Start()
      {
        base.Start();
        this.Actor.MoveCamera(new Vector2(0.0f, this.ActorPlayer.PunchCameraSway), 0.04f);
      }

      public override bool CanHitOpponent(PunchoutGameActor.State state)
      {
        if (this.m_missed)
          return false;
        bool flag = !(state is PunchoutGameActor.BlockState) && (state == null || state.CanBeHit(this.IsLeft));
        if (PunchoutController.InTutorial)
        {
          this.ActorPlayer.CurrentExhaust = 0.0f;
          this.m_missed = true;
        }
        else if (!flag)
        {
          this.m_missed = true;
          ++this.ActorPlayer.CurrentExhaust;
          if (this.Actor.Opponent.IsFarAway)
          {
            this.ActorPlayer.Play("punch_miss_far", this.IsLeft);
            AIAnimator aiAnimator = this.Actor.aiAnimator;
            string str = "miss_alert";
            Vector2? nullable = new Vector2?(this.Actor.transform.position.XY() + new Vector2(1f / 16f, 4.25f));
            string name = str;
            Vector2? sourceNormal = new Vector2?();
            Vector2? sourceVelocity = new Vector2?();
            Vector2? position = nullable;
            aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
          }
          else
          {
            this.ActorPlayer.Play("punch_miss", this.IsLeft);
            Vector2 vector2 = new Vector2(!this.IsLeft ? 5f / 16f : -3f / 16f, 4.375f);
            if (this.ActorPlayer.PlayerID == 4)
              vector2.x = 1f / 16f;
            else if (this.ActorPlayer.PlayerID == 5)
              vector2.y += 5f / 16f;
            AIAnimator aiAnimator = this.Actor.Opponent.aiAnimator;
            string str = "block_poof";
            Vector2? nullable = new Vector2?(this.Actor.transform.position.XY() + vector2);
            string name = str;
            Vector2? sourceNormal = new Vector2?();
            Vector2? sourceVelocity = new Vector2?();
            Vector2? position = nullable;
            aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
            if (this.ActorPlayer.IsSlinger)
              this.ActorPlayer.aiAnimator.PlayVfx(!this.IsLeft ? "shoot_right_miss" : "shoot_left_miss");
          }
        }
        else
        {
          this.ActorPlayer.CurrentExhaust = 0.0f;
          if (this.Actor.Opponent.state is PunchoutAIActor.ThrowAmmoState)
          {
            this.m_missed = true;
            this.ActorPlayer.Play("punch_miss_far", this.IsLeft);
            this.Actor.Opponent.aiAnimator.PlayVfx("normal_hit");
          }
          if (this.ActorPlayer.IsSlinger)
            this.ActorPlayer.aiAnimator.PlayVfx(!this.IsLeft ? "shoot_right" : "shoot_left");
        }
        return flag;
      }

      public override void OnFrame(int currentFrame)
      {
        base.OnFrame(currentFrame);
        if ((this.m_missed || currentFrame != 2) && (!this.m_missed || currentFrame != 1))
          return;
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.12f);
      }

      public override void Stop()
      {
        base.Stop();
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.08f);
      }

      public int RealFrame => this.m_missed ? this.Actor.CurrentFrame + 1 : this.Actor.CurrentFrame;
    }

    public class PlayerSuperState : PunchoutGameActor.BasicAttackState
    {
      private int m_starsUsed;
      private bool m_isFinal;

      public PlayerSuperState(int starsUsed)
        : base(false)
      {
        this.m_starsUsed = starsUsed;
      }

      public override string AnimName => "super";

      public override int DamageFrame => 6;

      public override float Damage => (float) (15 * this.m_starsUsed);

      public override void Start()
      {
        base.Start();
        float currentExhaust = this.ActorPlayer.CurrentExhaust;
        PunchoutAIActor opponent = this.Actor.Opponent as PunchoutAIActor;
        if (opponent.Phase == 2 && this.CanHitOpponent(opponent.state) && ((double) this.Actor.Opponent.Health <= (double) this.Damage || opponent.ShouldInstantKO(this.m_starsUsed)))
        {
          if (this.AnimName != null)
            this.Actor.Play("super_final", this.IsLeft);
          this.m_isFinal = true;
        }
        this.ActorPlayer.CurrentExhaust = currentExhaust;
        this.Actor.Opponent.spriteAnimator.Pause();
        this.Actor.Opponent.aiAnimator.ChildAnimator.spriteAnimator.Pause();
        this.Actor.Opponent.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator.Pause();
        this.Actor.MoveCamera(new Vector2(0.0f, -this.ActorPlayer.SuperBackCameraSway), 0.5f);
      }

      public override bool CanHitOpponent(PunchoutGameActor.State state)
      {
        if (this.m_isFinal)
          return true;
        bool flag = !this.Actor.Opponent.IsFarAway;
        if (!flag)
          ++this.ActorPlayer.CurrentExhaust;
        else
          this.ActorPlayer.CurrentExhaust = 0.0f;
        return flag;
      }

      public override void OnFrame(int currentFrame)
      {
        if (currentFrame == this.DamageFrame)
        {
          this.Actor.Opponent.spriteAnimator.Resume();
          this.Actor.Opponent.aiAnimator.ChildAnimator.spriteAnimator.Resume();
          this.Actor.Opponent.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator.Resume();
          if (this.CanHitOpponent(this.Actor.Opponent.state))
            this.Actor.Opponent.Hit(this.IsLeft, this.Damage, this.m_starsUsed);
        }
        if (currentFrame == 6)
        {
          this.Actor.MoveCamera(new Vector2(0.0f, this.ActorPlayer.SuperForwardCameraSway), 0.08f);
        }
        else
        {
          if (currentFrame != 7)
            return;
          this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.5f);
        }
      }

      public override void Stop()
      {
        base.Stop();
        this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.16f);
        if (!this.Actor.Opponent.spriteAnimator.Paused)
          return;
        this.Actor.Opponent.spriteAnimator.Resume();
        this.Actor.Opponent.aiAnimator.ChildAnimator.spriteAnimator.Resume();
        this.Actor.Opponent.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator.Resume();
      }
    }

    public class ExhaustState : PunchoutGameActor.State
    {
      private int m_cycles;
      private bool m_usesExhaustTime;
      private float? m_overrideExhaustTime;

      public ExhaustState(float? overrideExhaustTime = null)
      {
        this.m_overrideExhaustTime = overrideExhaustTime;
      }

      public override string AnimName => "exhaust";

      public int ExhaustCycles => 3;

      public override void Start()
      {
        base.Start();
        if (!this.m_overrideExhaustTime.HasValue)
          return;
        this.Actor.aiAnimator.PlayForDurationOrUntilFinished(this.AnimName, this.m_overrideExhaustTime.Value);
        this.m_usesExhaustTime = true;
      }

      public override void OnFrame(int currentFrame)
      {
        base.OnFrame(currentFrame);
        if (currentFrame != this.Actor.spriteAnimator.CurrentClip.frames.Length - 1)
          return;
        ++this.m_cycles;
        if (this.m_usesExhaustTime || this.m_cycles < this.ExhaustCycles)
          return;
        this.Actor.aiAnimator.EndAnimationIf(this.AnimName);
        this.IsDone = true;
      }

      public override void Stop()
      {
        base.Stop();
        this.ActorPlayer.CurrentExhaust = 0.0f;
      }
    }

    public class DeathState : PunchoutGameActor.State
    {
      private float m_timer;

      public DeathState(bool isLeft)
        : base(isLeft)
      {
      }

      public override void Start()
      {
        base.Start();
        this.Actor.aiAnimator.FacingDirection = !this.IsLeft ? 0.0f : 180f;
        this.ActorPlayer.VfxIsAboveCharacter = true;
        this.Actor.aiAnimator.PlayUntilCancelled("die");
      }

      public override void Update()
      {
        if ((double) this.m_timer >= 3.0)
          return;
        this.m_timer += UnityEngine.Time.unscaledDeltaTime;
        if ((double) this.m_timer > 2.0)
          BraveTime.SetTimeScaleMultiplier(Mathf.Lerp(0.25f, 1f, this.m_timer - 2f), this.Actor.gameObject);
        if ((double) this.m_timer < 3.0)
          return;
        BraveTime.ClearMultiplier(this.Actor.gameObject);
        UnityEngine.Object.FindObjectOfType<PunchoutController>().DoLoseFade(false);
      }

      public override bool CanBeHit(bool isLeft) => false;
    }

    public class WinState : PunchoutGameActor.State
    {
      public override void Start()
      {
        base.Start();
        this.Actor.aiAnimator.PlayUntilCancelled("win");
        this.ActorPlayer.CoopAnimator.PlayUntilCancelled("win");
      }

      public override bool CanBeHit(bool isLeft) => false;
    }

    private enum Action
    {
      DodgeLeft,
      DodgeRight,
      Block,
      Duck,
      PunchLeft,
      PunchRight,
      Super,
    }
  }

