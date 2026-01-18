using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Pixel-Perfect Platform Settings")]
public class RuntimePixelPerfect : MonoBehaviour
  {
    public bool PixelPerfectInEditor;
    public bool PixelPerfectAtRuntime = true;

    private void Awake()
    {
      dfGUIManager component = this.GetComponent<dfGUIManager>();
      if ((Object) component == (Object) null)
        throw new MissingComponentException("dfGUIManager instance not found");
      if (Application.isEditor)
        component.PixelPerfectMode = this.PixelPerfectInEditor;
      else
        component.PixelPerfectMode = this.PixelPerfectAtRuntime;
    }
  }

