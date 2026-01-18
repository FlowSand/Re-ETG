using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using HutongGames.PlayMaker;
using UnityEngine;

using Dungeonator;

#nullable disable

public class SecretFloorInteractableController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public bool IsResourcefulRatPit;
        public bool GoesToRatFloor;
        public string targetLevelName;
        public List<InteractableLock> WorldLocks;
        public SpeculativeRigidbody FlightCollider;
        public GlobalDungeonData.ValidTilesets TargetTileset = GlobalDungeonData.ValidTilesets.SEWERGEON;
        public TileIndexGrid OverridePitIndex;
        private bool m_hasOpened;
        public tk2dSpriteAnimator cryoAnimator;
        public string cryoArriveAnimation;
        public string cyroDepartAnimation;
        private FsmBool m_cryoBool;
        private FsmBool m_normalBool;
        private float m_timeHovering;
        private bool m_isLoading;

        public override GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 loc,
            bool deferConfiguration = false)
        {
            if (this.IsResourcefulRatPit)
            {
                IntVector2 intVector2 = loc + targetRoom.area.basePosition;
                int x = intVector2.x;
                int num1 = intVector2.x + this.placeableWidth;
                int y = intVector2.y;
                int num2 = intVector2.y + this.placeableHeight;
                if (this.GoesToRatFloor)
                {
                    ++x;
                    ++y;
                    --num1;
                    --num2;
                }
                for (int index1 = x; index1 < num1; ++index1)
                {
                    for (int index2 = y; index2 < num2; ++index2)
                    {
                        CellData cellData = GameManager.Instance.Dungeon.data.cellData[index1][index2];
                        cellData.type = CellType.PIT;
                        cellData.fallingPrevented = true;
                        if ((UnityEngine.Object) this.OverridePitIndex != (UnityEngine.Object) null)
                        {
                            cellData.cellVisualData.HasTriggeredPitVFX = true;
                            cellData.cellVisualData.PitVFXCooldown = float.MaxValue;
                            cellData.cellVisualData.pitOverrideIndex = index2 != intVector2.y + this.placeableHeight - 1 ? this.OverridePitIndex.centerIndices.GetIndexByWeight() : this.OverridePitIndex.topIndices.GetIndexByWeight();
                        }
                    }
                }
            }
            return base.InstantiateObject(targetRoom, loc, deferConfiguration);
        }

        private void Start()
        {
            RoomHandler absoluteParentRoom = this.GetAbsoluteParentRoom();
            for (int index = 0; index < this.WorldLocks.Count; ++index)
                absoluteParentRoom.RegisterInteractable((IPlayerInteractable) this.WorldLocks[index]);
            if (this.IsResourcefulRatPit)
            {
                this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered);
                this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.HandleTriggerExited);
                this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerRemain);
            }
            if ((bool) (UnityEngine.Object) this.FlightCollider)
                this.FlightCollider.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleFlightCollider);
            TalkDoerLite componentInChildren = absoluteParentRoom.hierarchyParent.GetComponentInChildren<TalkDoerLite>();
            if (!(bool) (UnityEngine.Object) componentInChildren || !componentInChildren.name.Contains("CryoButton"))
                return;
            componentInChildren.OnGenericFSMActionA += new System.Action(this.SwitchToCryoElevator);
            componentInChildren.OnGenericFSMActionB += new System.Action(this.RescindCryoElevator);
            this.m_cryoBool = componentInChildren.playmakerFsm.FsmVariables.GetFsmBool("IS_CRYO");
            this.m_normalBool = componentInChildren.playmakerFsm.FsmVariables.GetFsmBool("IS_NORMAL");
            this.m_cryoBool.Value = false;
            this.m_normalBool.Value = true;
        }

        private void RescindCryoElevator()
        {
            this.m_cryoBool.Value = false;
            this.m_normalBool.Value = true;
            if (!(bool) (UnityEngine.Object) this.cryoAnimator || string.IsNullOrEmpty(this.cyroDepartAnimation))
                return;
            this.cryoAnimator.Play(this.cyroDepartAnimation);
        }

        private void SwitchToCryoElevator()
        {
            this.m_cryoBool.Value = true;
            this.m_normalBool.Value = false;
            if (!(bool) (UnityEngine.Object) this.cryoAnimator || string.IsNullOrEmpty(this.cryoArriveAnimation))
                return;
            this.cryoAnimator.Play(this.cryoArriveAnimation);
        }

        private void HandleFlightCollider(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (GameManager.Instance.IsLoadingLevel || !this.IsValidForUse())
                return;
            PlayerController component = specRigidbody.GetComponent<PlayerController>();
            if (!(bool) (UnityEngine.Object) component || !component.IsFlying || this.m_isLoading || GameManager.Instance.IsLoadingLevel || string.IsNullOrEmpty(this.targetLevelName))
                return;
            this.m_timeHovering += BraveTime.DeltaTime;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(component);
                if (component.IsFlying && !otherPlayer.IsFlying && !otherPlayer.IsGhost)
                    this.m_timeHovering = 0.0f;
            }
            if ((double) this.m_timeHovering <= 0.5)
                return;
            this.m_isLoading = true;
            GameManager.Instance.LoadCustomLevel(this.targetLevelName);
        }

        private void HandleTriggerRemain(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (!this.IsValidForUse() || this.m_isLoading)
                return;
            this.StartCoroutine(this.FrameDelayedTriggerRemainCheck(specRigidbody.GetComponent<PlayerController>()));
        }

        [DebuggerHidden]
        private IEnumerator FrameDelayedTriggerRemainCheck(PlayerController targetPlayer)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new SecretFloorInteractableController__FrameDelayedTriggerRemainCheckc__Iterator0()
            {
                targetPlayer = targetPlayer,
                _this = this
            };
        }

        private void HandleTriggerEntered(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            PlayerController component = specRigidbody.GetComponent<PlayerController>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            if (this.m_cryoBool != null && this.m_cryoBool.Value)
                component.LevelToLoadOnPitfall = "midgamesave";
            else
                component.LevelToLoadOnPitfall = this.targetLevelName;
        }

        private void HandleTriggerExited(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody)
        {
            PlayerController component = specRigidbody.GetComponent<PlayerController>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.LevelToLoadOnPitfall = string.Empty;
        }

        private void Update()
        {
            if (this.m_hasOpened || !this.IsResourcefulRatPit || !this.IsValidForUse())
                return;
            this.m_hasOpened = true;
            this.spriteAnimator.Play();
            IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
            int x = intVector2.x;
            int num1 = intVector2.x + this.placeableWidth;
            int y = intVector2.y;
            int num2 = intVector2.y + this.placeableHeight;
            if (this.GoesToRatFloor)
            {
                ++x;
                ++y;
                --num1;
                --num2;
            }
            for (int index1 = x; index1 < num1; ++index1)
            {
                for (int index2 = y; index2 < num2; ++index2)
                {
                    if (!this.GoesToRatFloor || (index1 != intVector2.x + 1 || index2 != intVector2.y + 1) && (index1 != intVector2.x + 1 || index2 != intVector2.y + this.placeableHeight - 2) && (index1 != intVector2.x + this.placeableWidth - 2 || index2 != intVector2.y + 1) && (index1 != intVector2.x + this.placeableWidth - 2 || index2 != intVector2.y + this.placeableHeight - 2))
                        GameManager.Instance.Dungeon.data.cellData[index1][index2].fallingPrevented = false;
                }
            }
        }

        private bool IsValidForUse()
        {
            bool flag = true;
            for (int index = 0; index < this.WorldLocks.Count; ++index)
            {
                if (this.WorldLocks[index].IsLocked || this.WorldLocks[index].spriteAnimator.IsPlaying(this.WorldLocks[index].spriteAnimator.CurrentClip))
                    flag = false;
            }
            return flag;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
            this.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
            this.sprite.UpdateZDepth();
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (this.IsResourcefulRatPit || !this.IsValidForUse())
                return 1000f;
            Bounds bounds = this.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
            float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
        }

        public float GetOverrideMaxDistance() => -1f;

        public void Interact(PlayerController player)
        {
            if (!this.IsValidForUse())
                return;
            GameManager.Instance.LoadCustomLevel(this.targetLevelName);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

