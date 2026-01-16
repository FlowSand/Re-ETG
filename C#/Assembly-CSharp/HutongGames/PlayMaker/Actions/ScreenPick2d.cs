// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ScreenPick2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Input)]
[HutongGames.PlayMaker.Tooltip("Perform a raycast into the 2d scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object. NOTE: Uses the MainCamera!")]
public class ScreenPick2d : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
  public FsmVector3 screenVector;
  [HutongGames.PlayMaker.Tooltip("X position on screen.")]
  public FsmFloat screenX;
  [HutongGames.PlayMaker.Tooltip("Y position on screen.")]
  public FsmFloat screenY;
  [HutongGames.PlayMaker.Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
  public FsmBool normalized;
  [HutongGames.PlayMaker.Tooltip("Store whether the Screen pick did pick a GameObject")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeDidPickObject;
  [HutongGames.PlayMaker.Tooltip("Store the picked GameObject")]
  [UIHint(UIHint.Variable)]
  public FsmGameObject storeGameObject;
  [HutongGames.PlayMaker.Tooltip("Store the picked position in world Space")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 storePoint;
  [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
  [UIHint(UIHint.Layer)]
  public FsmInt[] layerMask;
  [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
  public FsmBool invertMask;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    FsmVector3 fsmVector3 = new FsmVector3();
    fsmVector3.UseVariable = true;
    this.screenVector = fsmVector3;
    FsmFloat fsmFloat1 = new FsmFloat();
    fsmFloat1.UseVariable = true;
    this.screenX = fsmFloat1;
    FsmFloat fsmFloat2 = new FsmFloat();
    fsmFloat2.UseVariable = true;
    this.screenY = fsmFloat2;
    this.normalized = (FsmBool) false;
    this.storeDidPickObject = (FsmBool) null;
    this.storeGameObject = (FsmGameObject) null;
    this.storePoint = (FsmVector3) null;
    this.layerMask = new FsmInt[0];
    this.invertMask = (FsmBool) false;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoScreenPick();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoScreenPick();

  private void DoScreenPick()
  {
    if ((Object) Camera.main == (Object) null)
    {
      this.LogError("No MainCamera defined!");
      this.Finish();
    }
    else
    {
      Vector3 zero = Vector3.zero;
      if (!this.screenVector.IsNone)
        zero = this.screenVector.Value;
      if (!this.screenX.IsNone)
        zero.x = this.screenX.Value;
      if (!this.screenY.IsNone)
        zero.y = this.screenY.Value;
      if (this.normalized.Value)
      {
        zero.x *= (float) Screen.width;
        zero.y *= (float) Screen.height;
      }
      RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(zero), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
      bool flag = (Object) rayIntersection.collider != (Object) null;
      this.storeDidPickObject.Value = flag;
      if (flag)
      {
        this.storeGameObject.Value = rayIntersection.collider.gameObject;
        this.storePoint.Value = (Vector3) rayIntersection.point;
      }
      else
      {
        this.storeGameObject.Value = (GameObject) null;
        this.storePoint.Value = Vector3.zero;
      }
    }
  }
}
