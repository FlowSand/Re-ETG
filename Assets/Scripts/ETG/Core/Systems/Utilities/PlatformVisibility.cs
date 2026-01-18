using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Platform-based Visibility")]
public class PlatformVisibility : MonoBehaviour
    {
        public bool HideOnWeb;
        public bool HideOnMobile;
        public bool HideInEditor;

        private void Start()
        {
            dfControl component = this.GetComponent<dfControl>();
            if ((Object) component == (Object) null || !this.HideInEditor || !Application.isEditor)
                return;
            component.Hide();
        }
    }

