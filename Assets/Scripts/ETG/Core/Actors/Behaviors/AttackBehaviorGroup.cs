// Decompiled with JetBrains decompiler
// Type: AttackBehaviorGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName(".Groups/AttackBehaviorGroup")]
    public class AttackBehaviorGroup : AttackBehaviorBase, IAttackBehaviorGroup
    {
      public bool ShareCooldowns;
      [InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
      public List<AttackBehaviorGroup.AttackGroupItem> AttackBehaviors;
      private AttackBehaviorBase m_currentBehavior;

      public AttackBehaviorBase CurrentBehavior => this.m_currentBehavior;

      public override void Start()
      {
        base.Start();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.Start();
        }
      }

      public override void Upkeep()
      {
        base.Upkeep();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.Upkeep();
        }
      }

      public override bool OverrideOtherBehaviors()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null && this.AttackBehaviors[index].Behavior.OverrideOtherBehaviors())
            return true;
        }
        return false;
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady())
          return BehaviorResult.Continue;
        float max = 0.0f;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if ((double) this.AttackBehaviors[index].Probability > 0.0 && this.AttackBehaviors[index].Behavior.IsReady())
            max += this.AttackBehaviors[index].Probability;
        }
        if ((double) max == 0.0)
          return BehaviorResult.Continue;
        float num = Random.Range(0.0f, max);
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if ((double) this.AttackBehaviors[index].Probability > 0.0 && this.AttackBehaviors[index].Behavior.IsReady())
          {
            this.m_currentBehavior = this.AttackBehaviors[index].Behavior;
            if ((double) num >= (double) this.AttackBehaviors[index].Probability)
              num -= this.AttackBehaviors[index].Probability;
            else
              break;
          }
        }
        return this.m_currentBehavior.Update();
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        return this.m_currentBehavior.ContinuousUpdate();
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        if (this.m_currentBehavior == null)
          return;
        this.m_currentBehavior.EndContinuousUpdate();
        this.m_currentBehavior = (AttackBehaviorBase) null;
      }

      public override void Destroy()
      {
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.Destroy();
        }
        base.Destroy();
      }

      public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
      {
        base.Init(gameObject, aiActor, aiShooter);
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.Init(gameObject, aiActor, aiShooter);
        }
      }

      public override void SetDeltaTime(float deltaTime)
      {
        base.SetDeltaTime(deltaTime);
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.SetDeltaTime(deltaTime);
        }
      }

      public override bool IsReady()
      {
        if (this.ShareCooldowns)
        {
          for (int index = 0; index < this.AttackBehaviors.Count; ++index)
          {
            if (this.AttackBehaviors[index].Behavior != null && !this.AttackBehaviors[index].Behavior.IsReady())
              return false;
          }
          return true;
        }
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null && this.AttackBehaviors[index].Behavior.IsReady())
            return true;
        }
        return false;
      }

      public override float GetMinReadyRange()
      {
        if (!this.IsReady())
          return -1f;
        float a = float.MaxValue;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            a = Mathf.Min(a, this.AttackBehaviors[index].Behavior.GetMinReadyRange());
        }
        return a;
      }

      public override float GetMaxRange()
      {
        float a = float.MinValue;
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            a = Mathf.Max(a, this.AttackBehaviors[index].Behavior.GetMaxRange());
        }
        return a;
      }

      public override bool UpdateEveryFrame()
      {
        return this.m_currentBehavior != null && this.m_currentBehavior.UpdateEveryFrame();
      }

      public override bool IsOverridable()
      {
        return this.m_currentBehavior != null ? this.m_currentBehavior.IsOverridable() : base.IsOverridable();
      }

      public override void OnActorPreDeath()
      {
        base.OnActorPreDeath();
        for (int index = 0; index < this.AttackBehaviors.Count; ++index)
        {
          if (this.AttackBehaviors[index].Behavior != null)
            this.AttackBehaviors[index].Behavior.OnActorPreDeath();
        }
      }

      public int Count => this.AttackBehaviors.Count;

      public AttackBehaviorBase GetAttackBehavior(int index) => this.AttackBehaviors[index].Behavior;

      public class AttackGroupItem
      {
        [InspectorName("Nickname")]
        public string NickName;
        public float Probability = 1f;
        public AttackBehaviorBase Behavior;
      }
    }

}
