// Decompiled with JetBrains decompiler
// Type: SimultaneousAttackBehaviorGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName(".Groups/SimultaneousAttackBehaviorGroup")]
    public class SimultaneousAttackBehaviorGroup : AttackBehaviorBase, IAttackBehaviorGroup
    {
      [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
      public List<AttackBehaviorBase> AttackBehaviors;
      private bool[] m_finished;

      public override void Start()
      {
        base.Start();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].Start();
        this.m_finished = new bool[this.AttackBehaviors.Count];
      }

      public override void Upkeep()
      {
        base.Upkeep();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].Upkeep();
      }

      public override bool OverrideOtherBehaviors()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].OverrideOtherBehaviors())
            return true;
        }
        return false;
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult1 = base.Update();
        if (behaviorResult1 != BehaviorResult.Continue)
          return behaviorResult1;
        if (!this.IsReady())
          return BehaviorResult.Continue;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          BehaviorResult behaviorResult2 = this.AttackBehaviors[index].Update();
          if (index > 0 & behaviorResult2 != behaviorResult1)
            Debug.LogError((object) "Mismatching result returned from a SimultaneousAttackBehaviorGroup: this is not supported!");
          behaviorResult1 = behaviorResult2;
          this.m_finished[index] = false;
        }
        return behaviorResult1;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        bool flag = false;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (!this.m_finished[index])
          {
            if (this.AttackBehaviors[index].ContinuousUpdate() == ContinuousBehaviorResult.Continue)
            {
              flag = true;
            }
            else
            {
              this.m_finished[index] = true;
              this.AttackBehaviors[index].EndContinuousUpdate();
            }
          }
        }
        return flag ? ContinuousBehaviorResult.Continue : ContinuousBehaviorResult.Finished;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (!this.m_finished[index])
            this.AttackBehaviors[index].EndContinuousUpdate();
        }
      }

      public override void Destroy()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].Destroy();
        base.Destroy();
      }

      public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
      {
        base.Init(gameObject, aiActor, aiShooter);
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].Init(gameObject, aiActor, aiShooter);
      }

      public override void SetDeltaTime(float deltaTime)
      {
        base.SetDeltaTime(deltaTime);
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].SetDeltaTime(deltaTime);
      }

      public override bool IsReady()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (!this.AttackBehaviors[index].IsReady())
            return false;
        }
        return true;
      }

      public override float GetMinReadyRange()
      {
        if (!this.IsReady())
          return -1f;
        float a = float.MaxValue;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          a = Mathf.Min(a, this.AttackBehaviors[index].GetMinReadyRange());
        return a;
      }

      public override float GetMaxRange()
      {
        float a = float.MinValue;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          a = Mathf.Max(a, this.AttackBehaviors[index].GetMaxRange());
        return a;
      }

      public override bool UpdateEveryFrame()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].UpdateEveryFrame())
            return true;
        }
        return false;
      }

      public override bool IsOverridable()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (!this.AttackBehaviors[index].IsOverridable())
            return false;
        }
        return true;
      }

      public override void OnActorPreDeath()
      {
        base.OnActorPreDeath();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          this.AttackBehaviors[index].OnActorPreDeath();
      }

      public int Count => this.AttackBehaviors.Count;

      public AttackBehaviorBase GetAttackBehavior(int index) => this.AttackBehaviors[index];
    }

}
