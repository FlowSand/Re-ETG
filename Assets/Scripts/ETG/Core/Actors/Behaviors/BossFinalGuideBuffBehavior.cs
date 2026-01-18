using System;

using FullInspector;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalGuide/BuffBehavior")]
public class BossFinalGuideBuffBehavior : BuffEnemiesBehavior
    {
        public string behaviorName;
        public float behaviorProb;
        public float behaviorCooldown;
        private AttackBehaviorGroup.AttackGroupItem m_behavior;
        private float m_cachedProb;
        private float m_cachedCooldown;

        protected override void BuffEnemy(AIActor enemy)
        {
            if (this.m_behavior == null)
                this.m_behavior = this.FindBehavior(enemy);
            if (this.m_behavior != null)
            {
                this.m_cachedProb = this.m_behavior.Probability;
                this.m_cachedCooldown = (this.m_behavior.Behavior as BasicAttackBehavior).Cooldown;
                this.m_behavior.Probability = this.behaviorProb;
                (this.m_behavior.Behavior as BasicAttackBehavior).Cooldown = this.behaviorCooldown;
            }
            base.BuffEnemy(enemy);
        }

        protected override void UnbuffEnemy(AIActor enemy)
        {
            if (this.m_behavior != null)
            {
                this.m_behavior.Probability = this.m_cachedProb;
                (this.m_behavior.Behavior as BasicAttackBehavior).Cooldown = this.m_cachedCooldown;
            }
            base.UnbuffEnemy(enemy);
        }

        private AttackBehaviorGroup.AttackGroupItem FindBehavior(AIActor enemy)
        {
            return !(enemy.behaviorSpeculator.AttackBehaviors.Find((Predicate<AttackBehaviorBase>) (b => b is AttackBehaviorGroup)) is AttackBehaviorGroup attackBehaviorGroup) ? (AttackBehaviorGroup.AttackGroupItem) null : attackBehaviorGroup.AttackBehaviors.Find((Predicate<AttackBehaviorGroup.AttackGroupItem>) (i => i.NickName.Equals(this.behaviorName, StringComparison.OrdinalIgnoreCase)));
        }
    }

