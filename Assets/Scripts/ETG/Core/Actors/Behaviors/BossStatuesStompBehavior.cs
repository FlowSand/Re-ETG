// Decompiled with JetBrains decompiler
// Type: BossStatuesStompBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/StompBehavior")]
public class BossStatuesStompBehavior : BossStatuesPatternBehavior
  {
    public float HangTime = 1f;
    private int m_frameCount;

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      for (int index = 0; index < this.m_activeStatueCount; ++index)
      {
        BossStatueController activeStatue = this.m_activeStatues[index];
        if ((bool) (Object) activeStatue && activeStatue.healthHaver.IsAlive)
        {
          activeStatue.IsStomping = false;
          activeStatue.HangTime = 0.0f;
          activeStatue.State = BossStatueController.StatueState.StandStill;
        }
      }
    }

    protected override void InitPositions()
    {
      for (int index = 0; index < this.m_activeStatueCount; ++index)
      {
        BossStatueController activeStatue = this.m_activeStatues[index];
        if ((bool) (Object) activeStatue && activeStatue.healthHaver.IsAlive)
        {
          PlayerController playerClosestToPoint = GameManager.Instance.GetPlayerClosestToPoint(activeStatue.GroundPosition);
          if ((bool) (Object) playerClosestToPoint)
            activeStatue.Target = new Vector2?(playerClosestToPoint.specRigidbody.UnitCenter);
          if (this.attackType == null)
            activeStatue.QueuedBulletScript.Add((BulletScriptSelector) null);
          activeStatue.IsStomping = true;
          activeStatue.HangTime = this.HangTime;
        }
      }
      this.m_frameCount = 0;
    }

    protected override void UpdatePositions()
    {
      for (int index = 0; index < this.m_activeStatueCount; ++index)
      {
        BossStatueController activeStatue = this.m_activeStatues[index];
        if ((bool) (Object) activeStatue && activeStatue.healthHaver.IsAlive)
        {
          PlayerController playerClosestToPoint = GameManager.Instance.GetPlayerClosestToPoint(activeStatue.GroundPosition);
          if ((bool) (Object) playerClosestToPoint)
            activeStatue.Target = new Vector2?(playerClosestToPoint.specRigidbody.UnitCenter);
        }
      }
      ++this.m_frameCount;
    }

    protected override bool IsFinished()
    {
      if (this.m_frameCount < 3)
        return false;
      for (int index = 0; index < this.m_activeStatueCount; ++index)
      {
        if (!this.m_activeStatues[index].IsGrounded)
          return false;
      }
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_kali_shockwave_01", this.m_statuesController.bulletBank.gameObject);
      return true;
    }

    protected override void BeginState(BossStatuesPatternBehavior.PatternState state)
    {
      base.BeginState(state);
      if (state != BossStatuesPatternBehavior.PatternState.InProgress)
        return;
      this.SetActiveState(BossStatueController.StatueState.WaitForAttack);
    }
  }

