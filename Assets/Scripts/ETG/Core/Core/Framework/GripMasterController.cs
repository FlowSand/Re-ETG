using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class GripMasterController : BraveBehaviour, IPlaceConfigurable, IHasDwarfConfigurables
    {
        [DwarfConfigurable]
        public bool Grip_StartAsEnemy;
        [DwarfConfigurable]
        public bool Grip_EndOnEnemiesCleared = true;
        [DwarfConfigurable]
        public int Grip_EndAfterNumAttacks = -1;
        [DwarfConfigurable]
        public int Grip_OverrideRoomsToSendBackward = -1;
        [Header("Become Enemy Stuff")]
        public DungeonPrerequisite BecomeEnemeyPrereq;
        public float BecomeEnemyChance = 0.5f;
        public int MinRoomWidth = 20;
        public int MinRoomHeight = 15;
        private bool m_isAttacking;
        private bool m_shouldBecomeEnemy;
        private int m_numTimesAttacked;
        private bool m_isEnemy;
        private DungeonData.Direction m_facingDirection;
        private float m_turnTimer;
        private List<AIActor> m_activeEnemies = new List<AIActor>();

        public bool IsAttacking
        {
            set
            {
                this.m_isAttacking = value;
                this.aiActor.IgnoreForRoomClear = this.Grip_EndOnEnemiesCleared && !this.m_shouldBecomeEnemy && !this.m_isAttacking;
            }
        }

        public void Start()
        {
            this.specRigidbody.CollideWithOthers = false;
            this.aiActor.IsGone = true;
            if (this.Grip_StartAsEnemy)
            {
                this.m_shouldBecomeEnemy = true;
                this.End(true);
            }
            else
            {
                if (this.ShouldBecomeEnemy())
                    this.m_shouldBecomeEnemy = true;
                this.aiActor.IgnoreForRoomClear = this.Grip_EndOnEnemiesCleared && !this.m_shouldBecomeEnemy;
            }
            if (this.Grip_EndAfterNumAttacks < 0 && !this.Grip_EndOnEnemiesCleared)
            {
                Debug.LogErrorFormat("Gripmaster was told to last forever! ({0})", (object) this.aiActor.ParentRoom.GetRoomName());
                this.Grip_EndOnEnemiesCleared = true;
            }
            if (this.m_isEnemy)
                return;
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
        }

        public void Update()
        {
            if (!this.m_isEnemy)
            {
                if (!this.healthHaver.IsAlive || !this.aiAnimator.IsIdle())
                    return;
                if (this.Grip_EndAfterNumAttacks > 0 && this.m_numTimesAttacked >= this.Grip_EndAfterNumAttacks)
                    this.End();
                if (!this.Grip_EndOnEnemiesCleared)
                    return;
                if (this.m_shouldBecomeEnemy)
                {
                    this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.m_activeEnemies);
                    bool flag = false;
                    for (int index = 0; index < this.m_activeEnemies.Count; ++index)
                    {
                        if ((bool) (UnityEngine.Object) this.m_activeEnemies[index] && !this.m_activeEnemies[index].healthHaver.PreventAllDamage)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        return;
                    this.End();
                }
                else
                {
                    if (this.aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) > 0)
                        return;
                    this.End();
                }
            }
            else
            {
                if (this.healthHaver.IsAlive && this.aiAnimator.IsIdle() && (double) this.m_turnTimer <= 0.0 && (bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
                {
                    DungeonData.Direction dir = DungeonData.GetCardinalFromVector2(this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.specRigidbody.GetUnitCenter(ColliderType.HitBox));
                    if (dir == DungeonData.Direction.NORTH)
                        dir = DungeonData.Direction.SOUTH;
                    if (this.m_facingDirection != dir)
                    {
                        this.aiAnimator.PlayUntilFinished(this.m_facingDirection != DungeonData.Direction.WEST ? (this.m_facingDirection != DungeonData.Direction.SOUTH ? (dir != DungeonData.Direction.SOUTH ? "red_trans_east_west" : "red_trans_east_south") : (dir != DungeonData.Direction.WEST ? "red_trans_south_east" : "red_trans_south_west")) : (dir != DungeonData.Direction.EAST ? "red_trans_west_south" : "red_trans_west_east"));
                        this.aiAnimator.AnimatedFacingDirection = DungeonData.GetAngleFromDirection(dir);
                        this.m_facingDirection = dir;
                        this.m_turnTimer = 1f;
                        this.behaviorSpeculator.AttackCooldown = Mathf.Max(this.behaviorSpeculator.AttackCooldown, this.aiAnimator.CurrentClipLength);
                    }
                }
                this.m_turnTimer = Mathf.Max(0.0f, this.m_turnTimer - this.aiActor.LocalDeltaTime);
            }
        }

        protected override void OnDestroy() => base.OnDestroy();

        public void OnAttack() => ++this.m_numTimesAttacked;

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.aiActor.IgnoreForRoomClear = this.Grip_EndOnEnemiesCleared;
        }

        public void End(bool skipAnim = false)
        {
            if (this.m_shouldBecomeEnemy)
            {
                this.healthHaver.PreventAllDamage = false;
                this.specRigidbody.CollideWithOthers = true;
                this.specRigidbody.PixelColliders[0].ManualOffsetY = 28;
                this.specRigidbody.ForceRegenerate();
                this.aiActor.IsGone = false;
                this.aiAnimator.IdleAnimation.Type = DirectionalAnimation.DirectionType.FourWayCardinal;
                this.aiAnimator.IdleAnimation.AnimNames = new string[4]
                {
                    "red_idle_south",
                    "red_idle_east",
                    "red_idle_south",
                    "red_idle_west"
                };
                this.aiAnimator.IdleAnimation.Flipped = new DirectionalAnimation.FlipType[4]
                {
                    DirectionalAnimation.FlipType.None,
                    DirectionalAnimation.FlipType.None,
                    DirectionalAnimation.FlipType.None,
                    DirectionalAnimation.FlipType.None
                };
                this.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "death")).anim.Prefix = "red_die";
                this.aiAnimator.UseAnimatedFacingDirection = true;
                this.aiAnimator.FacingDirection = -90f;
                this.m_facingDirection = DungeonData.Direction.SOUTH;
                if (!skipAnim)
                {
                    this.aiAnimator.PlayUntilFinished("transform");
                    this.behaviorSpeculator.GlobalCooldown = Mathf.Max(this.behaviorSpeculator.AttackCooldown, this.aiAnimator.CurrentClipLength);
                    this.m_turnTimer = this.aiAnimator.CurrentClipLength;
                    this.aiActor.MoveToSafeSpot(this.aiAnimator.CurrentClipLength);
                }
                AttackBehaviorGroup attackBehaviorGroup = this.behaviorSpeculator.AttackBehaviorGroup;
                attackBehaviorGroup.AttackBehaviors[0].Probability = 0.0f;
                attackBehaviorGroup.AttackBehaviors[1].Probability = 1f;
                this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
                this.m_isEnemy = true;
            }
            else
                this.healthHaver.ApplyDamage(10000f, Vector2.zero, "Grip Master Finished", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
        }

        private bool ShouldBecomeEnemy()
        {
            if (!this.BecomeEnemeyPrereq.CheckConditionsFulfilled())
                return false;
            RoomHandler parentRoom = this.aiActor.ParentRoom;
            return (parentRoom == null || parentRoom.area.dimensions.x >= this.MinRoomWidth && parentRoom.area.dimensions.y >= this.MinRoomHeight) && (double) UnityEngine.Random.value < (double) this.BecomeEnemyChance;
        }
    }

