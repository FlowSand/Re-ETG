using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class HeatIndicatorController : MonoBehaviour
    {
        public float CurrentRadius = 5f;
        public Color CurrentColor = new Color(1f, 0.0f, 0.0f, 1f);
        public bool IsFire = true;
        private Material m_materialInst;
        private int m_radiusID;
        private int m_centerID;
        private int m_colorID;

        public void Awake()
        {
            this.m_radiusID = Shader.PropertyToID("_Radius");
            this.m_centerID = Shader.PropertyToID("_WorldCenter");
            this.m_colorID = Shader.PropertyToID("_RingColor");
            this.m_materialInst = this.GetComponent<MeshRenderer>().material;
        }

        public void Start()
        {
            this.StartCoroutine(this.LerpColor(this, new Color(0.0f, 0.0f, 0.0f, 0.0f), this.CurrentColor, 0.5f));
            if (this.IsFire)
                return;
            ParticleSystem componentInChildren = this.GetComponentInChildren<ParticleSystem>();
            if (!(bool) (Object) componentInChildren)
                return;
            componentInChildren.emission.enabled = false;
            componentInChildren.gameObject.SetActive(false);
        }

        public void LateUpdate()
        {
            this.transform.rotation = Quaternion.Euler(45f, 0.0f, 0.0f);
            this.transform.localScale = new Vector3(this.CurrentRadius * 2f, this.CurrentRadius * 2f * Mathf.Sqrt(2f), 1f);
            Vector3 position = this.transform.position;
            this.m_materialInst.SetVector(this.m_centerID, new Vector4(position.x, position.y, position.z, 0.0f));
            this.m_materialInst.SetFloat(this.m_radiusID, this.CurrentRadius);
            this.m_materialInst.SetColor(this.m_colorID, this.CurrentColor);
        }

        public void EndEffect()
        {
            this.StartCoroutine(this.LerpColor(this, this.CurrentColor, new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.5f));
            this.StartCoroutine(this.HandleDeathDelay());
        }

        [DebuggerHidden]
        private IEnumerator HandleDeathDelay()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HeatIndicatorController__HandleDeathDelayc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator LerpColor(
            HeatIndicatorController indicator,
            Color startColor,
            Color targetColor,
            float lerpTime)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new HeatIndicatorController__LerpColorc__Iterator1()
            {
                lerpTime = lerpTime,
                startColor = startColor,
                targetColor = targetColor,
                indicator = indicator
            };
        }
    }

