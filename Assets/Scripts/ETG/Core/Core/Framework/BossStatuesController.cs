using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class BossStatuesController : BraveBehaviour
    {
        public List<BossStatueController> allStatues;
        public float groundedTime = 0.15f;
        public float moveSpeed = 5f;
        public float transitionMoveSpeed = 10f;
        public float moveHopHeight = 1.5f;
        public float moveHopTime = 0.33f;
        public float attackHopHeight = 1.5f;
        public float attackHopTime = 0.33f;
        private Vector2 m_patternCenter;

        public Vector2 PatternCenter => this.m_patternCenter;

        public int NumLivingStatues { get; set; }

        public float MoveHopSpeed { get; set; }

        public float MoveGravity { get; set; }

        public float AttackHopSpeed { get; set; }

        public float AttackGravity { get; set; }

        public float? OverrideMoveSpeed { get; set; }

        public float CurrentMoveSpeed
        {
            get
            {
                if (this.IsTransitioning)
                    return this.transitionMoveSpeed;
                return this.OverrideMoveSpeed.HasValue ? this.OverrideMoveSpeed.Value : this.moveSpeed;
            }
        }

        public bool IsTransitioning { get; set; }

        public void Awake()
        {
            for (int index = 0; index < this.allStatues.Count; ++index)
                this.allStatues[index].healthHaver.OnPreDeath += new Action<Vector2>(this.OnStatueDeath);
            this.NumLivingStatues = this.allStatues.Count;
            this.bulletBank.CollidesWithEnemies = false;
            this.m_patternCenter = this.transform.position.XY() + new Vector2((float) this.dungeonPlaceable.placeableWidth / 2f, (float) this.dungeonPlaceable.placeableHeight / 2f);
            this.RecalculateHopSpeeds();
            if (!TurboModeController.IsActive)
                return;
            this.moveSpeed *= TurboModeController.sEnemyMovementSpeedMultiplier;
        }

        public void Update()
        {
        }

        protected override void OnDestroy() => base.OnDestroy();

        public void RecalculateHopSpeeds()
        {
            this.MoveHopSpeed = (float) (2.0 * ((double) this.moveHopHeight / (0.5 * (double) this.moveHopTime)));
            this.MoveGravity = (float) (-(double) this.MoveHopSpeed / (0.5 * (double) this.moveHopTime));
            this.AttackHopSpeed = (float) (2.0 * ((double) this.attackHopHeight / (0.5 * (double) this.attackHopTime)));
            this.AttackGravity = (float) (-(double) this.AttackHopSpeed / (0.5 * (double) this.attackHopTime));
        }

        public float GetEffectiveMoveSpeed(float speed)
        {
            float num = this.moveHopTime + this.groundedTime;
            return speed * (this.moveHopTime / num);
        }

        public void ClearBullets(Vector2 centerPoint)
        {
            this.StartCoroutine(this.HandleSilence(centerPoint, 30f, 30f));
        }

        [DebuggerHidden]
        private IEnumerator HandleSilence(Vector2 centerPoint, float expandSpeed, float maxRadius)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossStatuesController__HandleSilencec__Iterator0()
            {
                maxRadius = maxRadius,
                expandSpeed = expandSpeed,
                centerPoint = centerPoint
            };
        }

        private void OnStatueDeath(Vector2 finalDeathDir)
        {
            for (int index = 0; index < this.allStatues.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) this.allStatues[index] && !this.allStatues[index].healthHaver.IsDead)
                    this.allStatues[index].LevelUp();
            }
            --this.NumLivingStatues;
            if (this.NumLivingStatues != 0)
                return;
            EncounterTrackable component = this.GetComponent<EncounterTrackable>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                GameStatsManager.Instance.HandleEncounteredObject(component);
            GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_STATUES, true);
        }
    }

