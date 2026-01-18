// Decompiled with JetBrains decompiler
// Type: SpawnReinforcementsBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SpawnReinforcementsBehavior : BasicAttackBehavior
  {
    public int MaxRoomOccupancy = -1;
    public int OverrideMaxOccupancyToSpawn = -1;
    public List<int> ReinforcementIndices;
    public SpawnReinforcementsBehavior.IndexType indexType;
    public bool StaggerSpawns;
    [InspectorIndent]
    [InspectorShowIf("StaggerSpawns")]
    public SpawnReinforcementsBehavior.StaggerMode staggerMode;
    [InspectorIndent]
    [InspectorShowIf("ShowStaggerDelay")]
    public float staggerDelay = 1f;
    public bool StopDuringAnimation = true;
    public bool DisableDrops = true;
    public float DelayAfterSpawn;
    public int DelayAfterSpawnMinOccupancy;
    [InspectorCategory("Visuals")]
    public string DirectionalAnimation;
    [InspectorCategory("Visuals")]
    public bool HideGun;
    [InspectorCategory("Conditions")]
    public float StaticCooldown;
    private int m_timesReinforced;
    private int m_reinforceIndex;
    private int m_reinforceSubIndex;
    private int m_thingsToSpawn;
    private float m_staggerTimer;
    private float m_timer;
    private static float s_staticCooldown;
    private static int s_lastStaticUpdateFrameNum = -1;
    private SpawnReinforcementsBehavior.State m_state;

    private bool ShowStaggerDelay()
    {
      return this.StaggerSpawns && this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Timer;
    }

    public override void Start() => base.Start();

    public override void Upkeep()
    {
      base.Upkeep();
      if ((double) SpawnReinforcementsBehavior.s_staticCooldown > 0.0 && SpawnReinforcementsBehavior.s_lastStaticUpdateFrameNum != UnityEngine.Time.frameCount)
      {
        SpawnReinforcementsBehavior.s_staticCooldown = Mathf.Max(0.0f, SpawnReinforcementsBehavior.s_staticCooldown - this.m_deltaTime);
        SpawnReinforcementsBehavior.s_lastStaticUpdateFrameNum = UnityEngine.Time.frameCount;
      }
      this.DecrementTimer(ref this.m_staggerTimer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_reinforceIndex = this.indexType != SpawnReinforcementsBehavior.IndexType.Ordered ? BraveUtility.RandomElement<int>(this.ReinforcementIndices) : this.ReinforcementIndices[this.m_timesReinforced];
      this.m_thingsToSpawn = this.m_aiActor.ParentRoom.GetEnemiesInReinforcementLayer(this.m_reinforceIndex);
      int num1 = this.MaxRoomOccupancy;
      if (this.OverrideMaxOccupancyToSpawn > 0)
        num1 = this.OverrideMaxOccupancyToSpawn;
      if (num1 >= 0)
      {
        int count = this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count;
        if (count >= num1)
        {
          ++this.m_timesReinforced;
          this.UpdateCooldowns();
          return BehaviorResult.Continue;
        }
        this.m_thingsToSpawn = this.MaxRoomOccupancy - count;
      }
      ++this.m_timesReinforced;
      SpawnReinforcementsBehavior.s_staticCooldown += this.StaticCooldown;
      if (!string.IsNullOrEmpty(this.DirectionalAnimation))
        this.m_aiAnimator.PlayUntilFinished(this.DirectionalAnimation, true);
      if (this.HideGun)
        this.m_aiShooter.ToggleGunAndHandRenderers(false, "SpawnReinforcementBehavior");
      if (this.StopDuringAnimation)
        this.m_aiActor.ClearPath();
      if (this.StaggerSpawns)
      {
        this.m_reinforceSubIndex = 0;
        if (this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Animation)
          this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        else if (this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Timer)
          this.m_staggerTimer = this.staggerDelay;
      }
      else if (this.m_thingsToSpawn > 0)
      {
        RoomHandler parentRoom = this.m_aiActor.ParentRoom;
        int reinforceIndex = this.m_reinforceIndex;
        bool flag = false;
        bool disableDrops = this.DisableDrops;
        int thingsToSpawn = this.m_thingsToSpawn;
        int index = reinforceIndex;
        int num2 = flag ? 1 : 0;
        int num3 = disableDrops ? 1 : 0;
        int specifyObjectCount = thingsToSpawn;
        parentRoom.TriggerReinforcementLayer(index, num2 != 0, num3 != 0, specifyObjectCount: specifyObjectCount);
      }
      if (this.StopDuringAnimation || this.StaggerSpawns)
      {
        this.m_updateEveryFrame = true;
        this.m_state = SpawnReinforcementsBehavior.State.Spawning;
        return BehaviorResult.RunContinuous;
      }
      this.UpdateCooldowns();
      return BehaviorResult.SkipRemainingClassBehaviors;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_state == SpawnReinforcementsBehavior.State.Spawning)
      {
        bool flag = false;
        if (!this.StaggerSpawns)
        {
          if (!this.m_aiAnimator.IsPlaying(this.DirectionalAnimation))
            flag = true;
        }
        else if (this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Timer)
        {
          if ((double) this.m_staggerTimer <= 0.0)
          {
            this.SpawnOneDude();
            this.m_staggerTimer = this.staggerDelay;
            if (this.m_thingsToSpawn <= 0)
              flag = true;
          }
        }
        else if (this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Animation && !this.m_aiAnimator.IsPlaying(this.DirectionalAnimation))
        {
          if (this.m_thingsToSpawn > 0)
            this.m_aiActor.ParentRoom.TriggerReinforcementLayer(this.m_reinforceIndex, false, this.DisableDrops, this.m_reinforceSubIndex, this.m_thingsToSpawn);
          flag = true;
        }
        if (flag)
        {
          if ((double) this.DelayAfterSpawn <= 0.0)
            return ContinuousBehaviorResult.Finished;
          this.m_timer = this.DelayAfterSpawn;
          this.m_state = SpawnReinforcementsBehavior.State.PostSpawnDelay;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == SpawnReinforcementsBehavior.State.PostSpawnDelay)
      {
        this.DecrementTimer(ref this.m_timer);
        if (this.DelayAfterSpawnMinOccupancy > 0 && this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count < this.DelayAfterSpawnMinOccupancy || (double) this.m_timer <= 0.0)
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.HideGun)
        this.m_aiShooter.ToggleGunAndHandRenderers(true, "SpawnReinforcementBehavior");
      if (this.StaggerSpawns && this.staggerMode == SpawnReinforcementsBehavior.StaggerMode.Animation)
        this.m_aiAnimator.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsReady()
    {
      return base.IsReady() && (double) SpawnReinforcementsBehavior.s_staticCooldown <= 0.0;
    }

    private void AnimationEventTriggered(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      if (!(clip.GetFrame(frameNum).eventInfo == "spawn") || this.m_thingsToSpawn <= 0)
        return;
      this.SpawnOneDude();
    }

    private void SpawnOneDude()
    {
      this.m_aiActor.ParentRoom.TriggerReinforcementLayer(this.m_reinforceIndex, false, this.DisableDrops, this.m_reinforceSubIndex, 1);
      ++this.m_reinforceSubIndex;
      --this.m_thingsToSpawn;
    }

    public enum IndexType
    {
      Random,
      Ordered,
    }

    public enum StaggerMode
    {
      Animation,
      Timer,
    }

    private enum State
    {
      Idle,
      Spawning,
      PostSpawnDelay,
    }
  }

