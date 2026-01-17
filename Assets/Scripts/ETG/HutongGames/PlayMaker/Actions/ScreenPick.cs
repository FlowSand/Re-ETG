// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ScreenPick
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Perform a raycast into the scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the object. NOTE: Uses the MainCamera!")]
  [ActionCategory(ActionCategory.Input)]
  public class ScreenPick : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
    public FsmVector3 screenVector;
    [HutongGames.PlayMaker.Tooltip("X position on screen.")]
    public FsmFloat screenX;
    [HutongGames.PlayMaker.Tooltip("Y position on screen.")]
    public FsmFloat screenY;
    [HutongGames.PlayMaker.Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
    public FsmBool normalized;
    [RequiredField]
    public FsmFloat rayDistance = (FsmFloat) 100f;
    [UIHint(UIHint.Variable)]
    public FsmBool storeDidPickObject;
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeGameObject;
    [UIHint(UIHint.Variable)]
    public FsmVector3 storePoint;
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeNormal;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeDistance;
    [UIHint(UIHint.Layer)]
    [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
    public FsmInt[] layerMask;
    [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
    public FsmBool invertMask;
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
      this.rayDistance = (FsmFloat) 100f;
      this.storeDidPickObject = (FsmBool) null;
      this.storeGameObject = (FsmGameObject) null;
      this.storePoint = (FsmVector3) null;
      this.storeNormal = (FsmVector3) null;
      this.storeDistance = (FsmFloat) null;
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
        RaycastHit hitInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(zero), out hitInfo, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
        bool flag = (Object) hitInfo.collider != (Object) null;
        this.storeDidPickObject.Value = flag;
        if (flag)
        {
          this.storeGameObject.Value = hitInfo.collider.gameObject;
          this.storeDistance.Value = hitInfo.distance;
          this.storePoint.Value = hitInfo.point;
          this.storeNormal.Value = hitInfo.normal;
        }
        else
        {
          this.storeGameObject.Value = (GameObject) null;
          this.storeDistance = (FsmFloat) float.PositiveInfinity;
          this.storePoint.Value = Vector3.zero;
          this.storeNormal.Value = Vector3.zero;
        }
      }
    }
  }
}
