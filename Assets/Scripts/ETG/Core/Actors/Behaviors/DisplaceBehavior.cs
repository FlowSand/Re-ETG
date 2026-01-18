// Decompiled with JetBrains decompiler
// Type: DisplaceBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

public class DisplaceBehavior : BasicAttackBehavior
  {
    public float ImageHealthMultiplier = 1f;
    public float InitialImageAttackDelay = 0.5f;
    [InspectorCategory("Visuals")]
    public string Anim;
    private DisplaceBehavior.State m_state;
    private Shader m_cachedShader;
    private float m_timer;
    private bool m_hasInstantSpawned;
    private BulletLimbController[] m_limbControllers;
    private AIActor m_image;

    public override void Start()
    {
      base.Start();
      this.m_aiAnimator.ChildAnimator.renderer.enabled = false;
      this.m_limbControllers = this.m_aiActor.GetComponentsInChildren<BulletLimbController>();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.m_aiAnimator.ChildAnimator.renderer.enabled = false;
      this.DecrementTimer(ref this.m_timer);
      if (!(bool) (Object) this.m_image || !this.m_image.healthHaver.IsDead)
        return;
      this.m_image = (AIActor) null;
      this.UpdateCooldowns();
    }

    public override BehaviorResult Update()
    {
      if (!(bool) (Object) this.m_aiActor.GetComponent<DisplacedImageController>() && !this.m_hasInstantSpawned)
      {
        this.SpawnImage();
        this.m_hasInstantSpawned = true;
      }
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_aiAnimator.PlayUntilFinished(this.Anim, true);
      this.m_aiActor.ClearPath();
      if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
        this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (DisplaceBehavior));
      for (int index = 0; index < this.m_limbControllers.Length; ++index)
      {
        this.m_limbControllers[index].enabled = true;
        this.m_limbControllers[index].HideBullets = false;
      }
      this.m_state = DisplaceBehavior.State.Summoning;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_state != DisplaceBehavior.State.Summoning || this.m_aiAnimator.IsPlaying(this.Anim))
        return ContinuousBehaviorResult.Continue;
      this.SpawnImage();
      return ContinuousBehaviorResult.Finished;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (!string.IsNullOrEmpty(this.Anim))
        this.m_aiAnimator.EndAnimationIf(this.Anim);
      if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
        this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (DisplaceBehavior));
      for (int index = 0; index < this.m_limbControllers.Length; ++index)
      {
        this.m_limbControllers[index].enabled = false;
        this.m_limbControllers[index].HideBullets = true;
      }
      this.m_state = DisplaceBehavior.State.Idle;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsReady()
    {
      return (!(bool) (Object) this.m_image || !this.m_image.healthHaver.IsAlive) && base.IsReady();
    }

    public override bool IsOverridable() => false;

    private void SpawnImage()
    {
      if ((bool) (Object) this.m_behaviorSpeculator && this.m_behaviorSpeculator.MovementBehaviors.Count == 0)
        return;
      this.m_image = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.m_aiActor.EnemyGuid), this.m_aiActor.specRigidbody.UnitBottomLeft, this.m_aiActor.ParentRoom, awakenAnimType: AIActor.AwakenAnimationType.Spawn);
      this.m_image.transform.position = this.m_aiActor.transform.position;
      this.m_image.specRigidbody.Reinitialize();
      this.m_image.aiAnimator.healthHaver.SetHealthMaximum(this.ImageHealthMultiplier * this.m_aiActor.healthHaver.GetMaxHealth());
      DisplacedImageController displacedImageController = this.m_image.gameObject.AddComponent<DisplacedImageController>();
      displacedImageController.Init();
      displacedImageController.SetHost(this.m_aiActor);
      if ((bool) (Object) this.m_behaviorSpeculator && this.m_behaviorSpeculator.MovementBehaviors != null && this.m_behaviorSpeculator.MovementBehaviors.Count > 0 && this.m_behaviorSpeculator.MovementBehaviors[0] is FleeTargetBehavior movementBehavior)
        movementBehavior.ForceRun = true;
      if (this.m_hasInstantSpawned)
        return;
      this.m_image.behaviorSpeculator.GlobalCooldown = this.InitialImageAttackDelay;
    }

    private enum State
    {
      Idle,
      Summoning,
    }
  }

