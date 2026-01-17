// Decompiled with JetBrains decompiler
// Type: dfReplaceGUICamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
