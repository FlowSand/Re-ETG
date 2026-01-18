using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class ResourcefulRatMinesHiddenTrapdoor : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public PrototypeDungeonRoom TargetMinecartRoom;
        public PrototypeDungeonRoom FirstSecretRoom;
        public PrototypeDungeonRoom SecondSecretRoom;
        public TileIndexGrid OverridePitGrid;
        public Material BlendMaterial;
        public Material LockBlendMaterial;
        public InteractableLock Lock;
        public Texture2D StoneFloorTex;
        public Texture2D DirtFloorTex;
        public float ExplosionReactDistance = 8f;
        public SpeculativeRigidbody FlightCollider;
        [NonSerialized]
        public float RevealPercentage;
        public GameObject MinimapIcon;
        private bool m_hasCreatedRoom;
        private RoomHandler m_createdRoom;
        private Texture2D m_blendTex;
        private Color[] m_blendTexColors;
        private bool m_blendTexDirty;
        private HashSet<IntVector2> m_goopedSpots = new HashSet<IntVector2>();
        private float m_timeHovering;
        private bool m_revealing;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatMinesHiddenTrapdoor__Startc__Iterator0()
            {
                _this = this
            };
        }

        private void HandleFlightCollider(
            SpeculativeRigidbody specRigidbody,
            SpeculativeRigidbody sourceSpecRigidbody,
            CollisionData collisionData)
        {
            if (GameManager.Instance.IsLoadingLevel || this.Lock.IsLocked || !this.m_hasCreatedRoom)
                return;
            PlayerController component = specRigidbody.GetComponent<PlayerController>();
            if (!(bool) (UnityEngine.Object) component || !component.IsFlying)
                return;
            this.m_timeHovering += BraveTime.DeltaTime;
            if ((double) this.m_timeHovering <= 1.0)
                return;
            component.ForceFall();
            this.m_timeHovering = 0.0f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StaticReferenceManager.AllRatTrapdoors.Remove(this);
        }

        public void OnNearbyExplosion(Vector3 center)
        {
            if ((double) (this.transform.position.XY() + new Vector2(2f, 2f) - center.XY()).sqrMagnitude >= (double) this.ExplosionReactDistance * (double) this.ExplosionReactDistance)
                return;
            this.RevealPercentage = Mathf.Max(this.RevealPercentage, Mathf.Min(0.3f, this.RevealPercentage + 0.125f));
            this.UpdatePlayerDustups();
            this.BlendMaterial.SetFloat("_BlendMin", this.RevealPercentage);
            this.LockBlendMaterial.SetFloat("_BlendMin", this.RevealPercentage);
        }

        public void OnBlank()
        {
            if (GameManager.Instance.BestActivePlayer.CurrentRoom != this.transform.position.GetAbsoluteRoom())
                return;
            this.RevealPercentage = Mathf.Max(this.RevealPercentage, Mathf.Min(0.3f, this.RevealPercentage + 0.5f));
            this.UpdatePlayerDustups();
            this.BlendMaterial.SetFloat("_BlendMin", this.RevealPercentage);
            this.LockBlendMaterial.SetFloat("_BlendMin", this.RevealPercentage);
        }

        private void UpdatePlayerPositions()
        {
            if ((double) this.RevealPercentage >= 1.0)
                return;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                Vector2 spriteBottomCenter = (Vector2) allPlayer.SpriteBottomCenter;
                bool flag = false;
                if ((double) spriteBottomCenter.x > (double) this.transform.position.x && (double) spriteBottomCenter.y > (double) this.transform.position.y && (double) spriteBottomCenter.x < (double) this.transform.position.x + 4.0 && (double) spriteBottomCenter.y < (double) this.transform.position.y + 4.0 && (allPlayer.IsGrounded || allPlayer.IsFlying) && !allPlayer.IsGhost)
                {
                    flag = true;
                    allPlayer.OverrideDustUp = ResourceCache.Acquire("Global VFX/VFX_RatDoor_DustUp") as GameObject;
                    if ((double) allPlayer.Velocity.magnitude > 0.0)
                    {
                        Vector2 vector2 = spriteBottomCenter - this.transform.position.XY();
                        this.SoftUpdateRadius(new IntVector2(Mathf.FloorToInt(vector2.x * 16f), Mathf.FloorToInt(vector2.y * 16f)), 10, 2f * UnityEngine.Time.deltaTime);
                    }
                }
                if (!flag && (bool) (UnityEngine.Object) allPlayer.OverrideDustUp && allPlayer.OverrideDustUp.name.StartsWith("VFX_RatDoor_DustUp", StringComparison.Ordinal))
                    allPlayer.OverrideDustUp = (GameObject) null;
            }
        }

        private float CalcAvgRevealedness()
        {
            if ((double) this.RevealPercentage >= 1.0)
                return 1f;
            float num = 0.0f;
            for (int index1 = 0; index1 < 64; ++index1)
            {
                for (int index2 = 0; index2 < 64; ++index2)
                {
                    float r = this.m_blendTexColors[index2 * 64 + index1].r;
                    num += Mathf.Max(r, this.RevealPercentage);
                }
            }
            return num / 4096f;
        }

        private bool SoftUpdateRadius(IntVector2 pxCenter, int radius, float amt)
        {
            bool flag = false;
            for (int x = pxCenter.x - radius; x < pxCenter.x + radius; ++x)
            {
                for (int y = pxCenter.y - radius; y < pxCenter.y + radius; ++y)
                {
                    if (x > 0 && y > 0 && x < 64 && y < 64)
                    {
                        Color blendTexColor = this.m_blendTexColors[y * 64 + x];
                        float num1 = Vector2.Distance(pxCenter.ToVector2(), new Vector2((float) x, (float) y));
                        float num2 = Mathf.Clamp01(((float) radius - num1) / (float) radius);
                        float num3 = Mathf.Clamp01(blendTexColor.r + amt * num2);
                        if ((double) num3 != (double) blendTexColor.r)
                        {
                            blendTexColor.r = num3;
                            this.m_blendTexColors[y * 64 + x] = blendTexColor;
                            flag = true;
                            this.m_blendTexDirty = true;
                        }
                    }
                }
            }
            return flag;
        }

        private void UpdateGoopedCells()
        {
            if ((double) this.RevealPercentage >= 1.0)
                return;
            Vector2 vector2 = this.transform.position.XY();
            for (int index1 = 0; index1 < 16; ++index1)
            {
                for (int index2 = 0; index2 < 16; ++index2)
                {
                    IntVector2 intVector2_1 = ((new Vector2((float) index1 / 4f, (float) index2 / 4f) + vector2) / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
                    if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(intVector2_1) && !this.m_goopedSpots.Contains(intVector2_1))
                    {
                        this.m_goopedSpots.Add(intVector2_1);
                        IntVector2 intVector2_2 = new IntVector2(index1 * 4, index2 * 4);
                        for (int x = intVector2_2.x; x < intVector2_2.x + 4; ++x)
                        {
                            for (int y = intVector2_2.y; y < intVector2_2.y + 4; ++y)
                                this.m_blendTexColors[y * 64 + x] = new Color(1f, 1f, 1f, 1f);
                        }
                        this.m_blendTexDirty = true;
                    }
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator GraduallyReveal()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatMinesHiddenTrapdoor__GraduallyRevealc__Iterator1()
            {
                _this = this
            };
        }

        private void UpdatePlayerDustups()
        {
            if ((double) this.RevealPercentage < 1.0)
                return;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                if ((bool) (UnityEngine.Object) allPlayer && (bool) (UnityEngine.Object) allPlayer.OverrideDustUp && allPlayer.OverrideDustUp.name.StartsWith("VFX_RatDoor_DustUp", StringComparison.Ordinal))
                    allPlayer.OverrideDustUp = (GameObject) null;
            }
        }

        private void LateUpdate()
        {
            if ((double) this.RevealPercentage < 1.0)
            {
                this.UpdateGoopedCells();
                this.UpdatePlayerPositions();
                if (!this.m_revealing && (double) this.CalcAvgRevealedness() > 0.33000001311302185)
                    this.StartCoroutine(this.GraduallyReveal());
                if (!this.m_blendTexDirty)
                    return;
                this.m_blendTex.SetPixels(this.m_blendTexColors);
                this.m_blendTex.Apply();
            }
            else if (this.Lock.Suppress)
            {
                this.Lock.Suppress = false;
                Minimap.Instance.RegisterRoomIcon(this.transform.position.GetAbsoluteRoom(), this.MinimapIcon);
            }
            else
            {
                if (this.m_hasCreatedRoom || this.Lock.IsLocked)
                    return;
                this.Open();
            }
        }

        public void Open()
        {
            if (this.m_hasCreatedRoom)
                return;
            if (!this.m_hasCreatedRoom)
            {
                this.m_hasCreatedRoom = true;
                List<RoomHandler> roomHandlerList = GameManager.Instance.Dungeon.AddRuntimeRoomCluster(new List<PrototypeDungeonRoom>()
                {
                    this.TargetMinecartRoom,
                    this.FirstSecretRoom,
                    this.SecondSecretRoom
                }, new List<IntVector2>()
                {
                    IntVector2.Zero,
                    new IntVector2(73, 17),
                    new IntVector2(73, 36)
                }, new Action<RoomHandler>(this.ActuallyMakeAllTheFacewallsLookTheSameInTheRightSpots), DungeonData.LightGenerationStyle.RAT_HALLWAY);
                this.m_createdRoom = roomHandlerList[0];
                for (int index = 0; index < roomHandlerList.Count; ++index)
                    roomHandlerList[index].PreventMinimapUpdates = true;
            }
            if (this.m_createdRoom == null)
                return;
            this.AssignPitfallRoom(this.m_createdRoom);
            this.spriteAnimator.Play();
            this.StartCoroutine(this.HandleFlaggingCells());
        }

        [DebuggerHidden]
        private IEnumerator HandleFlaggingCells()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatMinesHiddenTrapdoor__HandleFlaggingCellsc__Iterator2()
            {
                _this = this
            };
        }

        public void ActuallyMakeAllTheFacewallsLookTheSameInTheRightSpots(RoomHandler target)
        {
            if ((UnityEngine.Object) target.area.prototypeRoom != (UnityEngine.Object) this.TargetMinecartRoom)
                return;
            DungeonData data = GameManager.Instance.Dungeon.data;
            for (int x = 0; x < target.area.dimensions.x; ++x)
            {
                for (int y = 0; y < target.area.dimensions.y + 2; ++y)
                {
                    IntVector2 intVector2 = target.area.basePosition + new IntVector2(x, y);
                    if (data.CheckInBoundsAndValid(intVector2))
                    {
                        CellData current = data[intVector2];
                        if (data.isAnyFaceWall(intVector2.x, intVector2.y))
                        {
                            TilesetIndexMetadata.TilesetFlagType key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER;
                            if (data.isFaceWallLower(intVector2.x, intVector2.y))
                                key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER;
                            int indexFromTupleArray = SecretRoomUtility.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[key], current.cellVisualData.roomVisualTypeIndex, 0.0f);
                            current.cellVisualData.faceWallOverrideIndex = indexFromTupleArray;
                        }
                    }
                }
            }
        }

        private void AssignPitfallRoom(RoomHandler target)
        {
            IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
            for (int x = 0; x < this.placeableWidth; ++x)
            {
                for (int y = -2; y < this.placeableHeight; ++y)
                {
                    CellData cellData = GameManager.Instance.Dungeon.data[new IntVector2(x, y) + intVector2];
                    cellData.targetPitfallRoom = target;
                    cellData.forceAllowGoop = false;
                }
            }
        }

        public void ConfigureOnPlacement(RoomHandler room)
        {
            IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
            for (int x = 1; x < this.placeableWidth - 1; ++x)
            {
                for (int y = 1; y < this.placeableHeight - 1; ++y)
                {
                    CellData cellData = GameManager.Instance.Dungeon.data[new IntVector2(x, y) + intVector2];
                    int num = x != 1 ? (y != 1 ? this.OverridePitGrid.topRightIndices.GetIndexByWeight() : this.OverridePitGrid.bottomRightIndices.GetIndexByWeight()) : (y != 1 ? this.OverridePitGrid.topLeftIndices.GetIndexByWeight() : this.OverridePitGrid.bottomLeftIndices.GetIndexByWeight());
                    cellData.cellVisualData.pitOverrideIndex = num;
                    cellData.forceAllowGoop = true;
                    cellData.type = CellType.PIT;
                    cellData.fallingPrevented = true;
                    cellData.cellVisualData.containsObjectSpaceStamp = true;
                    cellData.cellVisualData.containsWallSpaceStamp = true;
                }
            }
        }
    }

