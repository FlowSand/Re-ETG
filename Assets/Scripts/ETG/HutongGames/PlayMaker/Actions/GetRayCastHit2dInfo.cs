// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetRayCastHit2dInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[HutongGames.PlayMaker.Tooltip("Gets info on the last 2d Raycast or LineCast and store in variables.")]
public class GetRayCastHit2dInfo : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
  [UIHint(UIHint.Variable)]
  public FsmGameObject gameObjectHit;
  [Title("Hit Point")]
  [HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable.")]
  [UIHint(UIHint.Variable)]
  public FsmVector2 point;
  [HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable.")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 normal;
  [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
  [UIHint(UIHint.Variable)]
  public FsmFloat distance;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObjectHit = (FsmGameObject) null;
    this.point = (FsmVector2) null;
    this.normal = (FsmVector3) null;
    this.distance = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.StoreRaycastInfo();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.StoreRaycastInfo();

  private void StoreRaycastInfo()
  {
    RaycastHit2D raycastHit2Dinfo = Fsm.GetLastRaycastHit2DInfo(this.Fsm);
    if (!((Object) raycastHit2Dinfo.collider != (Object) null))
      return;
    this.gameObjectHit.Value = raycastHit2Dinfo.collider.gameObject;
    this.point.Value = raycastHit2Dinfo.point;
    this.normal.Value = (Vector3) raycastHit2Dinfo.normal;
    this.distance.Value = raycastHit2Dinfo.fraction;
  }
}
