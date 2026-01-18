using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class AppliesGoopDoerModifier : MonoBehaviour
    {
        public GoopDefinition goopDefinitionToUse;
        public float goopRadius = 3f;
        public bool IsSynergyContingent;
        public CustomSynergyType SynergyToCheck;
        protected Projectile m_projectile;
        protected HashSet<AIActor> m_processedActors = new HashSet<AIActor>();

        private void Start()
        {
            this.m_projectile = this.GetComponent<Projectile>();
            this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
        }

        private void HandleHitEnemy(Projectile p1, SpeculativeRigidbody srb1, bool killedEnemy)
        {
            if (this.IsSynergyContingent && (!(bool) (UnityEngine.Object) p1 || !(bool) (UnityEngine.Object) p1.PossibleSourceGun || !(p1.PossibleSourceGun.CurrentOwner is PlayerController) || !(p1.PossibleSourceGun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck)) || !(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) srb1)
                return;
            AIActor component = srb1.GetComponent<AIActor>();
            if (!(bool) (UnityEngine.Object) component || this.m_processedActors.Contains(component))
                return;
            this.m_processedActors.Add(component);
            if (killedEnemy)
            {
                Vector2 unitCenter = srb1.UnitCenter;
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinitionToUse).TimedAddGoopCircle(unitCenter, this.goopRadius, 1f);
            }
            else
            {
                GoopDoer goopDoer = srb1.gameObject.AddComponent<GoopDoer>();
                goopDoer.updateTiming = GoopDoer.UpdateTiming.TriggerOnly;
                goopDoer.updateOnPreDeath = true;
                goopDoer.goopDefinition = this.goopDefinitionToUse;
                goopDoer.defaultGoopRadius = this.goopRadius;
                goopDoer.isTimed = true;
            }
        }
    }

