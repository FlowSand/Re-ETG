using UnityEngine;

#nullable disable

public class DFScaleFixer : MonoBehaviour
    {
        private dfGUIManager m_manager;

        private void Start() => this.m_manager = this.GetComponent<dfGUIManager>();

        private void Update()
        {
            this.m_manager.UIScaleLegacyMode = false;
            this.m_manager.UIScale = (float) this.m_manager.RenderCamera.pixelHeight / (float) this.m_manager.FixedHeight;
        }
    }

