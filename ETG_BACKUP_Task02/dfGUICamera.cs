// Decompiled with JetBrains decompiler
// Type: dfGUICamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Daikon Forge/User Interface/GUI Camera")]
[Serializable]
public class dfGUICamera : MonoBehaviour
{
  public Vector3 cameraPositionOffset;
  public bool MaintainCameraAspect = true;
  public bool ForceToSixteenNine;
  public bool ForceNoHalfPixelOffset;
  private Camera m_camera;

  public void Awake()
  {
    if (!Application.isPlaying || !this.MaintainCameraAspect)
      return;
    if (this.ForceToSixteenNine)
      BraveCameraUtility.MaintainCameraAspectForceAspect(this.GetComponent<Camera>(), 1.77777779f);
    else
      BraveCameraUtility.MaintainCameraAspect(this.GetComponent<Camera>());
    this.transform.parent.GetComponent<dfGUIManager>().ResolutionChanged();
  }

  public void OnEnable()
  {
  }

  public void Start()
  {
    this.m_camera = this.GetComponent<Camera>();
    this.m_camera.transparencySortMode = TransparencySortMode.Orthographic;
    this.m_camera.useOcclusionCulling = false;
    this.m_camera.eventMask &= ~this.GetComponent<Camera>().cullingMask;
  }

  private void Update()
  {
    if (!Application.isPlaying || !this.MaintainCameraAspect)
      return;
    if (this.ForceToSixteenNine)
      BraveCameraUtility.MaintainCameraAspectForceAspect(this.m_camera, 1.77777779f);
    else
      BraveCameraUtility.MaintainCameraAspect(this.m_camera);
  }
}
