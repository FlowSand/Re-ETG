// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetTouchInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Device)]
  [HutongGames.PlayMaker.Tooltip("Gets info on a touch event.")]
  public class GetTouchInfo : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
    public FsmInt fingerId;
    [HutongGames.PlayMaker.Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
    public FsmBool normalize;
    [UIHint(UIHint.Variable)]
    public FsmVector3 storePosition;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeX;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeY;
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeDeltaPosition;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeDeltaX;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeDeltaY;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeDeltaTime;
    [UIHint(UIHint.Variable)]
    public FsmInt storeTapCount;
    public bool everyFrame = true;
    private float screenWidth;
    private float screenHeight;

    public override void Reset()
    {
      FsmInt fsmInt = new FsmInt();
      fsmInt.UseVariable = true;
      this.fingerId = fsmInt;
      this.normalize = (FsmBool) true;
      this.storePosition = (FsmVector3) null;
      this.storeDeltaPosition = (FsmVector3) null;
      this.storeDeltaTime = (FsmFloat) null;
      this.storeTapCount = (FsmInt) null;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.screenWidth = (float) Screen.width;
      this.screenHeight = (float) Screen.height;
      this.DoGetTouchInfo();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetTouchInfo();

    private void DoGetTouchInfo()
    {
      if (Input.touchCount <= 0)
        return;
      foreach (Touch touch in Input.touches)
      {
        if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
        {
          float x1 = this.normalize.Value ? touch.position.x / this.screenWidth : touch.position.x;
          float y1 = this.normalize.Value ? touch.position.y / this.screenHeight : touch.position.y;
          if (!this.storePosition.IsNone)
            this.storePosition.Value = new Vector3(x1, y1, 0.0f);
          this.storeX.Value = x1;
          this.storeY.Value = y1;
          float x2 = this.normalize.Value ? touch.deltaPosition.x / this.screenWidth : touch.deltaPosition.x;
          float y2 = this.normalize.Value ? touch.deltaPosition.y / this.screenHeight : touch.deltaPosition.y;
          if (!this.storeDeltaPosition.IsNone)
            this.storeDeltaPosition.Value = new Vector3(x2, y2, 0.0f);
          this.storeDeltaX.Value = x2;
          this.storeDeltaY.Value = y2;
          this.storeDeltaTime.Value = touch.deltaTime;
          this.storeTapCount.Value = touch.tapCount;
        }
      }
    }
  }
}
