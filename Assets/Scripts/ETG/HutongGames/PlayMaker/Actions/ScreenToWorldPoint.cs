// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ScreenToWorldPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Camera)]
  [HutongGames.PlayMaker.Tooltip("Transforms position from screen space into world space. NOTE: Uses the MainCamera!")]
  public class ScreenToWorldPoint : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Screen position as a vector.")]
    public FsmVector3 screenVector;
    [HutongGames.PlayMaker.Tooltip("Screen X position in pixels or normalized. See Normalized.")]
    public FsmFloat screenX;
    [HutongGames.PlayMaker.Tooltip("Screen X position in pixels or normalized. See Normalized.")]
    public FsmFloat screenY;
    [HutongGames.PlayMaker.Tooltip("Distance into the screen in world units.")]
    public FsmFloat screenZ;
    [HutongGames.PlayMaker.Tooltip("If true, X/Y coordinates are considered normalized (0-1), otherwise they are expected to be in pixels")]
    public FsmBool normalized;
    [HutongGames.PlayMaker.Tooltip("Store the world position in a vector3 variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeWorldVector;
    [HutongGames.PlayMaker.Tooltip("Store the world X position in a float variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeWorldX;
    [HutongGames.PlayMaker.Tooltip("Store the world Y position in a float variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeWorldY;
    [HutongGames.PlayMaker.Tooltip("Store the world Z position in a float variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeWorldZ;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.screenVector = (FsmVector3) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.screenX = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.screenY = fsmFloat2;
      this.screenZ = (FsmFloat) 1f;
      this.normalized = (FsmBool) false;
      this.storeWorldVector = (FsmVector3) null;
      this.storeWorldX = (FsmFloat) null;
      this.storeWorldY = (FsmFloat) null;
      this.storeWorldZ = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoScreenToWorldPoint();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoScreenToWorldPoint();

    private void DoScreenToWorldPoint()
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
        if (!this.screenZ.IsNone)
          zero.z = this.screenZ.Value;
        if (this.normalized.Value)
        {
          zero.x *= (float) Screen.width;
          zero.y *= (float) Screen.height;
        }
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(zero);
        this.storeWorldVector.Value = worldPoint;
        this.storeWorldX.Value = worldPoint.x;
        this.storeWorldY.Value = worldPoint.y;
        this.storeWorldZ.Value = worldPoint.z;
      }
    }
  }
}
