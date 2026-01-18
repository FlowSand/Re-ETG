using System;

using FullInspector;
using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class ConsumeTargetBehavior : BasicAttackBehavior
    {
        public float PathInterval = 0.2f;
        public float PathMoveSpeed = 7f;
        public float MaxPathTime = 3f;
        public float TrackTime = 0.25f;
        public float PlayerClipSizePenalty = 0.15f;
        [InspectorCategory("Visuals")]
        public string TellAnim;
        [InspectorCategory("Visuals")]
        public string MoveAnim;
        [InspectorCategory("Visuals")]
        public string GrabAnim;
        [InspectorCategory("Visuals")]
        public string MissAnim;
        [InspectorCategory("Visuals")]
        public string GrabFinishAnim;
        private float m_startingHeightOffGround;
        private float m_startMoveSpeed;
        private Vector2 m_posOffset;
        private Vector2 m_targetPosition;
        private float m_repathTimer;
        private float m_timer;
        private Vector2 m_trackOffset;
        private bool m_trackDuringGrab;
        private Vector2? m_resetStartPos;
        private Vector2? m_resetEndPos;
        private PlayerController m_affectedPlayer;
        private ConsumeTargetBehavior.State m_state;

        public override void Start()
        {
            base.Start();
            this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
            this.m_startMoveSpeed = this.m_aiActor.MovementSpeed;
            this.m_posOffset = -(this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.transform.position.XY());
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_repathTimer);
            this.DecrementTimer(ref this.m_timer);
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            if (!this.IsReady() || (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody == (UnityEngine.Object) null)
                return BehaviorResult.Continue;
            this.m_state = ConsumeTargetBehavior.State.Tell;
            this.m_aiAnimator.PlayUntilCancelled(this.TellAnim);
            this.m_aiActor.ClearPath();
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num1 = (int) base.ContinuousUpdate();
            if (this.m_state == ConsumeTargetBehavior.State.Tell)
            {
                if (!this.m_aiAnimator.IsPlaying(this.TellAnim))
                {
                    int num2 = (int) AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Seeking_01", this.m_aiActor.gameObject);
                    this.m_state = ConsumeTargetBehavior.State.Path;
                    this.m_aiAnimator.PlayUntilCancelled(this.MoveAnim);
                    this.m_aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "pitfall")).anim.Prefix = "pitfall_attack";
                    this.m_timer = this.MaxPathTime;
                    this.m_repathTimer = 0.0f;
                    this.m_aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(this.PathMoveSpeed);
                    this.SetPlayerCollision(false);
                    this.Flatten(true);
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.m_state == ConsumeTargetBehavior.State.Path)
            {
                if ((double) this.PathToTarget() < 1.5)
                {
                    this.m_state = ConsumeTargetBehavior.State.Track;
                    this.m_timer = this.TrackTime;
                    if ((bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
                    {
                        this.m_trackOffset = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                        this.m_targetPosition = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.Ground);
                    }
                    else
                    {
                        this.m_trackOffset = Vector2.zero;
                        this.m_targetPosition = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground);
                    }
                }
                else if ((double) this.m_timer <= 0.0)
                {
                    this.m_state = ConsumeTargetBehavior.State.GrabBegin;
                    this.m_aiActor.ClearPath();
                    this.m_aiAnimator.PlayUntilFinished(this.GrabAnim);
                    this.m_trackDuringGrab = false;
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.m_state == ConsumeTargetBehavior.State.Track)
            {
                this.TrackToTarget(Vector2.Lerp(Vector2.zero, this.m_trackOffset, this.m_timer / this.TrackTime));
                if ((double) this.m_timer <= 0.0)
                {
                    this.m_state = ConsumeTargetBehavior.State.GrabBegin;
                    this.m_aiActor.ClearPath();
                    this.m_aiAnimator.PlayUntilFinished(this.GrabAnim);
                    this.m_aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "pitfall")).anim.Prefix = "pitfall";
                    this.m_trackDuringGrab = true;
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.m_state == ConsumeTargetBehavior.State.GrabBegin)
            {
                if (this.m_trackDuringGrab)
                    this.TrackToTarget(Vector2.zero);
                if (!this.m_aiAnimator.IsPlaying(this.GrabAnim))
                {
                    this.GetSafeEndPoint();
                    return ContinuousBehaviorResult.Finished;
                }
            }
            else if (this.m_state == ConsumeTargetBehavior.State.GrabSuccess)
            {
                if (!this.m_aiAnimator.IsPlaying(this.GrabAnim))
                {
                    this.UnconsumePlayer(true);
                    return ContinuousBehaviorResult.Finished;
                }
                this.TrackToSafeEndPoint();
            }
            else if (this.m_state == ConsumeTargetBehavior.State.Miss)
            {
                if (!this.m_aiAnimator.IsPlaying(this.MissAnim))
                    return ContinuousBehaviorResult.Finished;
                this.TrackToSafeEndPoint();
            }
            else if (this.m_state == ConsumeTargetBehavior.State.WaitingForFinish)
            {
                if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody || !this.m_behaviorSpeculator.TargetRigidbody.GroundPixelCollider.Overlaps(this.m_aiActor.specRigidbody.GroundPixelCollider) && !this.m_behaviorSpeculator.TargetRigidbody.GroundPixelCollider.Overlaps(this.m_aiActor.specRigidbody.HitboxPixelCollider))
                {
                    this.m_aiAnimator.PlayUntilFinished(this.GrabFinishAnim);
                    this.m_state = ConsumeTargetBehavior.State.GrabFinish;
                    this.Flatten(false);
                }
            }
            else if (this.m_state == ConsumeTargetBehavior.State.GrabFinish && !this.m_aiAnimator.IsPlaying(this.GrabFinishAnim))
                return ContinuousBehaviorResult.Finished;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if ((UnityEngine.Object) this.m_affectedPlayer != (UnityEngine.Object) null)
                this.UnconsumePlayer(false);
            this.m_state = ConsumeTargetBehavior.State.Idle;
            this.SetPlayerCollision(true);
            if ((bool) (UnityEngine.Object) this.m_aiActor)
                this.m_aiActor.specRigidbody.ClearSpecificCollisionExceptions();
            this.m_aiActor.MovementSpeed = this.m_startMoveSpeed;
            this.Flatten(false);
            this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (ConsumeTargetBehavior));
            if (this.m_resetEndPos.HasValue)
            {
                this.m_aiActor.transform.position = (Vector3) this.m_resetEndPos.Value;
                this.m_aiActor.specRigidbody.Reinitialize();
            }
            this.m_resetStartPos = new Vector2?();
            this.m_resetEndPos = new Vector2?();
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override void Destroy()
        {
            if ((bool) (UnityEngine.Object) this.m_affectedPlayer)
                this.UnconsumePlayer(false);
            base.Destroy();
        }

        private void AnimationEventTriggered(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frameNo)
        {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
            if (this.m_state == ConsumeTargetBehavior.State.GrabBegin && frame.eventInfo == "hit")
            {
                this.ForceBlank();
                bool flag = false;
                if ((bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
                {
                    PlayerController gameActor = this.m_behaviorSpeculator.TargetRigidbody.gameActor as PlayerController;
                    if ((bool) (UnityEngine.Object) gameActor)
                    {
                        this.m_aiActor.specRigidbody.RegisterSpecificCollisionException(gameActor.specRigidbody);
                        if (gameActor.CanBeGrabbed && gameActor.specRigidbody.HitboxPixelCollider.Overlaps(this.m_aiActor.specRigidbody.HitboxPixelCollider))
                        {
                            this.ConsumePlayer(gameActor);
                            flag = true;
                            this.m_state = ConsumeTargetBehavior.State.GrabSuccess;
                        }
                    }
                }
                this.Flatten(false);
                this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (ConsumeTargetBehavior));
                if (this.m_state != ConsumeTargetBehavior.State.GrabSuccess)
                {
                    this.m_state = ConsumeTargetBehavior.State.Miss;
                    this.m_aiAnimator.PlayUntilFinished(this.MissAnim);
                }
                this.GetSafeEndPoint();
            }
            if ((this.m_state == ConsumeTargetBehavior.State.GrabSuccess || this.m_state == ConsumeTargetBehavior.State.Miss) && frame.eventInfo == "enable_colliders")
            {
                this.SetPlayerCollision(true);
                this.Flatten(false);
            }
            if (this.m_state == ConsumeTargetBehavior.State.GrabSuccess && frame.eventInfo == "release")
            {
                this.UnconsumePlayer(true);
                this.Flatten(true);
            }
            if (this.m_state != ConsumeTargetBehavior.State.GrabSuccess || !(frame.eventInfo == "static"))
                return;
            this.m_state = ConsumeTargetBehavior.State.WaitingForFinish;
        }

        private void SetPlayerCollision(bool collision)
        {
            if (collision)
                this.m_aiActor.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox));
            else
                this.m_aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox));
        }

        private void Flatten(bool flatten)
        {
            this.m_aiActor.sprite.IsPerpendicular = !flatten;
            this.m_aiActor.specRigidbody.CollideWithOthers = !flatten;
            this.m_aiActor.IsGone = flatten;
            if (flatten)
            {
                this.m_startingHeightOffGround = this.m_aiActor.sprite.HeightOffGround;
                this.m_aiActor.sprite.HeightOffGround = -1.5f;
                this.m_aiActor.sprite.UpdateZDepth();
            }
            else
            {
                this.m_aiActor.sprite.HeightOffGround = this.m_startingHeightOffGround;
                this.m_aiActor.sprite.UpdateZDepth();
            }
        }

        private float PathToTarget()
        {
            if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
                return -1f;
            if ((double) this.m_repathTimer <= 0.0)
            {
                this.m_aiActor.PathfindToPosition(this.m_behaviorSpeculator.TargetRigidbody.UnitCenter, canPassOccupied: true);
                this.m_repathTimer = this.PathInterval;
            }
            return Vector2.Distance(this.m_behaviorSpeculator.TargetRigidbody.HitboxPixelCollider.UnitCenter, this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter);
        }

        private void TrackToTarget(Vector2 additionalOffset)
        {
            if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
                return;
            this.m_targetPosition = Vector2.MoveTowards(this.m_targetPosition, this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.Ground), 10f * this.m_deltaTime);
            this.m_aiActor.transform.position = (Vector3) (this.m_targetPosition + this.m_posOffset + additionalOffset);
            this.m_aiActor.specRigidbody.Reinitialize();
        }

        private void TrackToSafeEndPoint()
        {
            if (!this.m_resetStartPos.HasValue || !this.m_resetEndPos.HasValue)
                return;
            this.m_aiActor.transform.position = (Vector3) Vector2.Lerp(this.m_resetStartPos.Value, this.m_resetEndPos.Value, this.m_aiAnimator.CurrentClipProgress);
            this.m_aiActor.specRigidbody.Reinitialize();
        }

        private void GetSafeEndPoint()
        {
            if (!GameManager.HasInstance || (UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
                return;
            DungeonData data = GameManager.Instance.Dungeon.data;
            SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
            Vector2[] vector2Array = new Vector2[6]
            {
                specRigidbody.UnitBottomLeft,
                specRigidbody.UnitBottomCenter,
                specRigidbody.UnitBottomRight,
                specRigidbody.UnitTopLeft,
                specRigidbody.UnitTopCenter,
                specRigidbody.UnitTopRight
            };
            bool flag = false;
            for (int index = 0; index < vector2Array.Length; ++index)
            {
                IntVector2 intVector2 = vector2Array[index].ToIntVector2(VectorConversions.Floor);
                if (!data.CheckInBoundsAndValid(intVector2) || data.isWall(intVector2) || data.isTopWall(intVector2.x, intVector2.y) || data[intVector2].isOccupied)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
                return;
            CellValidator cellValidator = (CellValidator) (c =>
            {
                for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
                {
                    int x = c.x + index1;
                    for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
                    {
                        int y = c.y + index2;
                        if (GameManager.Instance.Dungeon.data.isTopWall(x, y))
                            return false;
                    }
                }
                return true;
            });
            Vector2 vector2 = this.m_aiActor.specRigidbody.UnitBottomCenter - this.m_aiActor.transform.position.XY();
            IntVector2? nearestAvailableCell = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)).GetNearestAvailableCell(this.m_aiActor.specRigidbody.UnitCenter, new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator);
            if (nearestAvailableCell.HasValue)
            {
                this.m_resetStartPos = new Vector2?((Vector2) this.m_aiActor.transform.position);
                this.m_resetEndPos = new Vector2?(Pathfinder.GetClearanceOffset(nearestAvailableCell.Value, this.m_aiActor.Clearance).WithY((float) nearestAvailableCell.Value.y) - vector2);
            }
            else
            {
                this.m_resetStartPos = new Vector2?();
                this.m_resetEndPos = new Vector2?();
            }
        }

        private void ConsumePlayer(PlayerController player)
        {
            player.specRigidbody.Velocity = Vector2.zero;
            player.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
            player.ToggleRenderer(false, "consumed");
            player.ToggleHandRenderers(false, "consumed");
            player.ToggleGunRenderers(false, "consumed");
            player.CurrentInputState = PlayerInputState.NoInput;
            player.healthHaver.IsVulnerable = false;
            this.m_affectedPlayer = player;
        }

        private void UnconsumePlayer(bool punishPlayer)
        {
            if ((bool) (UnityEngine.Object) this.m_affectedPlayer)
            {
                this.m_affectedPlayer.healthHaver.IsVulnerable = true;
                if (punishPlayer)
                    this.PunishPlayer();
            }
            if ((bool) (UnityEngine.Object) this.m_affectedPlayer)
            {
                this.m_affectedPlayer.ToggleRenderer(true, "consumed");
                this.m_affectedPlayer.ToggleHandRenderers(true, "consumed");
                this.m_affectedPlayer.ToggleGunRenderers(true, "consumed");
                this.m_affectedPlayer.CurrentInputState = PlayerInputState.AllInput;
                this.m_affectedPlayer.DoSpitOut();
            }
            this.m_affectedPlayer = (PlayerController) null;
            if (!(bool) (UnityEngine.Object) this.m_aiActor)
                return;
            this.m_aiActor.specRigidbody.ClearSpecificCollisionExceptions();
        }

        private void PunishPlayer()
        {
            if (!(bool) (UnityEngine.Object) this.m_affectedPlayer || !(bool) (UnityEngine.Object) this.m_aiActor)
                return;
            if (this.m_affectedPlayer.HasActiveItem(GlobalItemIds.EitrShield) && PickupObjectDatabase.HasInstance && this.m_aiActor.AdditionalSafeItemDrops != null)
            {
                this.m_affectedPlayer.RemoveActiveItem(GlobalItemIds.EitrShield);
                this.m_aiActor.AdditionalSafeItemDrops.Add(PickupObjectDatabase.Instance.InternalGetById(GlobalItemIds.EitrShield));
            }
            else
            {
                if ((bool) (UnityEngine.Object) this.m_affectedPlayer.healthHaver)
                    this.m_affectedPlayer.healthHaver.ApplyDamage(!this.m_aiActor.IsBlackPhantom ? 0.5f : 1f, Vector2.zero, this.m_aiActor.GetActorName());
                if ((bool) (UnityEngine.Object) this.m_affectedPlayer && this.m_affectedPlayer.ownerlessStatModifiers != null && (UnityEngine.Object) this.m_affectedPlayer.stats != (UnityEngine.Object) null)
                {
                    this.m_affectedPlayer.ownerlessStatModifiers.Add(new StatModifier()
                    {
                        statToBoost = PlayerStats.StatType.TarnisherClipCapacityMultiplier,
                        amount = -this.PlayerClipSizePenalty,
                        modifyType = StatModifier.ModifyMethod.ADDITIVE
                    });
                    this.m_affectedPlayer.stats.RecalculateStats(this.m_affectedPlayer);
                }
                if ((bool) (UnityEngine.Object) this.m_affectedPlayer && (bool) (UnityEngine.Object) this.m_affectedPlayer.CurrentGun && this.m_affectedPlayer.CurrentGun.ammo > 0)
                    this.m_affectedPlayer.CurrentGun.ammo = Mathf.RoundToInt((float) this.m_affectedPlayer.CurrentGun.ammo * 0.85f);
                if (!GameStatsManager.HasInstance)
                    return;
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_TARNISHED, 1f);
            }
        }

        public void ForceBlank(float overrideRadius = 5f, float overrideTimeAtMaxRadius = 0.65f)
        {
            if (!(bool) (UnityEngine.Object) this.m_aiActor || !(bool) (UnityEngine.Object) this.m_aiActor.specRigidbody)
                return;
            SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
            silencerInstance.ForceNoDamage = true;
            silencerInstance.TriggerSilencer(this.m_aiActor.specRigidbody.UnitCenter, 50f, overrideRadius, (GameObject) null, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, overrideTimeAtMaxRadius, (PlayerController) null, false, true);
        }

        public enum State
        {
            Idle,
            Tell,
            Path,
            Track,
            GrabBegin,
            GrabSuccess,
            Miss,
            WaitingForFinish,
            GrabFinish,
        }
    }

