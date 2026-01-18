using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class DraGunController : BraveBehaviour
    {
        public bool isAdvanced;
        public DraGunHeadController head;
        public GameObject neck;
        public GameObject wings;
        public GameObject headShootPoint;
        public GameObject leftArm;
        public GameObject rightArm;
        public float NearDeathTriggerHealth = 150f;
        public GameObject skyRocket;
        public GameObject skyBoulder;
        public AIActor AdvancedDraGunPrefab;
        [NonSerialized]
        public Vector2 SpotlightVelocity;
        [NonSerialized]
        public float SpotlightRadius = 3f;
        public float SpotlightShrink;
        public GameObject SpotlightSprite;
        [Header("For Brents")]
        public Material SpotlightMaterial;
        public Material PitCausticsMaterial;
        private float m_elapsedFlap;
        private bool m_isFlapping;
        private float m_babyCheckTimer;
        private AdditionalBraveLight m_spotlight;
        private GameObject m_spotlightSprite;
        private float m_elapsedSpotlight;
        private bool m_isMotionRestricted;
        private tk2dSpriteAnimator m_wingsAnimator;
        private EmbersController m_embers;
        private tk2dSpriteAnimator m_transitionDummy;
        private bool m_hasDoneIntro;
        private int m_minPlayerY;
        private int m_maxPlayerY;

        public float? OverrideTargetX { get; set; }

        public bool TrackPlayerWithHead { get; set; }

        public bool IsNearDeath { get; set; }

        public bool IsTransitioning { get; set; }

        public bool SpotlightEnabled { get; set; }

        public Vector2 SpotlightPos { get; set; }

        public float SpotlightSpeed { get; set; }

        public float SpotlightSmoothTime { get; set; }

        public bool HasDoneIntro
        {
            get => this.m_hasDoneIntro;
            set
            {
                if (!this.m_hasDoneIntro && value)
                    this.RestrictMotion(true);
                this.m_hasDoneIntro = value;
            }
        }

        public bool HasConvertedBaby { get; set; }

        public void Start()
        {
            this.TrackPlayerWithHead = true;
            this.specRigidbody.Initialize();
            float unitBottom = this.specRigidbody.PrimaryPixelCollider.UnitBottom;
            foreach (TileSpriteClipper componentsInChild in this.GetComponentsInChildren<TileSpriteClipper>(true))
            {
                componentsInChild.clipMode = TileSpriteClipper.ClipMode.ClipBelowY;
                componentsInChild.clipY = unitBottom;
            }
            if (!this.isAdvanced)
            {
                this.m_transitionDummy = this.transform.Find("TransitionDummy").GetComponent<tk2dSpriteAnimator>();
                this.healthHaver.minimumHealth = this.NearDeathTriggerHealth;
                this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
            }
            if ((bool) (UnityEngine.Object) this.wings && (bool) (UnityEngine.Object) this.wings.GetComponent<tk2dSpriteAnimator>())
                this.m_wingsAnimator = this.wings.GetComponent<tk2dSpriteAnimator>();
            foreach (Renderer componentsInChild in this.head.GetComponentsInChildren<TrailRenderer>(true))
                componentsInChild.sortingLayerName = "Foreground";
        }

        private void HandleFlaps()
        {
            if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH)
                return;
            if (!(bool) (UnityEngine.Object) this.m_embers)
                this.m_embers = GlobalSparksDoer.GetEmbersController();
            if (this.m_isFlapping)
                this.m_elapsedFlap += BraveTime.DeltaTime * 2f;
            if (!this.m_isFlapping)
                this.m_elapsedFlap -= BraveTime.DeltaTime / 3f;
            this.m_elapsedFlap = Mathf.Clamp01(this.m_elapsedFlap);
            if ((double) this.m_elapsedFlap <= 0.0)
            {
                this.m_embers.AdditionalVortices.Clear();
            }
            else
            {
                Vector4 vector4_1 = new Vector4(this.wings.transform.position.x + 6f, this.wings.transform.position.y + 5f, 15f * this.m_elapsedFlap, -11f * this.m_elapsedFlap);
                Vector4 vector4_2 = new Vector4(this.wings.transform.position.x + 22f, this.wings.transform.position.y + 5f, 15f * this.m_elapsedFlap, 11f * this.m_elapsedFlap);
                if (this.m_embers.AdditionalVortices.Count < 1)
                    this.m_embers.AdditionalVortices.Add(vector4_1);
                else
                    this.m_embers.AdditionalVortices[0] = vector4_1;
                if (this.m_embers.AdditionalVortices.Count < 2)
                    this.m_embers.AdditionalVortices.Add(vector4_2);
                else
                    this.m_embers.AdditionalVortices[1] = vector4_2;
            }
        }

        public void Update()
        {
            if (this.OverrideTargetX.HasValue)
                this.head.TargetX = new float?(this.specRigidbody.HitboxPixelCollider.UnitCenter.x + this.OverrideTargetX.Value);
            else if ((bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
                this.head.TargetX = new float?(this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox).x);
            this.head.UpdateHead();
            if ((bool) (UnityEngine.Object) this.m_wingsAnimator)
            {
                if (!this.IsTransitioning)
                    this.m_isFlapping = this.m_wingsAnimator.IsPlaying("wing_flap");
                this.HandleFlaps();
            }
            if (!this.isAdvanced && !this.IsNearDeath && (double) this.healthHaver.GetCurrentHealth() <= (double) this.NearDeathTriggerHealth)
                this.StartCoroutine(this.ConvertToNearDeath());
            if (this.SpotlightEnabled)
            {
                this.m_elapsedSpotlight += BraveTime.DeltaTime;
                if ((bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
                    this.SpotlightPos = Vector2.SmoothDamp(this.SpotlightPos, this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox), ref this.SpotlightVelocity, this.SpotlightSmoothTime, this.SpotlightSpeed, BraveTime.DeltaTime);
                Vector2 position = (Vector2) this.headShootPoint.transform.position;
                (this.SpotlightPos - position).ToAngle();
                if (!(bool) (UnityEngine.Object) this.m_spotlight)
                {
                    GameObject gameObject = new GameObject("dragunSpotlight");
                    this.m_spotlight = gameObject.AddComponent<AdditionalBraveLight>();
                    this.m_spotlight.CustomLightMaterial = this.SpotlightMaterial;
                    this.m_spotlight.UsesCustomMaterial = true;
                    this.m_spotlight.LightColor = new Color(1f, 0.286274523f, 0.419607848f);
                    this.m_spotlightSprite = UnityEngine.Object.Instantiate<GameObject>(this.SpotlightSprite);
                    this.m_spotlightSprite.transform.parent = gameObject.transform;
                    this.m_spotlightSprite.transform.localPosition = Vector3.zero;
                }
                else if (!this.m_spotlight.gameObject.activeSelf)
                    this.m_spotlight.gameObject.SetActive(true);
                float num = 5f;
                this.m_spotlight.LightIntensity = Mathf.Lerp(0.0f, GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW ? 92f : 50f, Mathf.Clamp01(this.m_elapsedSpotlight / num));
                this.m_spotlight.LightRadius = (float) ((double) this.SpotlightRadius * 2.0 + 1.25);
                this.m_spotlight.CustomLightMaterial.SetVector("_LightOrigin", new Vector4(position.x, position.y, 0.0f, 0.0f));
                this.m_spotlight.transform.position = this.SpotlightPos.ToVector3ZisY();
                this.m_spotlightSprite.transform.localScale = new Vector3(this.SpotlightShrink, this.SpotlightShrink, 1f);
            }
            else
            {
                this.m_elapsedSpotlight = 0.0f;
                if ((bool) (UnityEngine.Object) this.m_spotlight && this.m_spotlight.gameObject.activeSelf)
                    this.m_spotlight.gameObject.SetActive(false);
            }
            if (this.isAdvanced || !this.aiActor.HasBeenEngaged || this.HasConvertedBaby)
                return;
            this.m_babyCheckTimer -= BraveTime.DeltaTime;
            if ((double) this.m_babyCheckTimer > 0.0)
                return;
            BabyDragunController objectOfType = UnityEngine.Object.FindObjectOfType<BabyDragunController>();
            if ((bool) (UnityEngine.Object) objectOfType)
            {
                objectOfType.BecomeEnemy(this);
                this.HasConvertedBaby = true;
            }
            this.m_babyCheckTimer = 1f;
        }

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.m_embers)
                this.m_embers.AdditionalVortices.Clear();
            if ((bool) (UnityEngine.Object) this.PitCausticsMaterial)
            {
                this.PitCausticsMaterial.SetFloat("_LightCausticPower", 4f);
                this.PitCausticsMaterial.SetFloat("_ValueMaximum", 50f);
                this.PitCausticsMaterial.SetFloat("_ValueMinimum", 0.0f);
            }
            if ((bool) (UnityEngine.Object) this.m_spotlight)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.m_spotlight.gameObject);
            this.RestrictMotion(false);
            this.ModifyCamera(false);
            this.BlockPitTiles(false);
            SilencerInstance.s_MaxRadiusLimiter = new float?();
            if ((bool) (UnityEngine.Object) this.healthHaver)
                this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
            base.OnDestroy();
        }

        private void OnPreDeath(Vector2 finalDirection) => this.RestrictMotion(false);

        private void PlayerMovementRestrictor(
            SpeculativeRigidbody playerSpecRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation)
                return;
            if (pixelOffset.y < prevPixelOffset.y && playerSpecRigidbody.PixelColliders[0].MinY + pixelOffset.y < this.m_minPlayerY)
                validLocation = false;
            if (pixelOffset.y <= prevPixelOffset.y || playerSpecRigidbody.PixelColliders[0].MaxY + pixelOffset.y < this.m_maxPlayerY)
                return;
            validLocation = false;
        }

        public void RestrictMotion(bool value)
        {
            if (this.m_isMotionRestricted == value)
                return;
            if (value)
            {
                if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
                    return;
                this.m_minPlayerY = (this.aiActor.ParentRoom.area.basePosition.y + DraGunRoomPlaceable.HallHeight) * 16 /*0x10*/ + 8;
                this.m_maxPlayerY = (this.aiActor.ParentRoom.area.basePosition.y + this.aiActor.ParentRoom.area.dimensions.y - 1) * 16 /*0x10*/;
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                    GameManager.Instance.AllPlayers[index].specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
            }
            else
            {
                if (!GameManager.HasInstance || GameManager.IsReturningToBreach)
                    return;
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                    if ((bool) (UnityEngine.Object) allPlayer)
                        allPlayer.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
                }
            }
            this.m_isMotionRestricted = value;
        }

        public void ModifyCamera(bool value)
        {
            if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
                return;
            CameraController cameraController = GameManager.Instance.MainCameraController;
            if (!(bool) (UnityEngine.Object) cameraController)
                return;
            if (value)
            {
                cameraController.OverrideZoomScale = 0.75f;
                cameraController.LockToRoom = true;
                cameraController.AddFocusPoint(this.head.gameObject);
            }
            else
            {
                cameraController.OverrideZoomScale = 1f;
                cameraController.LockToRoom = false;
                cameraController.RemoveFocusPoint(this.head.gameObject);
            }
        }

        public void BlockPitTiles(bool value)
        {
            if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || (UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
                return;
            IntVector2 basePosition = this.aiActor.ParentRoom.area.basePosition;
            IntVector2 intVector2 = this.aiActor.ParentRoom.area.basePosition + this.aiActor.ParentRoom.area.dimensions - IntVector2.One;
            DungeonData data = GameManager.Instance.Dungeon.data;
            for (int x = basePosition.x; x <= intVector2.x; ++x)
            {
                for (int y = basePosition.y; y <= intVector2.y; ++y)
                {
                    CellData cellData = data[x, y];
                    if (cellData != null && cellData.type == CellType.PIT)
                        cellData.IsPlayerInaccessible = value;
                }
            }
        }

        public bool MaybeConvertToGold()
        {
            if (!this.HasConvertedBaby)
                return false;
            this.StartCoroutine(this.ConvertToGold());
            return true;
        }

        [DebuggerHidden]
        private IEnumerator ConvertToNearDeath()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunController__ConvertToNearDeathc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ConvertToGold()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunController__ConvertToGoldc__Iterator1()
            {
                _this = this
            };
        }

        public void HandleDarkRoomEffects(bool enabling, float duration)
        {
            this.StartCoroutine(this.HandleDarkRoomEffectsCR(enabling, duration));
        }

        [DebuggerHidden]
        private IEnumerator HandleDarkRoomEffectsCR(bool enabling, float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunController__HandleDarkRoomEffectsCRc__Iterator2()
            {
                duration = duration,
                enabling = enabling,
                _this = this
            };
        }
    }

