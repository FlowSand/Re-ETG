using UnityEngine;

#nullable disable

public class MotionTriggeredStatSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public StatModifier Stat;
        public float TimeRequiredMoving = 2f;
        private Gun m_gun;
        private bool m_isActive;
        private PlayerController m_cachedPlayer;
        private float m_elapsedMoving;

        private void Awake() => this.m_gun = this.GetComponent<Gun>();

        private void Update()
        {
            if ((bool) (Object) this.m_gun.CurrentOwner)
            {
                this.m_cachedPlayer = this.m_gun.CurrentOwner as PlayerController;
                if ((double) this.m_cachedPlayer.specRigidbody.Velocity.magnitude > 0.05000000074505806)
                    this.m_elapsedMoving += BraveTime.DeltaTime;
                else
                    this.m_elapsedMoving = 0.0f;
            }
            else
                this.m_elapsedMoving = 0.0f;
            bool flag = (bool) (Object) this.m_cachedPlayer && this.m_cachedPlayer.HasActiveBonusSynergy(this.RequiredSynergy);
            if (flag && (double) this.m_elapsedMoving > (double) this.TimeRequiredMoving && !this.m_isActive)
            {
                this.m_isActive = true;
                this.m_cachedPlayer.ownerlessStatModifiers.Add(this.Stat);
                this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer);
                this.EnableVFX(this.m_cachedPlayer);
            }
            else
            {
                if (!this.m_isActive || flag && (double) this.m_elapsedMoving >= (double) this.TimeRequiredMoving)
                    return;
                this.m_isActive = false;
                if (!(bool) (Object) this.m_cachedPlayer)
                    return;
                this.DisableVFX(this.m_cachedPlayer);
                this.m_cachedPlayer.ownerlessStatModifiers.Remove(this.Stat);
                this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer);
                this.m_cachedPlayer = (PlayerController) null;
            }
        }

        private void OnDisable()
        {
            if (!this.m_isActive)
                return;
            this.m_isActive = false;
            if (!(bool) (Object) this.m_cachedPlayer)
                return;
            this.DisableVFX(this.m_cachedPlayer);
            this.m_cachedPlayer.ownerlessStatModifiers.Remove(this.Stat);
            this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer);
            this.m_cachedPlayer = (PlayerController) null;
        }

        private void OnDestroy()
        {
            if (!this.m_isActive)
                return;
            this.m_isActive = false;
            if (!(bool) (Object) this.m_cachedPlayer)
                return;
            this.DisableVFX(this.m_cachedPlayer);
            this.m_cachedPlayer.ownerlessStatModifiers.Remove(this.Stat);
            this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer);
            this.m_cachedPlayer = (PlayerController) null;
        }

        public void EnableVFX(PlayerController target)
        {
            Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
            if (!((Object) outlineMaterial != (Object) null))
                return;
            outlineMaterial.SetColor("_OverrideColor", new Color(99f, 99f, 0.0f));
        }

        public void DisableVFX(PlayerController target)
        {
            Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
            if (!((Object) outlineMaterial != (Object) null))
                return;
            outlineMaterial.SetColor("_OverrideColor", new Color(0.0f, 0.0f, 0.0f));
        }
    }

