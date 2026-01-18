// Decompiled with JetBrains decompiler
// Type: FleeTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using Pathfinding;
using System;
using UnityEngine;

#nullable disable

public class FleeTargetBehavior : MovementBehaviorBase
  {
    public float PathInterval = 0.25f;
    public float CloseDistance = 9f;
    public float CloseTime = 3f;
    public float TooCloseDistance = 6f;
    public bool TooCloseLOS = true;
    public float DesiredDistance = 20f;
    public int PlayerPersonalSpace;
    public bool CanAttackWhileMoving;
    public bool ManuallyDefineRoom;
    [InspectorShowIf("ManuallyDefineRoom")]
    public Vector2 roomMin;
    [InspectorShowIf("ManuallyDefineRoom")]
    public Vector2 roomMax;
    [NonSerialized]
    public bool ForceRun;
    private float m_repathTimer;
    private float m_closeTimer;
    private bool m_wasDamaged;
    private bool m_shouldRun;
    private SpeculativeRigidbody m_otherTargetRigidbody;
    private IntVector2? m_targetPos;
    private IntVector2 m_cachedPlayerCell;
    private IntVector2? m_cachedOtherPlayerCell;

    public override void Start()
    {
      if (!(bool) (UnityEngine.Object) this.m_aiActor || !(bool) (UnityEngine.Object) this.m_aiActor.healthHaver)
        return;
      this.m_aiActor.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
      this.DecrementTimer(ref this.m_closeTimer);
      if ((double) this.m_aiActor.DistanceToTarget > (double) this.CloseDistance)
        this.m_closeTimer = this.CloseTime;
      this.m_shouldRun = false;
      if (this.m_wasDamaged)
      {
        this.m_shouldRun = true;
        this.m_wasDamaged = false;
      }
      this.m_otherTargetRigidbody = (SpeculativeRigidbody) null;
      if (!(this.m_aiActor.PlayerTarget is PlayerController) || GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return;
      PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this.m_aiActor.PlayerTarget as PlayerController);
      if (!(bool) (UnityEngine.Object) otherPlayer || !(bool) (UnityEngine.Object) otherPlayer.healthHaver || !otherPlayer.healthHaver.IsAlive)
        return;
      this.m_otherTargetRigidbody = otherPlayer.specRigidbody;
    }

    public override bool OverrideOtherBehaviors() => this.ShouldRun();

    public override BehaviorResult Update()
    {
      if (!this.m_targetPos.HasValue && (double) this.m_repathTimer > 0.0)
        return BehaviorResult.Continue;
      if (this.m_targetPos.HasValue && this.m_aiActor.PathComplete)
        this.m_targetPos = new IntVector2?();
      if (!this.m_targetPos.HasValue && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody && this.ShouldRun() && this.m_aiActor.ParentRoom != null)
      {
        RoomHandler parentRoom = this.m_aiActor.ParentRoom;
        this.m_targetPos = parentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: new Pathfinding.CellValidator(this.CellValidator));
        if (!this.m_targetPos.HasValue)
          this.m_targetPos = parentRoom.GetRandomWeightedAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: new Pathfinding.CellValidator(this.CellValidator), cellWeightFinder: new Func<IntVector2, float>(this.CellWeighter));
        this.m_repathTimer = 0.0f;
        this.m_closeTimer = 0.0f;
        this.ForceRun = false;
      }
      if ((double) this.m_repathTimer <= 0.0 && this.m_targetPos.HasValue && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      {
        this.m_repathTimer = this.PathInterval;
        this.m_cachedPlayerCell = this.m_aiActor.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
        this.m_cachedOtherPlayerCell = !(bool) (UnityEngine.Object) this.m_otherTargetRigidbody ? new IntVector2?() : new IntVector2?(this.m_otherTargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
        this.m_aiActor.PathfindToPosition(this.m_targetPos.Value.ToCenterVector2(), extraWeightingFunction: new ExtraWeightingFunction(this.CellPathingWeighter));
      }
      if (!this.m_targetPos.HasValue)
        return BehaviorResult.Continue;
      return this.CanAttackWhileMoving ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.SkipAllRemainingBehaviors;
    }

    private bool CellValidator(IntVector2 c)
    {
      if (this.ManuallyDefineRoom && ((double) c.x < (double) this.roomMin.x || (double) c.x > (double) this.roomMax.x || (double) c.y < (double) this.roomMin.y || (double) c.y > (double) this.roomMax.y))
        return false;
      for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
      {
        for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
        {
          if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
            return false;
        }
      }
      return (double) Vector2.Distance(Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance), this.m_aiActor.TargetRigidbody.UnitCenter) >= (double) this.DesiredDistance && (!(bool) (UnityEngine.Object) this.m_otherTargetRigidbody || (double) Vector2.Distance(Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance), this.m_otherTargetRigidbody.UnitCenter) >= (double) this.DesiredDistance);
    }

    private float CellWeighter(IntVector2 c)
    {
      for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
      {
        for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
        {
          if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
            return 1000000f;
        }
      }
      float a = Vector2.Distance(Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance), this.m_aiActor.TargetRigidbody.UnitCenter);
      if ((bool) (UnityEngine.Object) this.m_otherTargetRigidbody)
        a = Mathf.Min(a, Vector2.Distance(Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance), this.m_otherTargetRigidbody.UnitCenter));
      return a;
    }

    private int CellPathingWeighter(IntVector2 prevStep, IntVector2 thisStep)
    {
      return (double) IntVector2.Distance(thisStep, this.m_cachedPlayerCell) < (double) this.PlayerPersonalSpace || this.m_cachedOtherPlayerCell.HasValue && (double) IntVector2.Distance(thisStep, this.m_cachedOtherPlayerCell.Value) < (double) this.PlayerPersonalSpace ? 100 : 0;
    }

    private void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      this.m_wasDamaged = true;
    }

    private bool ShouldRun()
    {
      if (this.m_shouldRun || this.ForceRun)
        return true;
      float a = this.m_aiActor.DistanceToTarget;
      if (this.m_aiActor.PlayerTarget is PlayerController && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this.m_aiActor.PlayerTarget as PlayerController);
        if ((bool) (UnityEngine.Object) otherPlayer && (bool) (UnityEngine.Object) otherPlayer.healthHaver && otherPlayer.healthHaver.IsAlive)
        {
          float b = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, otherPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox));
          a = Mathf.Min(a, b);
        }
      }
      return (double) a < (double) this.TooCloseDistance && (!this.TooCloseLOS || this.m_aiActor.HasLineOfSightToTarget) || (double) a < (double) this.CloseDistance && (double) this.m_closeTimer <= 0.0;
    }
  }

