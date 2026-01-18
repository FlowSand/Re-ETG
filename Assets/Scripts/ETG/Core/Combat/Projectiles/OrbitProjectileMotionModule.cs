using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class OrbitProjectileMotionModule : ProjectileAndBeamMotionModule
    {
        private static Dictionary<int, List<OrbitProjectileMotionModule>> m_currentOrbiters = new Dictionary<int, List<OrbitProjectileMotionModule>>();
        public float MinRadius = 2f;
        public float MaxRadius = 5f;
        [NonSerialized]
        public float CustomSpawnVFXElapsed = -1f;
        [NonSerialized]
        public bool HasSpawnVFX;
        [NonSerialized]
        public GameObject SpawnVFX;
        public bool ForceInvert;
        private float m_radius;
        private float m_currentAngle;
        private bool m_initialized;
        private Vector2 m_initialRightVector;
        private Vector2 m_initialUpVector;
        [NonSerialized]
        private bool m_isOrbiting = true;
        [NonSerialized]
        public int OrbitGroup = -1;
        [NonSerialized]
        private bool m_hasDoneSpawnVFX;
        [NonSerialized]
        public float lifespan = -1f;
        [NonSerialized]
        public bool usesAlternateOrbitTarget;
        [NonSerialized]
        public SpeculativeRigidbody alternateOrbitTarget;
        private float m_beamOrbitRadius = 2.75f;
        private float m_beamOrbitRadiusCircumference = 17.27876f;
        private bool m_spawnVFXActive;
        private GameObject m_activeSpawnVFX;
        private float m_spawnVFXElapsed;
        public bool m_isBeam;
        public static int ActiveBeams = 0;
        public bool StackHelix;

        public static int GetOrbitersInGroup(int group)
        {
            return OrbitProjectileMotionModule.m_currentOrbiters.ContainsKey(group) && OrbitProjectileMotionModule.m_currentOrbiters[group] != null ? OrbitProjectileMotionModule.m_currentOrbiters[group].Count : 0;
        }

        public float BeamOrbitRadius
        {
            get => this.m_beamOrbitRadius;
            set
            {
                this.m_beamOrbitRadius = value;
                this.m_beamOrbitRadiusCircumference = 6.28318548f * this.m_beamOrbitRadius;
            }
        }

        public override void UpdateDataOnBounce(float angleDiff)
        {
            if (float.IsNaN(angleDiff))
                return;
            this.m_initialUpVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialUpVector);
            this.m_initialRightVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialRightVector);
        }

        public override void AdjustRightVector(float angleDiff)
        {
            if (float.IsNaN(angleDiff))
                return;
            this.m_initialUpVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialUpVector);
            this.m_initialRightVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialRightVector);
        }

        private List<OrbitProjectileMotionModule> RegisterSelfWithDictionary()
        {
            if (!OrbitProjectileMotionModule.m_currentOrbiters.ContainsKey(this.OrbitGroup))
                OrbitProjectileMotionModule.m_currentOrbiters.Add(this.OrbitGroup, new List<OrbitProjectileMotionModule>());
            List<OrbitProjectileMotionModule> currentOrbiter = OrbitProjectileMotionModule.m_currentOrbiters[this.OrbitGroup];
            if (!currentOrbiter.Contains(this))
                currentOrbiter.Add(this);
            return currentOrbiter;
        }

        private void DeregisterSelfWithDictionary()
        {
            if (!OrbitProjectileMotionModule.m_currentOrbiters.ContainsKey(this.OrbitGroup))
                return;
            OrbitProjectileMotionModule.m_currentOrbiters[this.OrbitGroup].Remove(this);
        }

        public override void Move(
            Projectile source,
            Transform projectileTransform,
            tk2dBaseSprite projectileSprite,
            SpeculativeRigidbody specRigidbody,
            ref float m_timeElapsed,
            ref Vector2 m_currentDirection,
            bool Inverted,
            bool shouldRotate)
        {
            if (this.m_isOrbiting && (!(bool) (UnityEngine.Object) source || !this.usesAlternateOrbitTarget && !(bool) (UnityEngine.Object) source.Owner || this.usesAlternateOrbitTarget && !(bool) (UnityEngine.Object) this.alternateOrbitTarget))
                this.m_isOrbiting = false;
            if (this.m_isOrbiting && (double) this.lifespan > 0.0 && (double) m_timeElapsed > (double) this.lifespan)
            {
                this.m_isOrbiting = false;
                this.DeregisterSelfWithDictionary();
            }
            if (!this.m_isOrbiting)
                return;
            Vector2 position = !(bool) (UnityEngine.Object) projectileSprite ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
            if (this.HasSpawnVFX && !this.m_hasDoneSpawnVFX)
            {
                this.m_hasDoneSpawnVFX = true;
                this.m_spawnVFXActive = true;
                this.m_activeSpawnVFX = SpawnManager.SpawnVFX(this.SpawnVFX, (Vector3) position, Quaternion.identity);
                source.sprite.renderer.enabled = false;
            }
            if (this.m_hasDoneSpawnVFX)
                this.m_spawnVFXElapsed += BraveTime.DeltaTime;
            if (this.m_spawnVFXActive && (!(bool) (UnityEngine.Object) this.m_activeSpawnVFX || !this.m_activeSpawnVFX.activeSelf || (double) this.CustomSpawnVFXElapsed > 0.0 && (double) this.m_spawnVFXElapsed > (double) this.CustomSpawnVFXElapsed))
            {
                this.m_activeSpawnVFX = (GameObject) null;
                this.m_spawnVFXActive = false;
                source.sprite.renderer.enabled = true;
            }
            if (!this.m_initialized)
            {
                this.m_initialized = true;
                this.m_initialRightVector = !shouldRotate ? m_currentDirection : projectileTransform.right.XY();
                this.m_initialUpVector = (Vector2) (!shouldRotate ? Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) m_currentDirection : projectileTransform.up);
                this.m_radius = UnityEngine.Random.Range(this.MinRadius, this.MaxRadius);
                this.m_currentAngle = this.m_initialRightVector.ToAngle();
                source.OnDestruction += new Action<Projectile>(this.OnDestroyed);
            }
            this.RegisterSelfWithDictionary();
            m_timeElapsed += BraveTime.DeltaTime;
            float radius = this.m_radius;
            this.m_currentAngle += (float) ((double) (source.Speed * BraveTime.DeltaTime) / (6.2831854820251465 * (double) radius) * 360.0);
            Vector2 zero = Vector2.zero;
            Vector2 vector2_1 = !this.usesAlternateOrbitTarget ? source.Owner.CenterPosition : this.alternateOrbitTarget.UnitCenter;
            Vector2 vector = vector2_1 + (Quaternion.Euler(0.0f, 0.0f, this.m_currentAngle) * (Vector3) Vector2.right * radius).XY();
            if (this.StackHelix)
            {
                float num1 = 2f;
                float num2 = 1f;
                float num3 = (!this.ForceInvert ? 1f : -1f) * num2 * Mathf.Sin(source.GetElapsedDistance() / num1);
                vector += (vector - vector2_1).normalized * num3;
            }
            Vector2 vector2_2 = (vector - position) / BraveTime.DeltaTime;
            m_currentDirection = vector2_2.normalized;
            if (shouldRotate)
            {
                float num = m_currentDirection.ToAngle();
                if (float.IsNaN(num) || float.IsInfinity(num))
                    num = 0.0f;
                projectileTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, num);
            }
            specRigidbody.Velocity = vector2_2;
            if (float.IsNaN(specRigidbody.Velocity.magnitude) || Mathf.Approximately(specRigidbody.Velocity.magnitude, 0.0f))
                source.DieInAir();
            if (!(bool) (UnityEngine.Object) this.m_activeSpawnVFX)
                return;
            this.m_activeSpawnVFX.transform.position = vector.ToVector3ZisY();
        }

        public void BeamDestroyed() => this.OnDestroyed((Projectile) null);

        private void OnDestroyed(Projectile obj)
        {
            this.DeregisterSelfWithDictionary();
            if (!this.m_isBeam)
                return;
            this.m_isBeam = false;
            --OrbitProjectileMotionModule.ActiveBeams;
        }

        public override void SentInDirection(
            ProjectileData baseData,
            Transform projectileTransform,
            tk2dBaseSprite projectileSprite,
            SpeculativeRigidbody specRigidbody,
            ref float m_timeElapsed,
            ref Vector2 m_currentDirection,
            bool shouldRotate,
            Vector2 dirVec,
            bool resetDistance,
            bool updateRotation)
        {
        }

        public void RegisterAsBeam(BeamController beam)
        {
            if (this.m_isBeam)
                return;
            BasicBeamController basicBeamController = beam as BasicBeamController;
            if ((bool) (UnityEngine.Object) basicBeamController && !basicBeamController.IsReflectedBeam)
                basicBeamController.IgnoreTilesDistance = this.m_beamOrbitRadiusCircumference;
            this.m_isBeam = true;
            ++OrbitProjectileMotionModule.ActiveBeams;
        }

        public override Vector2 GetBoneOffset(
            BasicBeamController.BeamBone bone,
            BeamController sourceBeam,
            bool inverted)
        {
            if (sourceBeam.IsReflectedBeam)
                return Vector2.zero;
            PlayerController owner = sourceBeam.Owner as PlayerController;
            Vector2 vector = owner.unadjustedAimPoint.XY() - owner.CenterPosition;
            float angle = vector.ToAngle();
            Vector2 vector2 = bone.Position - owner.CenterPosition;
            Vector2 boneOffset;
            if ((double) bone.PosX < (double) this.m_beamOrbitRadiusCircumference)
            {
                float num = (float) ((double) bone.PosX / (double) this.m_beamOrbitRadiusCircumference * 360.0) + angle;
                float x = Mathf.Cos((float) Math.PI / 180f * num) * this.BeamOrbitRadius;
                float y = Mathf.Sin((float) Math.PI / 180f * num) * this.BeamOrbitRadius;
                bone.RotationAngle = num + 90f;
                boneOffset = new Vector2(x, y) - vector2;
            }
            else
            {
                bone.RotationAngle = angle;
                boneOffset = vector.normalized * (bone.PosX - this.m_beamOrbitRadiusCircumference + this.m_beamOrbitRadius) - vector2;
            }
            if (this.StackHelix)
            {
                float num1 = 3f;
                float num2 = 1f;
                float num3 = 6f;
                int num4 = !(inverted ^ this.ForceInvert) ? 1 : -1;
                float num5 = bone.PosX - num3 * (UnityEngine.Time.timeSinceLevelLoad % 600000f);
                float to = (float) num4 * num2 * Mathf.Sin(num5 * 3.14159274f / num1);
                boneOffset += BraveMathCollege.DegreesToVector(bone.RotationAngle + 90f, Mathf.SmoothStep(0.0f, to, bone.PosX));
            }
            return boneOffset;
        }
    }

