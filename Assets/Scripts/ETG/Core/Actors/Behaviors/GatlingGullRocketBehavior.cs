using System;
using System.Collections.Generic;

using FullInspector;
using UnityEngine;

using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/RocketBehavior")]
public class GatlingGullRocketBehavior : BasicAttackBehavior
    {
        public Transform RocketOrigin;
        public GameObject Rocket;
        public int MaxRockets = 5;
        public float DamageToHalt = 20f;
        public float PerRocketCooldown = 1f;
        private bool m_passthrough;
        private GatlingGullLeapBehavior m_leapBehavior;
        private float m_fireTimer;
        private float m_healthToHalt;
        private bool m_firedThisCycle;
        private int m_rocketCount;
        private RoomHandler m_room;
        private List<IntVector2> m_leapPositions = new List<IntVector2>();

        public override void Start()
        {
            base.Start();
            this.m_leapBehavior = (GatlingGullLeapBehavior) ((AttackBehaviorGroup) this.m_aiActor.behaviorSpeculator.AttackBehaviors.Find((Predicate<AttackBehaviorBase>) (b => b is AttackBehaviorGroup))).AttackBehaviors.Find((Predicate<AttackBehaviorGroup.AttackGroupItem>) (b => b.Behavior is GatlingGullLeapBehavior)).Behavior;
            this.m_room = GameManager.Instance.Dungeon.GetRoomFromPosition(this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
            List<GatlingGullLeapPoint> componentsInRoom = this.m_room.GetComponentsInRoom<GatlingGullLeapPoint>();
            for (int index = 0; index < componentsInRoom.Count; ++index)
            {
                if (componentsInRoom[index].ForRockets)
                    this.m_leapPositions.Add(componentsInRoom[index].PlacedPosition - this.m_room.area.basePosition);
            }
        }

        public override void Upkeep()
        {
            base.Upkeep();
            if (!this.m_passthrough)
                return;
            this.m_leapBehavior.Upkeep();
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            if (this.m_leapPositions.Count == 0)
                return BehaviorResult.Continue;
            this.m_leapBehavior.OverridePosition = new Vector2?((this.m_room.area.basePosition + this.m_leapPositions[UnityEngine.Random.Range(0, this.m_leapPositions.Count)]).ToVector2() + new Vector2(1f, 0.5f));
            BehaviorResult behaviorResult = this.m_leapBehavior.Update();
            if (behaviorResult == BehaviorResult.RunContinuous)
                this.m_passthrough = true;
            else
                this.m_leapBehavior.OverridePosition = new Vector2?();
            return behaviorResult;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.m_passthrough)
            {
                if (this.m_leapBehavior.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
                {
                    this.m_leapBehavior.EndContinuousUpdate();
                    this.m_aiAnimator.SuppressHitStates = true;
                    this.m_aiAnimator.PlayUntilFinished("rocket", true);
                    this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
                    this.m_rocketCount = 0;
                    this.m_fireTimer = this.PerRocketCooldown;
                    this.m_firedThisCycle = false;
                    this.m_healthToHalt = this.m_aiActor.healthHaver.GetCurrentHealth() - this.DamageToHalt;
                    this.m_aiActor.ClearPath();
                    this.m_passthrough = false;
                    this.m_leapBehavior.OverridePosition = new Vector2?();
                }
                return ContinuousBehaviorResult.Continue;
            }
            this.m_fireTimer -= this.m_deltaTime;
            if ((double) this.m_fireTimer <= 0.0)
            {
                if (!this.m_firedThisCycle)
                    this.FireRocket();
                this.m_firedThisCycle = false;
                this.m_fireTimer += this.PerRocketCooldown;
                this.m_aiAnimator.PlayUntilFinished("rocket", true);
            }
            return (double) this.m_aiActor.healthHaver.GetCurrentHealth() <= (double) this.m_healthToHalt || this.m_rocketCount >= this.MaxRockets ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_aiAnimator.SuppressHitStates = false;
            this.m_aiAnimator.EndAnimationIf("rocket");
            this.m_aiActor.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
            this.UpdateCooldowns();
        }

        public override void SetDeltaTime(float deltaTime)
        {
            base.SetDeltaTime(deltaTime);
            if (!this.m_passthrough)
                return;
            this.m_leapBehavior.SetDeltaTime(deltaTime);
        }

        public override bool IsReady()
        {
            return this.m_passthrough ? this.m_leapBehavior.IsReady() : base.IsReady();
        }

        public override bool UpdateEveryFrame()
        {
            return this.m_passthrough ? this.m_leapBehavior.UpdateEveryFrame() : base.UpdateEveryFrame();
        }

        public override bool IsOverridable()
        {
            return !this.m_passthrough || this.m_leapBehavior.IsOverridable();
        }

        private void HandleAnimationEvent(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frameNo)
        {
            if (!(clip.GetFrame(frameNo).eventInfo == "fire_rocket"))
                return;
            this.FireRocket();
        }

        private void FireRocket()
        {
            SkyRocket component = SpawnManager.SpawnProjectile(this.Rocket, this.RocketOrigin.position, Quaternion.identity).GetComponent<SkyRocket>();
            component.Target = this.m_aiActor.TargetRigidbody;
            tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
            component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
            component.ExplosionData.ignoreList.Add(this.m_aiActor.specRigidbody);
            this.m_aiActor.sprite.AttachRenderer((tk2dBaseSprite) component.GetComponentInChildren<tk2dSprite>());
            this.m_firedThisCycle = true;
            ++this.m_rocketCount;
        }
    }

