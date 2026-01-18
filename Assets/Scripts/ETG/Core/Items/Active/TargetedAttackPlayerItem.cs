using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class TargetedAttackPlayerItem : PlayerItem
    {
        public float minDistance = 1f;
        public float maxDistance = 15f;
        public GameObject reticleQuad;
        public bool doesGoop;
        public GoopDefinition goopDefinition;
        public float goopRadius = 3f;
        public bool doesStrike = true;
        public GameObject strikeVFX;
        public ExplosionData strikeExplosionData;
        public bool DoScreenFlash = true;
        public float FlashHoldtime = 0.1f;
        public float FlashFadetime = 0.5f;
        public bool TransmogrifySurvivors;
        public float TransmogrifyRadius = 15f;
        public float TransmogrifyChance = 0.5f;
        [EnemyIdentifier]
        public string TransmogrifyTargetGuid;
        private PlayerController m_currentUser;
        private tk2dBaseSprite m_extantReticleQuad;
        private float m_currentAngle;
        private float m_currentDistance = 5f;

        public override void Update()
        {
            base.Update();
            if (!this.IsCurrentlyActive)
                return;
            if ((bool) (Object) this.m_extantReticleQuad)
            {
                this.UpdateReticlePosition();
            }
            else
            {
                this.IsCurrentlyActive = false;
                this.ClearCooldowns();
            }
        }

        private void UpdateReticlePosition()
        {
            if (BraveInput.GetInstanceForPlayer(this.m_currentUser.PlayerIDX).IsKeyboardAndMouse())
            {
                this.m_extantReticleQuad.transform.position = (Vector3) (this.m_currentUser.unadjustedAimPoint.XY() - this.m_extantReticleQuad.GetBounds().extents.XY());
            }
            else
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_currentUser.PlayerIDX);
                Vector2 a = this.m_currentUser.CenterPosition + (Quaternion.Euler(0.0f, 0.0f, this.m_currentAngle) * (Vector3) Vector2.right).XY() * this.m_currentDistance + instanceForPlayer.ActiveActions.Aim.Vector * 8f * BraveTime.DeltaTime;
                this.m_currentAngle = BraveMathCollege.Atan2Degrees(a - this.m_currentUser.CenterPosition);
                this.m_currentDistance = Vector2.Distance(a, this.m_currentUser.CenterPosition);
                this.m_currentDistance = Mathf.Min(this.m_currentDistance, this.maxDistance);
                this.m_extantReticleQuad.transform.position = (Vector3) (this.m_currentUser.CenterPosition + (Quaternion.Euler(0.0f, 0.0f, this.m_currentAngle) * (Vector3) Vector2.right).XY() * this.m_currentDistance - this.m_extantReticleQuad.GetBounds().extents.XY());
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if (!(bool) (Object) this.m_extantReticleQuad)
                return;
            Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
        }

        protected override void DoEffect(PlayerController user)
        {
            this.IsCurrentlyActive = true;
            this.m_currentUser = user;
            this.m_extantReticleQuad = Object.Instantiate<GameObject>(this.reticleQuad).GetComponent<tk2dBaseSprite>();
            this.m_currentAngle = BraveMathCollege.Atan2Degrees(this.m_currentUser.unadjustedAimPoint.XY() - this.m_currentUser.CenterPosition);
            this.m_currentDistance = 5f;
            this.UpdateReticlePosition();
        }

        protected override void DoActiveEffect(PlayerController user)
        {
            Vector2 worldCenter = this.m_extantReticleQuad.WorldCenter;
            if ((bool) (Object) this.m_extantReticleQuad)
                Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
            this.IsCurrentlyActive = true;
            if (this.doesStrike)
                this.DoStrike(worldCenter);
            if (this.doesGoop)
                this.HandleEngoopening(worldCenter, this.goopRadius);
            if (this.itemName == "Nuke" && (bool) (Object) user && user.HasActiveBonusSynergy(CustomSynergyType.MELTDOWN))
            {
                user.CurrentGun.GainAmmo(100);
                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", this.gameObject);
            }
            if (this.TransmogrifySurvivors && (bool) (Object) user && user.CurrentRoom != null)
            {
                List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null)
                {
                    int count = activeEnemies.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        if ((bool) (Object) activeEnemies[index] && activeEnemies[index].HasBeenEngaged && (bool) (Object) activeEnemies[index].healthHaver && activeEnemies[index].IsNormalEnemy && !activeEnemies[index].healthHaver.IsDead && !activeEnemies[index].healthHaver.IsBoss && !activeEnemies[index].IsTransmogrified && (double) Random.value < (double) this.TransmogrifyChance && (double) Vector2.Distance(activeEnemies[index].CenterPosition, worldCenter) < (double) this.TransmogrifyRadius)
                            activeEnemies[index].Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogrifyTargetGuid), (GameObject) null);
                    }
                }
            }
            if (this.DoScreenFlash)
            {
                Pixelator.Instance.FadeToColor(this.FlashFadetime, Color.white, true, this.FlashHoldtime);
                StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false);
            }
            this.IsCurrentlyActive = false;
        }

        protected void HandleEngoopening(Vector2 startPoint, float radius)
        {
            float duration = 1f;
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition).TimedAddGoopCircle(startPoint, radius, duration);
        }

        private void DoStrike(Vector2 currentTarget)
        {
            Exploder.Explode((Vector3) currentTarget, this.strikeExplosionData, Vector2.zero);
        }

        protected override void OnDestroy()
        {
            if ((bool) (Object) this.m_extantReticleQuad)
                Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
            base.OnDestroy();
        }
    }

