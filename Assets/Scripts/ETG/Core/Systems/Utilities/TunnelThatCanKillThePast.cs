using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Kvant;
using Reaktion;

#nullable disable

public class TunnelThatCanKillThePast : MonoBehaviour
    {
        public Tunnel targetTunnel;
        public Material targetMaterial;
        public MeshRenderer ShellRenderer;
        public float MinPulse = -1f;
        public float MaxPulse = 1f;
        public float PulsePeriod = 0.5f;
        public float ShatterTime = 1f;
        public AnimationCurve ShatterCurve;
        public tk2dSprite BulletSprite;
        [NonSerialized]
        public bool shattering;
        private TunnelThatCanKillThePast.CurrentTunnelPhase m_currentPhase;
        private float m_timeSincePhaseTransition;
        private int m_displacementID = -1;
        private float m_standardDisplacement;
        public float BulletX = 1.3f;
        public float BulletY = -1.5f;
        public float BulletZ = 10f;

        private void Awake()
        {
            if (!(bool) (UnityEngine.Object) this.BulletSprite)
                return;
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                this.BulletSprite.ignoresTiltworldDepth = true;
                JitterMotion componentInChildren = this.GetComponentInChildren<JitterMotion>();
                if (!(bool) (UnityEngine.Object) componentInChildren)
                    return;
                componentInChildren.InfluenceAxialX = 10f;
                componentInChildren.InfluenceAxialY = 10f;
            }
            else
            {
                this.BulletSprite.gameObject.SetActive(false);
                this.BulletSprite.renderer.enabled = false;
                this.BulletSprite = (tk2dSprite) null;
            }
        }

        private void Start()
        {
            this.m_displacementID = Shader.PropertyToID("_Displacement");
            this.ShellRenderer.material.SetFloat("_Gain", 0.1f);
            this.ShellRenderer.material.SetFloat("_Brightness", 0.005f);
            if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM || GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW)
                this.ShellRenderer.enabled = false;
            else
                this.ShellRenderer.enabled = true;
            this.StartCoroutine(this.HandleBulletPosition());
        }

        public void ChangeToPulsed()
        {
            this.m_timeSincePhaseTransition = 0.0f;
            this.m_currentPhase = TunnelThatCanKillThePast.CurrentTunnelPhase.PULSED;
        }

        public void ManuallySetShatterAmount(float amount) => this.m_standardDisplacement = amount;

        public void Shatter()
        {
            foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
                BraveUtility.EnableEmission(componentsInChild, false);
            this.m_timeSincePhaseTransition = 0.0f;
            this.m_currentPhase = TunnelThatCanKillThePast.CurrentTunnelPhase.SHATTER;
        }

        private void Update()
        {
            this.m_timeSincePhaseTransition += GameManager.INVARIANT_DELTA_TIME;
            switch (this.m_currentPhase)
            {
                case TunnelThatCanKillThePast.CurrentTunnelPhase.STANDARD:
                    this.UpdateStandard();
                    break;
                case TunnelThatCanKillThePast.CurrentTunnelPhase.PULSED:
                    this.UpdatePulsed();
                    break;
                case TunnelThatCanKillThePast.CurrentTunnelPhase.SHATTER:
                    this.UpdateShatter();
                    break;
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleBulletPosition()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TunnelThatCanKillThePast__HandleBulletPositionc__Iterator0()
            {
                _this = this
            };
        }

        private void UpdateStandard()
        {
            this.targetMaterial.SetFloat(this.m_displacementID, this.m_standardDisplacement);
        }

        private void UpdatePulsed()
        {
            this.targetMaterial.SetFloat(this.m_displacementID, Mathf.Lerp(this.MinPulse, this.MaxPulse, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.m_timeSincePhaseTransition, this.PulsePeriod) / this.PulsePeriod)));
        }

        private void UpdateShatter()
        {
            this.targetMaterial.SetFloat(this.m_displacementID, Mathf.Lerp(this.m_standardDisplacement, -100f, this.ShatterCurve.Evaluate(this.m_timeSincePhaseTransition / this.ShatterTime)));
            float num1 = Mathf.Lerp(0.005f, 0.0f, this.m_timeSincePhaseTransition / this.ShatterTime);
            float num2 = Mathf.Lerp(0.1f, 0.0f, this.m_timeSincePhaseTransition / this.ShatterTime);
            this.ShellRenderer.material.SetFloat("_Brightness", num1);
            this.ShellRenderer.material.SetFloat("_Gain", num2);
        }

        public enum CurrentTunnelPhase
        {
            STANDARD,
            PULSED,
            SHATTER,
        }
    }

