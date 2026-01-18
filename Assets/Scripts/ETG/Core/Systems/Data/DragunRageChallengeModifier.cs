using System;
using System.Collections.Generic;

using Brave.BulletScript;
using Dungeonator;

#nullable disable

public class DragunRageChallengeModifier : ChallengeModifier
    {
        private AIActor m_boss;
        public float GlockRicochetTimescaleIncrease = 1.7f;
        public float NormalGlockTimescaleIncrease = 1.5f;

        private void Start()
        {
            List<AIActor> activeEnemies = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
                    this.m_boss = activeEnemies[index];
            }
            if (!(bool) (UnityEngine.Object) this.m_boss)
                return;
            this.m_boss.bulletBank.OnBulletSpawned += new Action<Bullet, Projectile>(this.HandleProjectiles);
        }

        private void HandleProjectiles(Bullet source, Projectile p)
        {
            switch (source.BankName)
            {
                case "Breath":
                    BounceProjModifier orAddComponent = p.gameObject.GetOrAddComponent<BounceProjModifier>();
                    orAddComponent.numberOfBounces = 1;
                    orAddComponent.onlyBounceOffTiles = true;
                    orAddComponent.removeBulletScriptControl = true;
                    break;
            }
        }

        private void Update()
        {
            if (!(bool) (UnityEngine.Object) this.m_boss)
                return;
            this.m_boss.LocalTimeScale = 1.25f;
            if (!(this.m_boss.behaviorSpeculator.ActiveContinuousAttackBehavior is AttackBehaviorGroup))
                return;
            BehaviorBase behaviorBase = this.m_boss.behaviorSpeculator.ActiveContinuousAttackBehavior;
            while (true)
            {
                switch (behaviorBase)
                {
                    case AttackBehaviorGroup _:
                        behaviorBase = (BehaviorBase) (behaviorBase as AttackBehaviorGroup).CurrentBehavior;
                        continue;
                    case SimultaneousAttackBehaviorGroup _:
                        goto label_5;
                    default:
                        goto label_11;
                }
            }
    label_11:
            return;
    label_5:
            if ((behaviorBase as SimultaneousAttackBehaviorGroup).AttackBehaviors.Count <= 0 || !((behaviorBase as SimultaneousAttackBehaviorGroup).AttackBehaviors[0] is DraGunGlockBehavior))
                return;
            DraGunGlockBehavior attackBehavior = (DraGunGlockBehavior) (behaviorBase as SimultaneousAttackBehaviorGroup).AttackBehaviors[0];
            if (attackBehavior.attacks.Length >= 1 && attackBehavior.attacks[0].bulletScript.scriptTypeName.Contains("GlockRicochet"))
                this.m_boss.LocalTimeScale = this.GlockRicochetTimescaleIncrease;
            else
                this.m_boss.LocalTimeScale = this.NormalGlockTimescaleIncrease;
        }
    }

