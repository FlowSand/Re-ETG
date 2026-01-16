// Decompiled with JetBrains decompiler
// Type: dfFollowObject3D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/Examples/3D/Follow Object (3D)")]
public class dfFollowObject3D : MonoBehaviour
{
  public Transform attachedTo;
  public bool liveUpdate;
  [HideInInspector]
  [SerializeField]
  protected Vector3 designTimePosition;
  private dfControl control;
  private bool lastLiveUpdateValue;

  public void OnEnable()
  {
    this.control = this.GetComponent<dfControl>();
    this.Update();
  }

  public void Update()
  {
    if (this.lastLiveUpdateValue != this.liveUpdate)
    {
      this.lastLiveUpdateValue = this.liveUpdate;
      if (!this.liveUpdate)
      {
        this.control.RelativePosition = this.designTimePosition;
        this.control.transform.localScale = Vector3.one;
        this.control.transform.localRotation = Quaternion.identity;
      }
      else
        this.designTimePosition = this.control.RelativePosition;
      this.control.Invalidate();
    }
    if (!this.liveUpdate && !Application.isPlaying)
      return;
    this.updatePosition3D();
  }

  public void OnDrawGizmos()
  {
    if ((Object) this.control == (Object) null)
      this.control = this.GetComponent<dfControl>();
    Vector3 size = (Vector3) this.control.Size * this.control.PixelsToUnits();
    Gizmos.matrix = Matrix4x4.TRS(this.attachedTo.position, this.attachedTo.rotation, this.attachedTo.localScale);
    Gizmos.color = Color.clear;
    Gizmos.DrawCube(Vector3.zero, size);
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(Vector3.zero, size);
  }

  public void OnDrawGizmosSelected() => this.OnDrawGizmos();

  private void updatePosition3D()
  {
    if ((Object) this.attachedTo == (Object) null)
      return;
    this.control.transform.position = this.attachedTo.position;
    this.control.transform.rotation = this.attachedTo.rotation;
    this.control.transform.localScale = this.attachedTo.localScale;
  }
}
