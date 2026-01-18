using UnityEngine;

#nullable disable

public class EmissionSettings : MonoBehaviour
    {
        public float EmissivePower;
        public float EmissiveColorPower = 7f;
        private static bool indicesInitialized;
        private static int powerIndex;
        private static int colorPowerIndex;

        private void Start()
        {
            if (!EmissionSettings.indicesInitialized)
            {
                EmissionSettings.indicesInitialized = true;
                EmissionSettings.powerIndex = Shader.PropertyToID("_EmissivePower");
                EmissionSettings.colorPowerIndex = Shader.PropertyToID("_EmissiveColorPower");
            }
            tk2dBaseSprite component = this.GetComponent<tk2dBaseSprite>();
            if ((Object) component != (Object) null)
                component.usesOverrideMaterial = true;
            this.GetComponent<Renderer>().material.SetFloat(EmissionSettings.powerIndex, this.EmissivePower);
            this.GetComponent<Renderer>().material.SetFloat(EmissionSettings.colorPowerIndex, this.EmissiveColorPower);
        }
    }

