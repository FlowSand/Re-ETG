using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class KickableObject : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public float rollSpeed = 3f;
        [CheckAnimation(null)]
        public string[] rollAnimations;
        [CheckAnimation(null)]
        public string[] impactAnimations;
        public bool leavesGoopTrail;
        [ShowInInspectorIf("leavesGoopTrail", false)]
        public GoopDefinition goopType;
        [ShowInInspectorIf("leavesGoopTrail", false)]
        public float goopFrequency = 0.05f;
        [ShowInInspectorIf("leavesGoopTrail", false)]
        public float goopRadius = 1f;
        public bool triggersBreakTimer;
        [ShowInInspectorIf("triggersBreakTimer", false)]
        public float breakTimerLength = 3f;
        [ShowInInspectorIf("triggersBreakTimer", false)]
        public GameObject timerVFX;
        public bool RollingDestroysSafely = true;
        public string RollingBreakAnim = "red_barrel_break";
        private float m_goopElapsed;
        private DeadlyDeadlyGoopManager m_goopManager;
        private RoomHandler m_room;
        private bool m_isBouncingBack;
        private bool m_timerIsActive;
        [NonSerialized]
        public bool AllowTopWallTraversal;
        public IntVector2? m_lastDirectionKicked;
        private bool m_shouldDisplayOutline;
        private PlayerController m_lastInteractingPlayer;
        private DungeonData.Direction m_lastOutlineDirection = ~DungeonData.Direction.NORTH;
        private int m_lastSpriteId;

        private void Start()
        {
            this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnPlayerCollision);
        }

        public void Update()
        {
            if (this.m_shouldDisplayOutline)
            {
                DungeonData.Direction inverseDirection = DungeonData.GetInverseDirection(DungeonData.GetDirectionFromIntVector2(this.GetFlipDirection(this.m_lastInteractingPlayer.specRigidbody, out int _)));
                if (inverseDirection != this.m_lastOutlineDirection || this.sprite.spriteId != this.m_lastSpriteId)
                {
                    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
                    SpriteOutlineManager.AddSingleOutlineToSprite<tk2dSprite>(this.sprite, DungeonData.GetIntVector2FromDirection(inverseDirection), Color.white, 0.25f);
                }
                this.m_lastOutlineDirection = inverseDirection;
                this.m_lastSpriteId = this.sprite.spriteId;
            }
            if (!this.leavesGoopTrail || (double) this.specRigidbody.Velocity.magnitude <= 0.10000000149011612)
                return;
            this.m_goopElapsed += BraveTime.DeltaTime;
            if ((double) this.m_goopElapsed > (double) this.goopFrequency)
            {
                this.m_goopElapsed -= BraveTime.DeltaTime;
                if ((UnityEngine.Object) this.m_goopManager == (UnityEngine.Object) null)
                    this.m_goopManager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopType);
                this.m_goopManager.AddGoopCircle(this.sprite.WorldCenter, this.goopRadius + 0.1f);
            }
            if (!this.AllowTopWallTraversal || !GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(this.sprite.WorldCenter.ToIntVector2(VectorConversions.Floor)) || !GameManager.Instance.Dungeon.data[this.sprite.WorldCenter.ToIntVector2(VectorConversions.Floor)].IsFireplaceCell)
                return;
            MinorBreakable component = this.GetComponent<MinorBreakable>();
            if (!(bool) (UnityEngine.Object) component || component.IsBroken)
                return;
            component.Break(Vector2.zero);
            GameStatsManager.Instance.SetFlag(GungeonFlags.FLAG_ROLLED_BARREL_INTO_FIREPLACE, true);
        }

        public void ForceDeregister()
        {
            if (this.m_room != null)
                this.m_room.DeregisterInteractable((IPlayerInteractable) this);
            RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        }

        protected override void OnDestroy() => base.OnDestroy();

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            switch (BraveMathCollege.VectorToQuadrant(interactor.CenterPosition - this.specRigidbody.UnitCenter))
            {
                case 0:
                    return "tablekick_down";
                case 1:
                    shouldBeFlipped = true;
                    return "tablekick_right";
                case 2:
                    return "tablekick_up";
                case 3:
                    return "tablekick_right";
                default:
                    UnityEngine.Debug.Log((object) "fail");
                    return "tablekick_up";
            }
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            this.m_lastInteractingPlayer = interactor;
            this.m_shouldDisplayOutline = true;
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            this.ClearOutlines();
            this.m_shouldDisplayOutline = false;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            Bounds bounds = this.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + this.sprite.transform.position, bounds.max + this.sprite.transform.position);
            float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
        }

        public float GetOverrideMaxDistance() => -1f;

        public void Interact(PlayerController player)
        {
            GameManager.Instance.Dungeon.GetRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2()).DeregisterInteractable((IPlayerInteractable) this);
            RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
            this.Kick(player.specRigidbody);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_table_flip_01", player.gameObject);
            this.ClearOutlines();
            this.m_shouldDisplayOutline = false;
            if (!GameManager.Instance.InTutorial)
                return;
            GameManager.BroadcastRoomTalkDoerFsmEvent("playerRolledBarrel");
        }

        private void NoPits(
            SpeculativeRigidbody specRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation)
                return;
            Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
            {
                Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
                if (!GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) unitMidpoint))
                    return false;
                List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt((Vector3) unitMidpoint);
                if (platformsAt != null)
                {
                    for (int index = 0; index < platformsAt.Count; ++index)
                    {
                        if (platformsAt[index].PrimaryPixelCollider.ContainsPixel(pixel))
                            return false;
                    }
                }
                return true;
            });
            PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
            if (primaryPixelCollider != null)
            {
                IntVector2 intVector2 = pixelOffset - prevPixelOffset;
                if (intVector2 == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
                    validLocation = false;
                else if (intVector2 == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
                    validLocation = false;
                else if (intVector2 == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
                    validLocation = false;
                else if (intVector2 == IntVector2.Left && func(primaryPixelCollider.UpperLeft + pixelOffset) && func(primaryPixelCollider.LowerLeft + pixelOffset) && (!func(primaryPixelCollider.LowerRight + prevPixelOffset) || !func(primaryPixelCollider.UpperRight + prevPixelOffset)))
                    validLocation = false;
            }
            if (validLocation)
                return;
            this.StopRolling(true);
        }

        public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;

        private void OnPlayerCollision(CollisionData rigidbodyCollision)
        {
            PlayerController component1 = rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>();
            if (!this.RollingDestroysSafely || !((UnityEngine.Object) component1 != (UnityEngine.Object) null) || !component1.IsDodgeRolling)
                return;
            MinorBreakable component2 = this.GetComponent<MinorBreakable>();
            component2.destroyOnBreak = true;
            component2.makeParallelOnBreak = false;
            component2.breakAnimName = this.RollingBreakAnim;
            component2.explodesOnBreak = false;
            component2.Break(-rigidbodyCollision.Normal);
        }

        private void OnPreCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherPixelCollider)
        {
            MinorBreakable component = otherRigidbody.GetComponent<MinorBreakable>();
            if ((bool) (UnityEngine.Object) component && !component.onlyVulnerableToGunfire && !component.IsBig)
            {
                component.Break(this.specRigidbody.Velocity);
                PhysicsEngine.SkipCollision = true;
            }
            if (!(bool) (UnityEngine.Object) otherRigidbody || !(bool) (UnityEngine.Object) otherRigidbody.aiActor || otherRigidbody.aiActor.IsNormalEnemy)
                return;
            PhysicsEngine.SkipCollision = true;
        }

        private void OnCollision(CollisionData collision)
        {
            if (collision.collisionType == CollisionData.CollisionType.Rigidbody && (UnityEngine.Object) collision.OtherRigidbody.projectile != (UnityEngine.Object) null || ((double) BraveMathCollege.ActualSign(this.specRigidbody.Velocity.x) == 0.0 || (double) Mathf.Sign(collision.Normal.x) == (double) Mathf.Sign(this.specRigidbody.Velocity.x)) && ((double) BraveMathCollege.ActualSign(this.specRigidbody.Velocity.y) == 0.0 || (double) Mathf.Sign(collision.Normal.y) == (double) Mathf.Sign(this.specRigidbody.Velocity.y)) || ((double) BraveMathCollege.ActualSign(this.specRigidbody.Velocity.x) == 0.0 || (double) Mathf.Sign(collision.Contact.x - this.specRigidbody.UnitCenter.x) != (double) Mathf.Sign(this.specRigidbody.Velocity.x)) && ((double) BraveMathCollege.ActualSign(this.specRigidbody.Velocity.y) == 0.0 || (double) Mathf.Sign(collision.Contact.y - this.specRigidbody.UnitCenter.y) != (double) Mathf.Sign(this.specRigidbody.Velocity.y)))
                return;
            this.StopRolling(collision.collisionType == CollisionData.CollisionType.TileMap);
        }

        private bool IsRollAnimation()
        {
            for (int index = 0; index < this.rollAnimations.Length; ++index)
            {
                if (this.spriteAnimator.CurrentClip.name == this.rollAnimations[index])
                    return true;
            }
            return false;
        }

        private void StopRolling(bool bounceBack)
        {
            if (bounceBack && !this.m_isBouncingBack)
            {
                this.StartCoroutine(this.HandleBounceback());
            }
            else
            {
                this.spriteAnimator.Stop();
                if (this.IsRollAnimation())
                {
                    tk2dSpriteAnimationClip currentClip = this.spriteAnimator.CurrentClip;
                    this.spriteAnimator.Stop();
                    this.spriteAnimator.Sprite.SetSprite(currentClip.frames[currentClip.frames.Length - 1].spriteId);
                }
                this.specRigidbody.Velocity = Vector2.zero;
                MinorBreakable component = this.GetComponent<MinorBreakable>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                    component.onlyVulnerableToGunfire = false;
                this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
                this.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.NoPits);
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
                this.m_isBouncingBack = false;
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleBounceback()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new KickableObject__HandleBouncebackc__Iterator0()
            {
                _this = this
            };
        }

        private void ClearOutlines()
        {
            this.m_lastOutlineDirection = ~DungeonData.Direction.NORTH;
            this.m_lastSpriteId = -1;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        }

        [DebuggerHidden]
        private IEnumerator HandleBreakTimer()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new KickableObject__HandleBreakTimerc__Iterator1()
            {
                _this = this
            };
        }

        private void RemoveFromRoomHierarchy()
        {
            Transform hierarchyParent = this.transform.position.GetAbsoluteRoom().hierarchyParent;
            for (Transform transform = this.transform; (UnityEngine.Object) transform.parent != (UnityEngine.Object) null; transform = transform.parent)
            {
                if ((UnityEngine.Object) transform.parent == (UnityEngine.Object) hierarchyParent)
                {
                    transform.parent = (Transform) null;
                    break;
                }
            }
        }

        private void Kick(SpeculativeRigidbody kickerRigidbody)
        {
            if ((bool) (UnityEngine.Object) this.specRigidbody && !this.specRigidbody.enabled)
                return;
            this.RemoveFromRoomHierarchy();
            List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody.PrimaryPixelCollider);
            for (int index = 0; index < overlappingRigidbodies.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) overlappingRigidbodies[index] && (bool) (UnityEngine.Object) overlappingRigidbodies[index].minorBreakable && !overlappingRigidbodies[index].minorBreakable.IsBroken && !overlappingRigidbodies[index].minorBreakable.onlyVulnerableToGunfire && !overlappingRigidbodies[index].minorBreakable.OnlyBrokenByCode)
                    overlappingRigidbodies[index].minorBreakable.Break();
            }
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody, new int?(~CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox)));
            this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.NoPits);
            this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
            this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
            int quadrant;
            IntVector2 flipDirection = this.GetFlipDirection(kickerRigidbody, out quadrant);
            if (this.AllowTopWallTraversal)
                this.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBlocker));
            this.specRigidbody.Velocity = this.rollSpeed * flipDirection.ToVector2();
            tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.rollAnimations[quadrant]);
            bool flag = false;
            if (this.m_lastDirectionKicked.HasValue)
            {
                if (this.m_lastDirectionKicked.Value.y == 0 && flipDirection.y == 0)
                    flag = true;
                if (this.m_lastDirectionKicked.Value.x == 0 && flipDirection.x == 0)
                    flag = true;
            }
            if (clipByName != null && clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && flag)
                this.spriteAnimator.PlayFromFrame(clipByName, clipByName.loopStart);
            else
                this.spriteAnimator.Play(clipByName);
            if (this.triggersBreakTimer && !this.m_timerIsActive)
                this.StartCoroutine(this.HandleBreakTimer());
            MinorBreakable component = this.GetComponent<MinorBreakable>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            {
                component.breakAnimName = this.impactAnimations[quadrant];
                component.onlyVulnerableToGunfire = true;
            }
            GameManager.Instance.Dungeon.data[this.transform.PositionVector2().ToIntVector2()].isOccupied = false;
            this.m_lastDirectionKicked = new IntVector2?(flipDirection);
        }

        public IntVector2 GetFlipDirection(SpeculativeRigidbody kickerRigidbody, out int quadrant)
        {
            Vector2 inVec = this.specRigidbody.UnitCenter - kickerRigidbody.UnitCenter;
            quadrant = BraveMathCollege.VectorToQuadrant(inVec);
            return IntVector2.Cardinals[quadrant];
        }
    }

