using System;

using UnityEngine;

using Dungeonator;

#nullable disable

public class SkyRocket : BraveBehaviour
    {
        public float AscentTime = 1f;
        public AnimationCurve AscentCurve;
        public float HangTime = 1f;
        public float DescentTime = 1f;
        public AnimationCurve DescentCurve;
        public float MaxHeight = 30f;
        public string DownSprite = "rocket_white_red_down_001";
        public float Variance = 0.25f;
        public float LeadPercentage = 0.66f;
        public GameObject LandingTargetSprite;
        public bool DoExplosion = true;
        public ExplosionData ExplosionData;
        public bool IgnoreExplosionQueues;
        public VFXPool SpawnVfx;
        public GameObject SpawnObject;
        private float MaxSpriteHeight = 10f;
        [NonSerialized]
        public SpeculativeRigidbody Target;
        [NonSerialized]
        public Vector2 TargetVector2;
        private Vector3 m_startPosition;
        private float m_startHeight;
        private Vector3 m_targetLandPosition;
        private float m_timer;
        private float m_totalDuration;
        private GameObject m_landingTarget;
        private SkyRocket.SkyRocketState m_state;

        public void Start()
        {
            this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
            this.m_timer = this.AscentTime;
            this.m_startPosition = this.transform.position;
            this.m_startHeight = this.sprite.HeightOffGround;
            this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
            this.spriteAnimator = this.GetComponentInChildren<tk2dSpriteAnimator>();
            if ((double) this.AscentTime != 0.0)
                return;
            this.transform.position = this.m_startPosition + new Vector3(0.0f, this.MaxHeight, 0.0f);
        }

        public void Update()
        {
            this.m_timer -= BraveTime.DeltaTime;
            if (this.m_state == SkyRocket.SkyRocketState.Ascend)
            {
                float num = this.AscentCurve.Evaluate(1f - Mathf.Clamp01(this.m_timer / this.AscentTime));
                this.transform.position = this.m_startPosition + new Vector3(0.0f, num * this.MaxHeight, 0.0f);
                this.sprite.HeightOffGround = this.m_startHeight + num * this.MaxSpriteHeight;
                if ((double) this.m_timer <= 0.0)
                {
                    this.m_timer = this.HangTime;
                    this.m_state = SkyRocket.SkyRocketState.Hang;
                    if ((bool) (UnityEngine.Object) this.sprite.attachParent)
                        this.sprite.attachParent.DetachRenderer(this.sprite);
                    if (this.TargetVector2 != Vector2.zero)
                    {
                        this.m_targetLandPosition = (Vector3) this.TargetVector2;
                    }
                    else
                    {
                        Vector2 vector2 = new Vector2(UnityEngine.Random.Range(-this.Variance, this.Variance), UnityEngine.Random.Range(-this.Variance, this.Variance));
                        bool flag = (double) UnityEngine.Random.value < (double) this.LeadPercentage;
                        this.m_targetLandPosition = (Vector3) (this.Target.UnitCenter + vector2);
                        if ((bool) (UnityEngine.Object) this.Target)
                        {
                            PlayerController gameActor = this.Target.gameActor as PlayerController;
                            if (flag && (bool) (UnityEngine.Object) gameActor)
                                this.m_targetLandPosition += (Vector3) (!(bool) (UnityEngine.Object) gameActor ? this.Target.Velocity : gameActor.AverageVelocity) * (this.HangTime + this.DescentTime);
                        }
                        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetRoomFromPosition(this.Target.UnitCenter.ToIntVector2(VectorConversions.Floor));
                        if (roomFromPosition != null)
                            this.m_targetLandPosition = (Vector3) Vector2Extensions.Clamp((Vector2) this.m_targetLandPosition, (roomFromPosition.area.basePosition + IntVector2.One).ToVector2(), (roomFromPosition.area.basePosition + roomFromPosition.area.dimensions - IntVector2.One).ToVector2());
                    }
                    this.m_landingTarget = SpawnManager.SpawnVFX(this.LandingTargetSprite, this.m_targetLandPosition, Quaternion.identity);
                    this.m_landingTarget.GetComponentInChildren<tk2dSprite>().UpdateZDepth();
                    tk2dSpriteAnimator componentInChildren = this.m_landingTarget.GetComponentInChildren<tk2dSpriteAnimator>();
                    componentInChildren.Play(componentInChildren.DefaultClip, 0.0f, (float) componentInChildren.DefaultClip.frames.Length / (this.HangTime + this.DescentTime));
                }
            }
            else if (this.m_state == SkyRocket.SkyRocketState.Hang)
            {
                this.transform.position = this.m_targetLandPosition + new Vector3(0.0f, this.MaxHeight, 0.0f);
                if ((double) this.m_timer <= 0.0)
                {
                    this.m_timer = this.DescentTime;
                    this.m_state = SkyRocket.SkyRocketState.Descend;
                    this.transform.localEulerAngles = this.transform.localEulerAngles + new Vector3(0.0f, 0.0f, 180f);
                    if (!string.IsNullOrEmpty(this.DownSprite))
                        this.sprite.SetSprite(this.DownSprite);
                }
            }
            else if (this.m_state == SkyRocket.SkyRocketState.Descend)
            {
                float num = 1f - this.DescentCurve.Evaluate(1f - Mathf.Clamp01(this.m_timer / this.DescentTime));
                this.transform.position = this.m_targetLandPosition + new Vector3(0.0f, this.MaxHeight - num * this.MaxHeight, 0.0f);
                this.sprite.HeightOffGround = this.m_startHeight + (this.MaxSpriteHeight - num * this.MaxSpriteHeight);
                if ((double) this.m_timer <= 0.0)
                {
                    this.transform.position = this.m_targetLandPosition;
                    if (this.DoExplosion)
                        Exploder.Explode(this.m_targetLandPosition, this.ExplosionData, Vector2.zero, ignoreQueues: this.IgnoreExplosionQueues);
                    this.SpawnVfx.SpawnAtPosition(this.transform.position);
                    if ((bool) (UnityEngine.Object) this.SpawnObject)
                        UnityEngine.Object.Instantiate<GameObject>(this.SpawnObject, this.transform.position, Quaternion.identity);
                    SpawnManager.Despawn(this.m_landingTarget);
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
                }
            }
            this.sprite.HeightOffGround = 2f;
            this.sprite.UpdateZDepth();
        }

        public void DieInAir()
        {
            if (this.m_state == SkyRocket.SkyRocketState.Descend && (double) this.m_timer < 0.5)
                return;
            if ((bool) (UnityEngine.Object) this.m_landingTarget)
                SpawnManager.Despawn(this.m_landingTarget);
            UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }

        protected override void OnDestroy() => base.OnDestroy();

        public enum SkyRocketState
        {
            Ascend,
            Hang,
            Descend,
        }
    }

