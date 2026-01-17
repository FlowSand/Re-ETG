// Decompiled with JetBrains decompiler
// Type: tk2dUICamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/Core/tk2dUICamera")]
public class tk2dUICamera : MonoBehaviour
{
  [SerializeField]
  private LayerMask raycastLayerMask = (LayerMask) -1;

  public void AssignRaycastLayerMask(LayerMask mask) => this.raycastLayerMask = mask;

  public LayerMask FilteredMask
  {
    get => (LayerMask) ((int) this.raycastLayerMask & this.GetComponent<Camera>().cullingMask);
  }

  public Camera HostCamera => this.GetComponent<Camera>();

  private void OnEnable()
  {
    if ((Object) this.GetComponent<Camera>() == (Object) null)
    {
      Debug.LogError((object) "tk2dUICamera should only be attached to a camera.");
      this.enabled = false;
    }
    else
      tk2dUIManager.RegisterCamera(this);
  }

  private void OnDisable() => tk2dUIManager.UnregisterCamera(this);
}
