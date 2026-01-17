// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RectContains
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Tests if a point is inside a rectangle.")]
  [ActionCategory(ActionCategory.Rect)]
  public class RectContains : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Rectangle to test.")]
    [RequiredField]
    public FsmRect rectangle;
    [HutongGames.PlayMaker.Tooltip("Point to test.")]
    public FsmVector3 point;
    [HutongGames.PlayMaker.Tooltip("Specify/override X value.")]
    public FsmFloat x;
    [HutongGames.PlayMaker.Tooltip("Specify/override Y value.")]
    public FsmFloat y;
    [HutongGames.PlayMaker.Tooltip("Event to send if the Point is inside the Rectangle.")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if the Point is outside the Rectangle.")]
    public FsmEvent falseEvent;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the result in a variable.")]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      FsmRect fsmRect = new FsmRect();
      fsmRect.UseVariable = true;
      this.rectangle = fsmRect;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.point = fsmVector3;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.x = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.y = fsmFloat2;
      this.storeResult = (FsmBool) null;
      this.trueEvent = (FsmEvent) null;
      this.falseEvent = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoRectContains();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoRectContains();

    private void DoRectContains()
    {
      if (this.rectangle.IsNone)
        return;
      Vector3 point = this.point.Value;
      if (!this.x.IsNone)
        point.x = this.x.Value;
      if (!this.y.IsNone)
        point.y = this.y.Value;
      bool flag = this.rectangle.Value.Contains(point);
      this.storeResult.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
