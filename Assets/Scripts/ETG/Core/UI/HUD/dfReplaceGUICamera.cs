using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/3D/Replace GUI Camera")]
public class dfReplaceGUICamera : MonoBehaviour
  {
    public Camera mainCamera;

    public void OnEnable()
    {
      if ((Object) this.mainCamera == (Object) null)
        this.mainCamera = Camera.main;
      dfGUIManager component = this.GetComponent<dfGUIManager>();
      if ((Object) component == (Object) null)
      {
        Debug.LogError((object) "This script should be attached to a dfGUIManager instance", (Object) this);
        this.enabled = false;
      }
      else
      {
        this.mainCamera.cullingMask |= 1 << this.gameObject.layer;
        component.OverrideCamera = true;
        component.RenderCamera = this.mainCamera;
      }
    }
  }

