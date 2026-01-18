using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class LabAmbientLightController : MonoBehaviour
    {
        public Gradient colorGradient;
        public float period = 5f;
        public Transform[] HallwayLights;
        public float HallwayXTranslation = 10f;
        public float HallwayPeriod = 3f;
        private ShadowSystem[] HallwayLightManagers;
        private float[] m_lightIntensities;
        private Vector3[] m_lightStarts;
        private int m_colorID;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new LabAmbientLightController__Startc__Iterator0()
            {
                _this = this
            };
        }

        private void Update()
        {
            if (this.m_lightStarts == null)
                return;
            GameManager.Instance.Dungeon.OverrideAmbientLight = true;
            GameManager.Instance.Dungeon.OverrideAmbientColor = this.colorGradient.Evaluate(UnityEngine.Time.timeSinceLevelLoad % this.period / this.period);
            float t = UnityEngine.Time.timeSinceLevelLoad % this.HallwayPeriod / this.HallwayPeriod;
            float num = Mathf.PingPong(t, 0.5f) * 2f;
            for (int index = 0; index < this.HallwayLights.Length; ++index)
            {
                this.HallwayLightManagers[index].uLightIntensity = this.m_lightIntensities[index] * num;
                Material sharedMaterial = this.HallwayLightManagers[index].renderer.sharedMaterial;
                sharedMaterial.SetColor(this.m_colorID, sharedMaterial.GetColor(this.m_colorID).WithAlpha(num));
                this.HallwayLights[index].position = this.m_lightStarts[index] + new Vector3(this.HallwayXTranslation * t, 0.0f, 0.0f);
            }
            PlatformInterface.SetAlienFXAmbientColor((Color32) new Color(1f, 0.0f, 0.0f, num));
        }
    }

