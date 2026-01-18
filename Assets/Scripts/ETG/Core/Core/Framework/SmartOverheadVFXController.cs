using UnityEngine;

#nullable disable

public class SmartOverheadVFXController : BraveBehaviour
    {
        public Vector2 offset;
        private PlayerController m_attachedPlayer;
        private Vector3 m_originalOffset;
        private bool m_playerInitialized;

        public void Initialize(PlayerController attachTarget, Vector3 offset)
        {
            this.m_playerInitialized = true;
            this.m_attachedPlayer = attachTarget;
            this.m_originalOffset = this.transform.localPosition.Quantize(1f / 16f, VectorConversions.Floor);
        }

        public void OnDespawned()
        {
            this.m_playerInitialized = false;
            this.m_attachedPlayer = (PlayerController) null;
            this.m_originalOffset = Vector3.zero;
        }

        private void Update()
        {
            if (this.m_playerInitialized)
            {
                if (this.m_attachedPlayer.healthHaver.IsDead)
                    SpawnManager.Despawn(this.gameObject);
                Vector3 originalOffset = this.m_originalOffset;
                if (GameUIRoot.Instance.GetReloadBarForPlayer(this.m_attachedPlayer).AnyStatusBarVisible())
                    originalOffset += new Vector3(0.0f, 1.25f, 0.0f);
                this.transform.localPosition = originalOffset;
            }
            if (!(this.offset != Vector2.zero))
                return;
            this.transform.localPosition += (Vector3) this.offset;
        }
    }

