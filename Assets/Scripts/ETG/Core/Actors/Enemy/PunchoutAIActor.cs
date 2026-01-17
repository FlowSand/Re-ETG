// Decompiled with JetBrains decompiler
// Type: PunchoutAIActor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Enemy
{
    public class PunchoutAIActor : PunchoutGameActor
    {
      public dfSprite RatUiSprite;
      public tk2dSpriteAnimator BoxAnimator;
      public GameObject HealParticleVfx;
      public GameObject BoxShellVfx;
      public GameObject BoxShellBackVfx;
      public GameObject DroppedItemPrefab;
      [Header("Constants")]
      public float TauntCooldown = 5f;
      public int MaxHeals = 2;
      public Vector2 BoxStart;
      public Vector2 BoxEnd;
      public float BoxThrowTime;
      public float BoxCounterStartTime;
      public float BoxCounterReturnTime;
      public Color RedPulseColor;
      [Header("AI Phase 1")]
      public float p1AttackChance;
      public float p1TauntChance;
      public float p1PunchChance;
      public float p1UppercutChance;
      [Header("AI Phase 2")]
      public float p2AttackChance;
      public float p2TauntChance;
      public float p2SneakChance;
      public float p2ComboChance;
      public float p2TailwhipChance;
      public float p2ThrowAmmoChance;
      [Header("AI Phase 3")]
      public float p3AttackChance;
      public float p3TauntChance;
      public float p3SneakChance;
      public float p3ComboChance;
      public float p3TailwhipChance;
      public float p3ThrowAmmoChance;
      public float p3BrassKnucklesChance;
      public float p3TailTornadoChance;
      public List<int> DroppedRewardIds = new List<int>();
      private PunchoutController m_punchoutController;
      private float m_punishTimer;
      private Vector2 m_startPosition;
      private bool m_droppedFirstKey;
      private int m_hitUntilFirstDrop = 5;
      private bool m_hasDroppedStarDrop;
      private int m_glassGuonsDropped;

      public int Phase { get; set; }

      public float TauntCooldownTimer { get; set; }

      public int SuccessfulHeals { get; set; }

      public int NumKeysDropped { get; set; }

      public int NumTimesTripleStarred { get; set; }

      public override bool IsDead => this.state is PunchoutAIActor.DeathState;

      public override void Start()
      {
        base.Start();
        this.m_punchoutController = Object.FindObjectOfType<PunchoutController>();
        this.state = (PunchoutGameActor.State) new PunchoutAIActor.IntroState();
        float num1 = this.p1TauntChance + this.p1PunchChance + this.p1UppercutChance;
        this.p1TauntChance /= num1;
        this.p1PunchChance /= num1;
        this.p1UppercutChance /= num1;
        float num2 = this.p2TauntChance + this.p2SneakChance + this.p2ComboChance + this.p2TailwhipChance + this.p2ThrowAmmoChance;
        this.p2TauntChance /= num2;
        this.p2SneakChance /= num2;
        this.p2ComboChance /= num2;
        this.p2TailwhipChance /= num2;
        this.p2ThrowAmmoChance /= num2;
        float num3 = this.p3TauntChance + this.p3SneakChance + this.p3ComboChance + this.p3TailwhipChance + this.p3ThrowAmmoChance + this.p3BrassKnucklesChance + this.p3TailTornadoChance;
        this.p3TauntChance /= num3;
        this.p3SneakChance /= num3;
        this.p3ComboChance /= num3;
        this.p3TailwhipChance /= num3;
        this.p3ThrowAmmoChance /= num3;
        this.p3BrassKnucklesChance /= num3;
        this.p3TailTornadoChance /= num3;
        this.m_startPosition = (Vector2) this.transform.localPosition;
        this.m_hitUntilFirstDrop = Random.Range(5, 9);
      }

      public override void ManualUpdate()
      {
        base.ManualUpdate();
        this.m_punishTimer = Mathf.Max(0.0f, this.m_punishTimer - BraveTime.DeltaTime);
        this.TauntCooldownTimer = Mathf.Max(0.0f, this.TauntCooldownTimer - BraveTime.DeltaTime);
        bool finishedThisFrame = false;
        if (this.state != null)
        {
          this.state.Update();
          if (this.state.IsDone)
          {
            if ((double) this.state.PunishTime > 0.0 && !this.state.WasBlocked)
              this.m_punishTimer = this.state.PunishTime;
            this.state = (PunchoutGameActor.State) null;
            finishedThisFrame = true;
          }
        }
        if (this.state != null)
          return;
        this.state = this.GetNextState(finishedThisFrame);
      }

      public PunchoutGameActor.State GetNextState(bool finishedThisFrame)
      {
        if ((double) this.m_punishTimer > 0.0)
          return (PunchoutGameActor.State) null;
        if (this.Opponent.state is PunchoutPlayerController.DeathState && this.state == null)
          return (PunchoutGameActor.State) new PunchoutAIActor.WinState();
        if (this.Opponent.state is PunchoutGameActor.BasicAttackState)
          return this.Opponent.aiAnimator.IsPlaying("super") ? (PunchoutGameActor.State) new PunchoutAIActor.PunchState(BraveUtility.RandomBool(), true) : (PunchoutGameActor.State) new PunchoutGameActor.BlockState();
        if (finishedThisFrame)
          return (PunchoutGameActor.State) null;
        if (this.Phase == 0)
        {
          if ((double) Random.value < (double) BraveMathCollege.SliceProbability(this.p1AttackChance, BraveTime.DeltaTime))
          {
            float num1 = (double) this.TauntCooldownTimer > 0.0 ? Random.RandomRange(0.0f, 1f - this.p1TauntChance) : Random.value;
            if ((double) num1 < (double) this.p1PunchChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.PunchState(BraveUtility.RandomBool(), true);
            float num2 = num1 - this.p1PunchChance;
            if ((double) num2 < (double) this.p1UppercutChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.UppercutState(BraveUtility.RandomBool());
            float num3 = num2 - this.p1UppercutChance;
            if ((double) num3 < (double) this.p1TauntChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.LaughTauntState();
            float num4 = num3 - this.p1TauntChance;
          }
          return (PunchoutGameActor.State) null;
        }
        if (this.Phase == 1)
        {
          if ((double) Random.value < (double) BraveMathCollege.SliceProbability(this.p2AttackChance, BraveTime.DeltaTime))
          {
            float num5 = (double) this.TauntCooldownTimer > 0.0 || (double) this.Health >= 100.0 || this.SuccessfulHeals >= this.MaxHeals ? Random.RandomRange(0.0f, 1f - this.p2TauntChance) : Random.value;
            if ((double) num5 < (double) this.p2SneakChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.SuperAttackState();
            float num6 = num5 - this.p2SneakChance;
            if ((double) num6 < (double) this.p2ComboChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.PunchBasicComboState(BraveUtility.RandomBool());
            float num7 = num6 - this.p2ComboChance;
            if ((double) num7 < (double) this.p2TailwhipChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.TailWhipState();
            float num8 = num7 - this.p2TailwhipChance;
            if ((double) num8 < (double) this.p2ThrowAmmoChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.ThrowAmmoState(BraveUtility.RandomBool());
            float num9 = num8 - this.p2ThrowAmmoChance;
            if ((double) num9 < (double) this.p2TauntChance)
              return (PunchoutGameActor.State) new PunchoutAIActor.CheeseTauntState();
            float num10 = num9 - this.p2TauntChance;
          }
          return (PunchoutGameActor.State) null;
        }
        if (this.Phase != 2)
          return (PunchoutGameActor.State) null;
        if ((double) Random.value < (double) BraveMathCollege.SliceProbability(this.p3AttackChance, BraveTime.DeltaTime))
        {
          float num11 = (double) this.TauntCooldownTimer > 0.0 || (double) this.Health >= 100.0 || this.SuccessfulHeals >= this.MaxHeals ? Random.RandomRange(0.0f, 1f - this.p3TauntChance) : Random.value;
          if ((double) num11 < (double) this.p3SneakChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.SuperAttackState();
          float num12 = num11 - this.p3SneakChance;
          if ((double) num12 < (double) this.p3ComboChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.PunchBasicComboState(BraveUtility.RandomBool());
          float num13 = num12 - this.p3ComboChance;
          if ((double) num13 < (double) this.p3TailwhipChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.TailWhipState();
          float num14 = num13 - this.p3TailwhipChance;
          if ((double) num14 < (double) this.p3ThrowAmmoChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.ThrowAmmoState(BraveUtility.RandomBool());
          float num15 = num14 - this.p3ThrowAmmoChance;
          if ((double) num15 < (double) this.p3BrassKnucklesChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.BrassKnucklesPunchState(BraveUtility.RandomBool());
          float num16 = num15 - this.p3BrassKnucklesChance;
          if ((double) num16 < (double) this.p3TailTornadoChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.SuperTailWhipState();
          float num17 = num16 - this.p3TailTornadoChance;
          if ((double) num17 < (double) this.p3TauntChance)
            return (PunchoutGameActor.State) new PunchoutAIActor.CheeseTauntState();
          float num18 = num17 - this.p3TauntChance;
        }
        return (PunchoutGameActor.State) null;
      }

      public override void Hit(bool isLeft, float damage, int starsUsed = 0, bool skipProcessing = false)
      {
        PunchoutGameActor.State state = this.state;
        bool preventDamage = false;
        if (this.state != null && !skipProcessing)
        {
          if (this.state.ShouldInstantKO(starsUsed))
          {
            if (starsUsed > 0 && !this.m_hasDroppedStarDrop)
            {
              this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.A);
              this.m_hasDroppedStarDrop = true;
            }
            if (starsUsed >= 3)
            {
              ++this.NumTimesTripleStarred;
              UnityEngine.Debug.LogWarningFormat("Hit by 3 stars {0} times", (object) this.NumTimesTripleStarred);
            }
            this.aiAnimator.PlayVfx("star_hit");
            int num = (int) AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", this.gameObject);
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Slow, Vibration.Strength.Hard);
            this.Knockdown(isLeft, true);
            return;
          }
          this.state.OnHit(ref preventDamage, isLeft, starsUsed);
        }
        if (!preventDamage)
        {
          if (this.IsYellow)
            ((PunchoutPlayerController) this.Opponent).AddStar();
          if (!this.m_droppedFirstKey)
          {
            this.DropKey(isLeft);
            this.m_droppedFirstKey = true;
          }
          if (this.m_hitUntilFirstDrop > 0)
          {
            --this.m_hitUntilFirstDrop;
            if (this.m_hitUntilFirstDrop == 0)
              this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.COMMON, PickupObject.ItemQuality.D, PickupObject.ItemQuality.C);
          }
          if ((double) Random.value < (double) this.m_punchoutController.NormalHitRewardChance)
            this.DropReward(isLeft);
          this.aiAnimator.PlayVfx(starsUsed <= 0 ? "normal_hit" : "star_hit");
          int num = (int) AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", this.gameObject);
          if (starsUsed > 0)
          {
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Slow, Vibration.Strength.Hard);
            if (!this.m_hasDroppedStarDrop)
            {
              this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.A);
              this.m_hasDroppedStarDrop = true;
            }
          }
          else
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
          if (starsUsed > 0 && !(this.state is PunchoutAIActor.InstantKnockdownState))
          {
            if ((double) this.Health - (double) damage > 0.0)
            {
              this.state = (PunchoutGameActor.State) new PunchoutAIActor.SuperHitState();
            }
            else
            {
              if (starsUsed >= 3)
              {
                ++this.NumTimesTripleStarred;
                UnityEngine.Debug.LogWarningFormat("Hit by 3 stars {0} times", (object) this.NumTimesTripleStarred);
              }
              this.Knockdown(isLeft, true);
              return;
            }
          }
          else if (this.state == state && !skipProcessing)
          {
            if (this.state is PunchoutAIActor.HitState)
            {
              (this.state as PunchoutAIActor.HitState).HitAgain(isLeft);
            }
            else
            {
              int remainingHits = !(this.state is PunchoutAIActor.DazeState) ? 3 : 5;
              this.state = (PunchoutGameActor.State) new PunchoutAIActor.HitState(isLeft, remainingHits);
            }
          }
          this.LastHitBy = this.Opponent.state;
          this.Health -= damage;
          this.FlashDamage();
          if (skipProcessing && (double) this.Health < 1.0)
            this.Health = 1f;
        }
        else
          GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        if ((double) this.Health > 0.0 || skipProcessing)
          return;
        if (this.Phase == 0)
          this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.C);
        else
          this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.B);
        this.GoToNextPhase(new bool?(isLeft), starsUsed > 0);
      }

      private void DropKey(bool isLeft) => this.StartCoroutine(this.DropKeyCR(isLeft));

      [DebuggerHidden]
      private IEnumerator DropKeyCR(bool isLeft)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PunchoutAIActor__DropKeyCRc__Iterator0()
        {
          isLeft = isLeft,
          _this = this
        };
      }

      private void DropReward(bool isLeft, params PickupObject.ItemQuality[] targetQualities)
      {
        this.StartCoroutine(this.DropRewardCR(isLeft, targetQualities));
      }

      [DebuggerHidden]
      private IEnumerator DropRewardCR(bool isLeft, params PickupObject.ItemQuality[] targetQualities)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PunchoutAIActor__DropRewardCRc__Iterator1()
        {
          targetQualities = targetQualities,
          isLeft = isLeft,
          _this = this
        };
      }

      public void Knockdown(bool isLeft, bool triggeredBySuper)
      {
        this.Health = 0.0f;
        if (this.Phase < 2)
        {
          if (this.Phase == 0)
            this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.C);
          else
            this.DropReward((isLeft ? 1 : 0) != 0, PickupObject.ItemQuality.B);
          this.state = (PunchoutGameActor.State) new PunchoutAIActor.InstantKnockdownState(isLeft);
        }
        else
          this.state = (PunchoutGameActor.State) new PunchoutAIActor.DeathState(isLeft, triggeredBySuper);
      }

      public void GoToNextPhase(bool? isLeft, bool triggeredBySuper)
      {
        if (!isLeft.HasValue)
          isLeft = new bool?(false);
        if (this.Phase < 2)
          this.state = (PunchoutGameActor.State) new PunchoutAIActor.PhaseTransitionState(isLeft.Value, this.Health);
        else
          this.state = (PunchoutGameActor.State) new PunchoutAIActor.DeathState(isLeft.Value, triggeredBySuper);
      }

      public void UpdateUI(int phase = -1)
      {
        if (phase < 0)
          phase = this.Phase;
        if (phase == 0)
        {
          this.HealthBarUI.SpriteName = "punch_health_bar_green_001";
          this.RatUiSprite.SpriteName = "punch_boss_health_rat_001";
        }
        else if (phase == 1)
        {
          this.HealthBarUI.SpriteName = "punch_health_bar_yellow_001";
          this.RatUiSprite.SpriteName = "punch_boss_health_rat_002";
        }
        else
        {
          this.HealthBarUI.SpriteName = "punch_health_bar_001";
          this.RatUiSprite.SpriteName = "punch_boss_health_rat_003";
        }
      }

      public bool ShouldInstantKO(int starsUsed)
      {
        return this.state != null && this.state.ShouldInstantKO(starsUsed);
      }

      public void DoFadeOut()
      {
        this.aiAnimator.PlayVfx("bomb_explosion");
        this.m_punchoutController.DoBombFade();
      }

      public void DoHealSuck(Vector3 deltaPos)
      {
        SpawnManager.SpawnVFX(this.HealParticleVfx, this.transform.position + deltaPos, Quaternion.Euler(-45f, 0.0f, 0.0f)).GetComponent<ParticleKiller>().ForceInit();
      }

      public void DoBoxShells(Vector3 deltaPos)
      {
        SpawnManager.SpawnVFX(this.BoxShellVfx, this.transform.position + deltaPos, Quaternion.Euler(325f, 0.0f, 0.0f)).GetComponent<ParticleKiller>().ForceInit();
      }

      public void DoBoxShellsBack(Vector3 deltaPos, bool isLeft)
      {
        SpawnManager.SpawnVFX(this.BoxShellBackVfx, this.transform.position + deltaPos + new Vector3(0.0f, 0.0f, 1.75f), Quaternion.Euler(340f, !isLeft ? 225f : 135f, 180f)).GetComponent<ParticleKiller>().ForceInit();
      }

      public void Reset()
      {
        this.Phase = 0;
        this.Health = 100f;
        this.UpdateUI(0);
        this.NumKeysDropped = 0;
        this.DroppedRewardIds.Clear();
        this.SuccessfulHeals = 0;
        this.m_droppedFirstKey = false;
        this.m_hitUntilFirstDrop = Random.Range(5, 9);
        this.m_hasDroppedStarDrop = false;
        this.m_glassGuonsDropped = 0;
        this.NumTimesTripleStarred = 0;
        if (this.state is PunchoutAIActor.DeathState)
        {
          this.state.IsDone = true;
          this.aiAnimator.EndAnimationIf("die");
        }
        this.aiAnimator.EndAnimation();
        this.state = (PunchoutGameActor.State) new PunchoutAIActor.IntroState();
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
        this.transform.localPosition = this.m_startPosition.ToVector3ZUp(this.transform.localPosition.z);
        this.Opponent.state = (PunchoutGameActor.State) null;
        this.Opponent.Health = 0.0f;
        this.Opponent.aiAnimator.EndAnimation();
        (this.Opponent as PunchoutPlayerController).CurrentExhaust = 0.0f;
        (this.Opponent as PunchoutPlayerController).Stars = 0;
      }

      public class PunchState : PunchoutGameActor.BasicAttackState
      {
        private bool m_missed;
        private bool m_canWhiff;

        public PunchState(bool isLeft, bool canWhiff)
          : base(isLeft)
        {
          this.m_canWhiff = canWhiff;
        }

        public override string AnimName => "punch";

        public override int DamageFrame => 7;

        public override float Damage => 10f;

        public override bool CanHitOpponent(PunchoutGameActor.State state)
        {
          if (this.m_missed)
            return false;
          bool flag = !(state is PunchoutGameActor.DuckState);
          if (state is PunchoutGameActor.BlockState)
          {
            (state as PunchoutGameActor.BlockState).Bonk();
            flag = false;
          }
          if (state is PunchoutGameActor.DodgeState dodgeState && dodgeState.IsLeft != this.IsLeft)
            flag = false;
          if (flag || !this.m_canWhiff)
            return flag;
          this.Actor.Play("punch_miss", this.IsLeft);
          this.m_missed = true;
          return false;
        }

        public override bool CanBeHit(bool isLeft)
        {
          if (this.WasBlocked)
            return false;
          if (this.m_missed)
            return true;
          return this.Actor.CurrentFrame >= 3 && (double) this.Actor.CurrentFrameFloat < 5.5;
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (this.m_missed || currentFrame != 3)
            return;
          this.Actor.FlashWarn(2.5f);
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          if (this.m_missed || this.Actor.CurrentFrame != 4 && this.Actor.CurrentFrame != 5)
            return;
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }
      }

      public class UppercutState : PunchoutGameActor.BasicAttackState
      {
        public UppercutState(bool isLeft) : base(isLeft)
        {
        }

        public override string AnimName => "uppercut";

        public override int DamageFrame => 8;

        public override float Damage => 20f;

        public override float PunishTime => 0.3f;

        public override bool CanHitOpponent(PunchoutGameActor.State state)
        {
          switch (state)
          {
            case PunchoutGameActor.BlockState _:
              (state as PunchoutGameActor.BlockState).Bonk();
              return false;
            case PunchoutGameActor.DuckState _:
              return false;
            case PunchoutGameActor.DodgeState dodgeState:
              if (dodgeState.IsLeft != this.IsLeft)
                return false;
              break;
          }
          return true;
        }

        public override bool CanBeHit(bool isLeft)
        {
          return !this.WasBlocked && this.Actor.CurrentFrame > this.DamageFrame;
        }
      }

      public class SuperAttackState : PunchoutGameActor.BasicAttackState
      {
        public override string AnimName => "super";

        public override int DamageFrame => 16 /*0x10*/;

        public override float Damage => 35f;

        public override float PunishTime => 0.3f;

        public override bool CanHitOpponent(PunchoutGameActor.State state)
        {
          return !(state is PunchoutGameActor.DuckState);
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (currentFrame != 15)
            return;
          this.Actor.FlashWarn(1f);
        }

        public override bool CanBeHit(bool isLeft)
        {
          if (this.WasBlocked)
            return false;
          return this.Actor.CurrentFrame == 15 || this.Actor.CurrentFrame >= 17;
        }

        public override bool IsFarAway()
        {
          return this.Actor.CurrentFrame >= 2 && this.Actor.CurrentFrame <= 15;
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          if (this.Actor.CurrentFrame != 15)
            return;
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }
      }

      public class TailWhipState : PunchoutGameActor.BasicAttackState
      {
        public override string AnimName => "tail_whip";

        public override int DamageFrame => 11;

        public override float Damage => 20f;

        public override float PunishTime => 0.3f;

        public override void Update()
        {
          base.Update();
          this.WasBlocked = false;
        }

        public override bool CanHitOpponent(PunchoutGameActor.State state)
        {
          if (!(state is PunchoutGameActor.BlockState))
            return true;
          (state as PunchoutGameActor.BlockState).Bonk();
          return false;
        }

        public override bool CanBeHit(bool isLeft)
        {
          return (double) this.Actor.CurrentFrameFloat >= 8.5 && this.Actor.CurrentFrame <= 10;
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.TailDazeState();
        }

        public override bool ShouldInstantKO(int starsUsed) => starsUsed >= 1;
      }

      public class ThrowAmmoState : PunchoutGameActor.State
      {
        public ThrowAmmoState(bool isLeft) : base(isLeft)
        {
        }

        public float SwitchChance = 0.33f;
        public float Damage = 20f;
        public float ReturnDamage = 20f;
        private PunchoutAIActor.ThrowAmmoState.ThrowState m_state;
        private bool m_hasThrown;
        private Vector2 m_boxStartPos;
        private Vector2 m_boxEndPos;
        private float m_boxThrowTime;
        private float m_boxThrowTimer;

        public override void Start()
        {
          this.Actor.Play("throw_intro", this.IsLeft);
          this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Intro;
          this.Actor.MoveCamera(new Vector2(0.0f, 1f), 0.5f);
          base.Start();
        }

        public override void Update()
        {
          base.Update();
          if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Throw && this.m_hasThrown)
          {
            this.m_boxThrowTimer += BraveTime.DeltaTime;
            float t = this.m_boxThrowTimer / this.m_boxThrowTime;
            Vector2 vector = Vector2.Lerp(this.m_boxStartPos, this.m_boxEndPos, t);
            vector.y += Mathf.Sin(t * 3.14159274f) * 0.5f;
            this.ActorEnemy.BoxAnimator.transform.localPosition = vector.ToVector3ZisY();
            this.ActorEnemy.BoxAnimator.sprite.HeightOffGround = Mathf.Lerp(16f, 5f, t);
            this.ActorEnemy.BoxAnimator.sprite.UpdateZDepth();
            if ((double) this.m_boxThrowTimer < (double) this.m_boxThrowTime)
              return;
            if (this.CanHitOpponent())
            {
              this.ActorEnemy.DoBoxShells((Vector3) (this.m_boxEndPos + new Vector2(0.0f, 1f)));
              if (this.Actor.Opponent.state is PunchoutPlayerController.PlayerPunchState state && state.RealFrame == 0)
              {
                this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Return;
                this.BoxReturn();
              }
              else
              {
                this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.ThrowLaugh;
                this.Actor.Play("throw_laugh", this.IsLeft);
                this.Actor.Opponent.Hit(!this.IsLeft, this.Damage);
                this.Actor.Opponent.aiAnimator.PlayVfx("box_hit");
                this.ActorEnemy.BoxAnimator.gameObject.SetActive(false);
              }
            }
            else
            {
              this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.ThrowMiss;
              this.Actor.Play("throw_miss", this.IsLeft);
              this.BoxMiss();
            }
          }
          else if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.ThrowMiss)
          {
            this.m_boxThrowTimer += BraveTime.DeltaTime;
            this.ActorEnemy.BoxAnimator.transform.localPosition = (Vector3) Vector2.Lerp(this.m_boxStartPos, this.m_boxEndPos, this.m_boxThrowTimer / this.m_boxThrowTime);
            this.ActorEnemy.BoxAnimator.sprite.HeightOffGround = 5f;
            this.ActorEnemy.BoxAnimator.sprite.UpdateZDepth();
            if ((double) this.m_boxThrowTimer < (double) this.m_boxThrowTime)
              return;
            this.ActorEnemy.BoxAnimator.gameObject.SetActive(false);
          }
          else
          {
            if (this.m_state != PunchoutAIActor.ThrowAmmoState.ThrowState.Return)
              return;
            this.m_boxThrowTimer += BraveTime.DeltaTime;
            float t = this.m_boxThrowTimer / this.m_boxThrowTime;
            this.ActorEnemy.BoxAnimator.transform.localPosition = (Vector3) Vector2.Lerp(this.m_boxStartPos, this.m_boxEndPos, t);
            this.ActorEnemy.BoxAnimator.sprite.HeightOffGround = Mathf.Lerp(6f, 16f, t);
            this.ActorEnemy.BoxAnimator.sprite.UpdateZDepth();
            if ((double) this.m_boxThrowTimer < (double) this.m_boxThrowTime)
              return;
            this.Actor.Play("throw_hit", this.IsLeft);
            this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Hit;
            this.Actor.Hit(this.IsLeft, this.ReturnDamage, skipProcessing: true);
            this.Actor.aiAnimator.PlayVfx(!this.IsLeft ? "box_hit_right" : "box_hit_left");
            this.ActorEnemy.BoxAnimator.gameObject.SetActive(false);
            this.ActorEnemy.DoBoxShellsBack((Vector3) this.m_boxEndPos, this.IsLeft);
          }
        }

        public bool CanHitOpponent()
        {
          PunchoutGameActor.State state = this.Actor.Opponent.state;
          if (this.m_state != PunchoutAIActor.ThrowAmmoState.ThrowState.Throw)
            return false;
          switch (state)
          {
            case PunchoutGameActor.DuckState _:
              return false;
            case PunchoutGameActor.DodgeState dodgeState:
              if (dodgeState.IsLeft == this.IsLeft)
                return false;
              break;
          }
          return true;
        }

        public override void OnAnimationCompleted()
        {
          base.OnAnimationCompleted();
          if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Intro)
          {
            if ((double) Random.value < (double) this.SwitchChance)
            {
              this.Actor.Play("throw_switch", this.IsLeft);
              this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Switch;
            }
            else
            {
              this.Actor.Play("throw", this.IsLeft);
              this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Throw;
            }
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          }
          else if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Switch)
          {
            this.IsLeft = !this.IsLeft;
            this.Actor.Play("throw", this.IsLeft);
            this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Throw;
          }
          else if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Throw && this.Actor.aiAnimator.IsPlaying("throw"))
          {
            this.Actor.Play("throw_outro", this.IsLeft);
            this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Outro;
            this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.5f);
          }
          else if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.ThrowLaugh || this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.ThrowMiss || this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Hit)
          {
            this.Actor.Play("throw_outro", this.IsLeft);
            this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Outro;
            this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.5f);
          }
          else
          {
            if (this.m_state != PunchoutAIActor.ThrowAmmoState.ThrowState.Outro)
              return;
            this.IsDone = true;
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          }
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Switch && currentFrame == 6)
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          else if (this.Actor.aiAnimator.IsPlaying("throw_miss") && currentFrame % 3 == 2)
          {
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          }
          else
          {
            if (this.m_state != PunchoutAIActor.ThrowAmmoState.ThrowState.Throw || currentFrame != 15)
              return;
            this.BoxThrow();
          }
        }

        public override bool CanBeHit(bool isLeft)
        {
          if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Throw && (double) this.m_boxThrowTimer > (double) this.ActorEnemy.BoxCounterStartTime)
            return true;
          return this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Return && (double) this.m_boxThrowTimer < 0.33;
        }

        public override bool IsFarAway() => true;

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          if (this.m_state == PunchoutAIActor.ThrowAmmoState.ThrowState.Throw)
          {
            preventDamage = true;
            this.m_state = PunchoutAIActor.ThrowAmmoState.ThrowState.Return;
            this.BoxReturn();
          }
          else
          {
            if (this.m_state != PunchoutAIActor.ThrowAmmoState.ThrowState.Return)
              return;
            preventDamage = true;
          }
        }

        private void BoxThrow()
        {
          this.m_boxStartPos = this.ActorEnemy.BoxStart;
          if (this.IsLeft)
            this.m_boxStartPos.x *= -1f;
          this.m_boxEndPos = this.ActorEnemy.BoxEnd;
          this.m_boxThrowTime = this.ActorEnemy.BoxThrowTime;
          this.m_boxThrowTimer = 0.0f;
          this.ActorEnemy.BoxAnimator.gameObject.SetActive(true);
          this.ActorEnemy.BoxAnimator.Play(this.IsLeft ? "rat_box_left" : "rat_box_right");
          this.ActorEnemy.BoxAnimator.transform.localPosition = (Vector3) this.m_boxStartPos;
          this.ActorEnemy.BoxAnimator.sprite.HeightOffGround = 16f;
          this.ActorEnemy.BoxAnimator.sprite.UpdateZDepth();
          this.m_hasThrown = true;
        }

        private void BoxMiss()
        {
          this.ActorEnemy.BoxAnimator.Play(this.IsLeft ? "rat_box_left_fall" : "rat_box_right_fall");
          Vector2 vector2 = this.m_boxEndPos - this.m_boxStartPos;
          this.m_boxStartPos = this.m_boxEndPos;
          this.m_boxEndPos = this.m_boxStartPos + vector2;
          this.m_boxThrowTime = this.ActorEnemy.BoxAnimator.CurrentClip.BaseClipLength;
          this.m_boxThrowTimer = 0.0f;
        }

        private void BoxReturn()
        {
          this.m_boxEndPos = this.m_boxStartPos;
          this.m_boxStartPos = this.ActorEnemy.BoxAnimator.transform.localPosition.XY();
          if ((double) this.m_boxStartPos.y < 2.0)
            this.m_boxStartPos.y = 2f;
          this.m_boxThrowTime = this.ActorEnemy.BoxCounterReturnTime;
          this.m_boxThrowTimer = 0.0f;
          this.ActorEnemy.BoxAnimator.Play(this.IsLeft ? "rat_box_left_return" : "rat_box_right_return");
          this.ActorEnemy.BoxAnimator.transform.localPosition = (Vector3) this.m_boxStartPos;
          this.ActorEnemy.BoxAnimator.sprite.HeightOffGround = 6f;
          this.ActorEnemy.BoxAnimator.sprite.UpdateZDepth();
          this.ActorEnemy.BoxAnimator.ignoreTimeScale = true;
          StickyFrictionManager.Instance.RegisterCustomStickyFriction(5f / 16f, 0.0f, false);
          this.Actor.Opponent.aiAnimator.PlayVfx(!this.Actor.Opponent.state.IsLeft ? "box_punch_right" : "box_punch_left");
        }

        private enum ThrowState
        {
          None,
          Intro,
          Switch,
          Throw,
          ThrowMiss,
          ThrowLaugh,
          Outro,
          Return,
          Hit,
        }
      }

      public class PunchBasicComboState : PunchoutGameActor.BasicComboState
      {
        public PunchBasicComboState(bool firstIsLeft) : base(new PunchoutGameActor.State[4]
        {
          (PunchoutGameActor.State) new PunchoutAIActor.PunchState(firstIsLeft, false),
          (PunchoutGameActor.State) new PunchoutAIActor.PunchState(!firstIsLeft, false),
          (PunchoutGameActor.State) new PunchoutAIActor.PunchState(firstIsLeft, false),
          (PunchoutGameActor.State) new PunchoutAIActor.UppercutState(!firstIsLeft)
        })
        {
        }

        public override float PunishTime => 0.3f;
      }

      public class BrassKnucklesPunchState : PunchoutGameActor.BasicAttackState
      {
        public BrassKnucklesPunchState(bool isLeft) : base(isLeft)
        {
        }

        public override string AnimName => "brass_punch";

        public override int DamageFrame => 26;

        public override float Damage => 20f;

        public override float PunishTime => 0.3f;

        public override bool CanHitOpponent(PunchoutGameActor.State state) => true;

        public override bool CanBeHit(bool isLeft)
        {
          if (this.WasBlocked)
            return false;
          return this.Actor.CurrentFrame == 24 || this.Actor.CurrentFrame == 25;
        }

        public override bool IsFarAway()
        {
          return (this.Actor.CurrentFrame < 23 ? 0 : ((double) this.Actor.CurrentFrameFloat < 25.5 ? 1 : 0)) == 0;
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (currentFrame == 4 || currentFrame == 14 || currentFrame == 19 || currentFrame == 24)
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          if (currentFrame != 23)
            return;
          this.Actor.FlashWarn(2.5f);
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }

        public override bool ShouldInstantKO(int starsUsed) => starsUsed >= 3;
      }

      public class SuperTailWhipState : PunchoutGameActor.BasicAttackState
      {
        private int m_spins;
        private bool m_hitPlayer;

        public override string AnimName => "super_tail_whip";

        public int FlashFrame => 15;

        public override int DamageFrame => 18;

        public override float Damage => 10f;

        public override float PunishTime => 0.3f;

        public override void Update()
        {
          base.Update();
          this.WasBlocked = false;
        }

        public override bool CanHitOpponent(PunchoutGameActor.State state)
        {
          if (state is PunchoutGameActor.BlockState)
          {
            (state as PunchoutGameActor.BlockState).Bonk();
            return false;
          }
          this.m_hitPlayer = true;
          return true;
        }

        public override bool CanBeHit(bool isLeft)
        {
          return this.m_spins == 0 && this.Actor.CurrentFrame >= this.FlashFrame && this.Actor.CurrentFrame < this.DamageFrame;
        }

        public override bool IsFarAway()
        {
          return this.Actor.CurrentFrame >= 6 && this.Actor.CurrentFrame <= 14;
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (currentFrame == 5 || currentFrame == 7 || currentFrame == 9 || currentFrame == 11)
            GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
          if (this.m_spins == 0 && currentFrame == this.FlashFrame)
            this.Actor.FlashWarn((float) (this.DamageFrame - this.FlashFrame));
          if (currentFrame == this.DamageFrame)
            ++this.m_spins;
          if (currentFrame != 22 || this.m_spins < 4)
            return;
          if (this.m_hitPlayer)
            this.Actor.aiAnimator.EndAnimation();
          else
            this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }

        public override bool ShouldInstantKO(int starsUsed) => starsUsed >= 3;
      }

      public class LaughTauntState : PunchoutGameActor.State
      {
        public LaughTauntState()
          : base(false)
        {
        }

        public override string AnimName => "laugh_taunt";

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (currentFrame == 6)
            this.Actor.FlashWarn(1.5f);
          if (currentFrame % 3 != 1)
            return;
          GameManager.Instance.PrimaryPlayer.DoVibration(Vibration.Time.Quick, Vibration.Strength.Light);
        }

        public override bool ShouldInstantKO(int starsUsed) => starsUsed >= 2;

        public override void Stop()
        {
          base.Stop();
          this.ActorEnemy.TauntCooldownTimer = this.ActorEnemy.TauntCooldown;
        }
      }

      public class CheeseTauntState : PunchoutGameActor.State
      {
        public float HealAmount = 30f;
        private bool m_isCountering;
        private float m_startingHealth;
        private float? m_targetHealth;

        public CheeseTauntState()
          : base(false)
        {
        }

        public override string AnimName => "cheese_taunt";

        public override void Start()
        {
          base.Start();
          this.m_startingHealth = this.Actor.Health;
          this.m_targetHealth = new float?(Mathf.Min(100f, this.m_startingHealth + this.HealAmount));
        }

        public override bool CanBeHit(bool isLeft)
        {
          if (this.m_isCountering)
            return false;
          if (this.Actor.CurrentFrame < 9)
          {
            this.m_isCountering = true;
            this.Actor.Play("cheese_taunt_counter");
            return false;
          }
          return (double) this.Actor.CurrentFrameFloat >= 8.5 && this.Actor.CurrentFrame <= 9 && !isLeft;
        }

        public override void OnFrame(int currentFrame)
        {
          if (!this.m_isCountering)
          {
            switch (currentFrame)
            {
              case 9:
                GameManager.Instance.PrimaryPlayer.DoVibration(0.545454562f, Vibration.Strength.Light);
                break;
              case 10:
                this.ActorEnemy.DoHealSuck(new Vector3(-0.125f, 2f, -2.5f));
                ++this.ActorEnemy.SuccessfulHeals;
                break;
              case 11:
              case 15:
                this.Actor.PulseColor(this.ActorEnemy.RedPulseColor, 3f);
                break;
            }
          }
          else
          {
            switch (currentFrame)
            {
              case 8:
                this.Actor.Opponent.Hit(true, 3f);
                (this.Actor.Opponent as PunchoutPlayerController).Exhaust(new float?(1.45f));
                break;
              case 16 /*0x10*/:
                GameManager.Instance.PrimaryPlayer.DoVibration(0.727272749f, Vibration.Strength.Light);
                break;
              case 17:
                this.ActorEnemy.DoHealSuck(new Vector3(-0.125f, 2f, -2.5f));
                ++this.ActorEnemy.SuccessfulHeals;
                break;
              case 18:
              case 22:
                this.Actor.PulseColor(this.ActorEnemy.RedPulseColor, 3f);
                break;
            }
          }
        }

        public override void Update()
        {
          base.Update();
          if (!this.m_isCountering)
          {
            if (this.Actor.CurrentFrame < 10 || !this.m_targetHealth.HasValue)
              return;
            this.Actor.Health = Mathf.Lerp(this.m_startingHealth, this.m_targetHealth.Value, Mathf.Clamp01((float) (((double) this.Actor.CurrentFrameFloat - 10.0) / 7.0)));
          }
          else
          {
            if (this.Actor.CurrentFrame < 17 || !this.m_targetHealth.HasValue)
              return;
            this.Actor.Health = Mathf.Lerp(this.m_startingHealth, this.m_targetHealth.Value, Mathf.Clamp01((float) (((double) this.Actor.CurrentFrameFloat - 17.0) / 7.0)));
          }
        }

        public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
        {
          base.OnHit(ref preventDamage, isLeft, starsUsed);
          this.m_targetHealth = new float?();
          if (starsUsed != 0 || this.m_isCountering || this.Actor.CurrentFrame != 9)
            return;
          preventDamage = true;
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.CheeseHitState();
          ((PunchoutPlayerController) this.Actor.Opponent).AddStar();
        }

        public override void Stop()
        {
          base.Stop();
          this.ActorEnemy.TauntCooldownTimer = this.ActorEnemy.TauntCooldown;
          if (!this.m_targetHealth.HasValue)
            return;
          this.Actor.Health = this.m_targetHealth.Value;
        }
      }

      public class IntroState : PunchoutGameActor.State
      {
        private PunchoutAIActor.IntroState.State m_state;

        public override string AnimName => (string) null;

        public override void Start()
        {
          base.Start();
          GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Intro_01", this.Actor.gameObject);
          this.Actor.Play("intro");
          this.m_state = PunchoutAIActor.IntroState.State.MaybeIntro;
        }

        public override void Update()
        {
          base.Update();
          if (this.m_state == PunchoutAIActor.IntroState.State.MaybeIntro)
          {
            if (!PunchoutController.InTutorial)
              return;
            this.Actor.Play("intro_tutorial");
            this.m_state = PunchoutAIActor.IntroState.State.Tutorial;
          }
          else if (this.m_state == PunchoutAIActor.IntroState.State.Tutorial)
          {
            if (PunchoutController.InTutorial)
              return;
            this.Actor.Play("intro_transition");
            this.m_state = PunchoutAIActor.IntroState.State.Transition;
          }
          else
          {
            if (this.m_state != PunchoutAIActor.IntroState.State.Transition || !this.Actor.aiAnimator.IsIdle())
              return;
            this.Actor.Play("intro");
            this.m_state = PunchoutAIActor.IntroState.State.Intro;
          }
        }

        public override void Stop()
        {
          base.Stop();
          GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Theme_01", this.Actor.gameObject);
        }

        public override bool IsFarAway()
        {
          return !PunchoutController.IsActive || (this.m_state == PunchoutAIActor.IntroState.State.MaybeIntro ? 1 : (this.m_state == PunchoutAIActor.IntroState.State.Intro ? 1 : 0)) == 0;
        }

        public override bool CanBeHit(bool isLeft)
        {
          if (!PunchoutController.IsActive)
            return false;
          return this.m_state == PunchoutAIActor.IntroState.State.MaybeIntro || this.m_state == PunchoutAIActor.IntroState.State.Intro;
        }

        private enum State
        {
          MaybeIntro,
          Tutorial,
          Transition,
          Intro,
        }
      }

      public class InstantKnockdownState : PunchoutGameActor.State
      {
        public InstantKnockdownState(bool isLeft) : base(isLeft)
        {
        }

        public int DamageFrame = 11;
        public float Damage = 10f;
        private PunchoutAIActor.InstantKnockdownState.KnockdownState m_state;

        public override void Start()
        {
          this.Actor.Play("knockdown", this.IsLeft);
          this.m_state = PunchoutAIActor.InstantKnockdownState.KnockdownState.Fall;
          this.ActorEnemy.UpdateUI(this.ActorEnemy.Phase + 1);
          this.ActorEnemy.DropKey(this.IsLeft);
          base.Start();
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (this.m_state == PunchoutAIActor.InstantKnockdownState.KnockdownState.Fall && currentFrame == 10)
          {
            this.m_state = PunchoutAIActor.InstantKnockdownState.KnockdownState.Attack;
            this.Actor.Play("knockdown_cheat", !this.IsLeft);
          }
          else
          {
            if (this.m_state != PunchoutAIActor.InstantKnockdownState.KnockdownState.Attack || currentFrame != this.DamageFrame || !this.CanHitOpponent())
              return;
            this.Actor.Opponent.Hit(!this.IsLeft, this.Damage);
          }
        }

        public bool CanHitOpponent()
        {
          PunchoutGameActor.State state = this.Actor.Opponent.state;
          if (!(state is PunchoutGameActor.BlockState))
            return !(state is PunchoutGameActor.DuckState);
          (state as PunchoutGameActor.BlockState).Bonk();
          return false;
        }

        public override void OnAnimationCompleted()
        {
          base.OnAnimationCompleted();
          if (this.m_state != PunchoutAIActor.InstantKnockdownState.KnockdownState.Attack)
            return;
          this.ActorEnemy.GoToNextPhase(new bool?(), false);
        }

        public override bool CanBeHit(bool isLeft) => false;

        public override bool IsFarAway() => true;

        private enum KnockdownState
        {
          None,
          Fall,
          Attack,
        }
      }

      public class DeathState : PunchoutGameActor.State
      {
        private bool m_killedBySuper;

        public DeathState(bool isLeft, bool killedBySuper)
          : base(isLeft)
        {
          this.m_killedBySuper = killedBySuper;
        }

        public override void Start()
        {
          base.Start();
          this.Actor.aiAnimator.FacingDirection = !this.IsLeft ? 0.0f : 180f;
          this.Actor.aiAnimator.PlayUntilCancelled(!this.m_killedBySuper ? "die" : "die_super");
          this.ActorEnemy.DropKey(this.IsLeft);
          if (this.m_killedBySuper)
            this.ActorEnemy.DropKey(this.IsLeft);
          if (this.ActorEnemy.NumTimesTripleStarred >= 3)
            this.ActorEnemy.DropKey(this.IsLeft);
          this.ActorEnemy.DropReward((this.IsLeft ? 1 : 0) != 0, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S);
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (this.m_killedBySuper)
          {
            if (currentFrame == 10)
              SpriteOutlineManager.RemoveOutlineFromSprite(this.Actor.sprite);
            else if (currentFrame == 18 && !(this.Actor.Opponent.state is PunchoutPlayerController.WinState))
            {
              ((PunchoutPlayerController) this.Actor.Opponent).Win();
            }
            else
            {
              if (currentFrame != 30)
                return;
              this.Actor.transform.position += new Vector3(0.0f, -11f / 16f);
            }
          }
          else
          {
            if (currentFrame != 13 || this.Actor.Opponent.state is PunchoutPlayerController.WinState)
              return;
            ((PunchoutPlayerController) this.Actor.Opponent).Win();
          }
        }

        public override bool CanBeHit(bool isLeft) => false;

        public override bool IsFarAway() => true;
      }

      public class WinState : PunchoutGameActor.State
      {
        public override void Start()
        {
          base.Start();
          this.Actor.aiAnimator.PlayUntilCancelled("win");
        }

        public override bool CanBeHit(bool isLeft) => false;
      }

      public class EscapeState : PunchoutGameActor.State
      {
        public override void Start()
        {
          base.Start();
          this.Actor.aiAnimator.FacingDirection = -90f;
          this.Actor.aiAnimator.PlayUntilCancelled("escape");
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          if (currentFrame != 27)
            return;
          this.ActorEnemy.DoFadeOut();
        }

        public override bool CanBeHit(bool isLeft) => false;

        public override bool IsFarAway() => true;
      }

      public class PhaseTransitionState : PunchoutGameActor.State
      {
        private float m_startingHealth;

        public PhaseTransitionState(bool isLeft, float startingHealth)
          : base(isLeft)
        {
          this.m_startingHealth = startingHealth;
        }

        public override string AnimName => "transition";

        public override void Start()
        {
          base.Start();
          this.ActorEnemy.UpdateUI(this.ActorEnemy.Phase + 1);
          if (this.ActorEnemy.Phase == 0)
            GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Transition_01", this.Actor.gameObject);
          else
            GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Transition_02", this.Actor.gameObject);
        }

        public override void Update()
        {
          base.Update();
          if (this.Actor.CurrentFrame < 16 /*0x10*/)
            return;
          this.Actor.Health = Mathf.Lerp(this.m_startingHealth, 100f, Mathf.Clamp01((float) (((double) this.Actor.CurrentFrameFloat - 16.0) / 8.0)));
        }

        public override void OnFrame(int currentFrame)
        {
          base.OnFrame(currentFrame);
          switch (currentFrame)
          {
            case 16 /*0x10*/:
              this.ActorEnemy.DoHealSuck(new Vector3(!this.IsLeft ? -1f : 1f, 3.625f, -69f / 16f));
              break;
            case 17:
            case 21:
              this.Actor.PulseColor(this.ActorEnemy.RedPulseColor, 3f);
              break;
          }
        }

        public override bool CanBeHit(bool isLeft) => false;

        public override bool IsFarAway() => true;

        public override void Stop()
        {
          base.Stop();
          this.Actor.Health = 100f;
          this.ActorEnemy.Phase = (this.ActorEnemy.Phase + 1) % 3;
          this.ActorEnemy.SuccessfulHeals = 0;
          if (this.ActorEnemy.Phase == 1)
            GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Theme_02", this.Actor.gameObject);
          else
            GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Theme_03", this.Actor.gameObject);
        }
      }

      public new class HitState : PunchoutGameActor.State
      {
        private int m_maxHits;
        private int m_hits = 1;
        private bool m_isAlternating = true;

        public HitState(bool isLeft, int remainingHits)
          : base(isLeft)
        {
          this.m_maxHits = remainingHits;
        }

        public override string AnimName => "hit";

        public override void Update()
        {
          base.Update();
          if (!(this.Actor.Opponent.state is PunchoutGameActor.BasicAttackState) || this.Actor.Opponent.state == this.Actor.LastHitBy)
            return;
          if (this.m_isAlternating && (this.Actor.Opponent.state as PunchoutGameActor.BasicAttackState).IsLeft != this.IsLeft)
          {
            if (this.m_hits + 1 <= this.m_maxHits * 2)
              return;
            this.Actor.state = (PunchoutGameActor.State) new PunchoutGameActor.BlockState();
          }
          else
          {
            if (this.m_hits + 1 <= this.m_maxHits)
              return;
            this.Actor.state = (PunchoutGameActor.State) new PunchoutGameActor.BlockState();
          }
        }

        public void HitAgain(bool newIsLeft)
        {
          ++this.m_hits;
          if (newIsLeft == this.IsLeft)
            this.m_isAlternating = false;
          this.IsLeft = newIsLeft;
          this.Actor.Play("hit", this.IsLeft);
        }
      }

      public class SuperHitState : PunchoutGameActor.State
      {
        public override string AnimName => "hit_super";

        public override bool CanBeHit(bool isLeft) => false;
      }

      public class CheeseHitState : PunchoutGameActor.State
      {
        public override string AnimName => "cheese_taunt_hit";

        public override bool CanBeHit(bool isLeft) => false;
      }

      public class DazeState : PunchoutGameActor.State
      {
        public override string AnimName => "daze";

        public override void Stop()
        {
          base.Stop();
          this.Actor.aiAnimator.EndAnimationIf("daze");
        }
      }

      public class TailDazeState : PunchoutGameActor.State
      {
        public override string AnimName => "tail_whip_hit";

        public override bool CanBeHit(bool isLeft) => false;

        public override void OnAnimationCompleted()
        {
          base.OnAnimationCompleted();
          this.Actor.state = (PunchoutGameActor.State) new PunchoutAIActor.DazeState();
        }
      }
    }

}
