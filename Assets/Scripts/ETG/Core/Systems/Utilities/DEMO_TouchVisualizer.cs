using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Debugging/Touch Visualizer")]
public class DEMO_TouchVisualizer : MonoBehaviour
    {
        public bool editorOnly;
        public bool showMouse;
        public bool showPlatformInfo;
        public int iconSize = 32;
        public Texture2D touchIcon;
        private IDFTouchInputSource input;

        private void Awake() => this.useGUILayout = false;

        public void OnGUI()
        {
            if (this.editorOnly && !Application.isEditor)
                return;
            if (this.input == null)
            {
                dfInputManager component = this.GetComponent<dfInputManager>();
                if ((Object) component == (Object) null)
                {
                    Debug.LogError((object) "No dfInputManager instance found", (Object) this);
                    this.enabled = false;
                    return;
                }
                if (component.UseTouch)
                {
                    this.input = component.TouchInputSource;
                    if (this.input == null)
                    {
                        Debug.LogError((object) "No dfTouchInputSource component found", (Object) this);
                        this.enabled = false;
                        return;
                    }
                }
                else
                {
                    if (!Application.isPlaying)
                        return;
                    this.enabled = false;
                    return;
                }
            }
            if (this.showPlatformInfo)
                GUI.Label(new Rect(5f, 0.0f, 800f, 25f), $"Touch Source: {(object) this.input}, Platform: {(object) Application.platform}");
            if (this.showMouse && !Application.isEditor)
                this.drawTouchIcon(Input.mousePosition);
            int touchCount = this.input.TouchCount;
            for (int index = 0; index < touchCount; ++index)
                this.drawTouchIcon((Vector3) this.input.GetTouch(index).position);
        }

        private void drawTouchIcon(Vector3 pos)
        {
            int height = Screen.height;
            pos.y = (float) height - pos.y;
            GUI.DrawTexture(new Rect(pos.x - (float) (this.iconSize / 2), pos.y - (float) (this.iconSize / 2), (float) this.iconSize, (float) this.iconSize), (Texture) this.touchIcon);
        }
    }

