// Decompiled with JetBrains decompiler
// Type: DemonWallSpewBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DemonWall/SpewBehavior")]
public class DemonWallSpewBehavior : BasicAttackBehavior
  {
    public Transform goopPoint;
    public GoopDefinition goopToUse;
    public DemonWallSpewBehavior.GoopType goopType;
    [InspectorShowIf("ShowArcParams")]
    public float goopConeLength = 5f;
    [InspectorShowIf("ShowArcParams")]
    public float goopConeArc = 45f;
    [InspectorShowIf("ShowArcParams")]
    public AnimationCurve goopCurve;
    [InspectorShowIf("ShowLineParams")]
    public float goopLength = 5f;
    [InspectorShowIf("ShowLineParams")]
    public float goopRadius = 5f;
    public float goopDuration = 0.5f;
    public bool igniteGoop;
    [InspectorShowIf("igniteGoop")]
    public float igniteDelay = 1f;
    [InspectorCategory("Attack")]
    public GameObject ShootPoint;
    [InspectorCategory("Attack")]
    public BulletScriptSelector BulletScript;
    [InspectorCategory("Visuals")]
    public string spewAnimation;
    [InspectorCategory("Visuals")]
    public GameObject spewSprite;
    private BulletScriptSource m_bulletSource;
    private float m_goopTimer;
    private float m_igniteTimer;

    private bool ShowArcParams() => this.goopType == DemonWallSpewBehavior.GoopType.Arc;

    private bool ShowLineParams() => this.goopType == DemonWallSpewBehavior.GoopType.Line;

    public override void Start()
    {
      base.Start();
      this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_goopTimer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_aiActor.ClearPath();
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_aiAnimator.PlayUntilFinished(this.spewAnimation);
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.igniteGoop && (double) this.m_igniteTimer > 0.0)
      {
        this.m_igniteTimer -= this.m_deltaTime;
        if ((double) this.m_igniteTimer <= 0.0)
          DeadlyDeadlyGoopManager.IgniteGoopsCircle((Vector2) (this.goopPoint.transform.position + new Vector3(0.0f, -0.5f)), 0.5f);
      }
      return (double) this.m_goopTimer > 0.0 || this.m_aiAnimator.IsPlaying(this.spewAnimation) || (UnityEngine.Object) this.m_bulletSource != (UnityEngine.Object) null && !this.m_bulletSource.IsEnded ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if ((bool) (UnityEngine.Object) this.m_bulletSource && !this.m_bulletSource.IsEnded)
        this.m_bulletSource.ForceStop();
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    private void AnimationEventTriggered(
      tk2dSpriteAnimator spriteAnimator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (!this.m_updateEveryFrame)
        return;
      if (clip.GetFrame(frame).eventInfo == "spew")
      {
        this.spewSprite.SetActive(true);
        this.spewSprite.GetComponent<SpriteAnimatorKiller>().Restart();
        DeadlyDeadlyGoopManager managerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopToUse);
        if (this.goopType == DemonWallSpewBehavior.GoopType.Arc)
        {
          managerForGoopType.TimedAddGoopArc((Vector2) this.goopPoint.transform.position, this.goopConeLength, this.goopConeArc, -Vector2.up, this.goopDuration, this.goopCurve);
        }
        else
        {
          Vector2 position = (Vector2) this.goopPoint.transform.position;
          managerForGoopType.TimedAddGoopLine(position, position + new Vector2(0.0f, -this.goopLength), this.goopRadius, this.goopDuration);
        }
        this.m_goopTimer = this.goopDuration;
        this.m_igniteTimer = this.igniteDelay;
      }
      if (!(clip.GetFrame(frame).eventInfo == "fire"))
        return;
      if (!(bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = this.BulletScript;
      this.m_bulletSource.Initialize();
    }

    public enum GoopType
    {
      Arc,
      Line,
    }
  }

