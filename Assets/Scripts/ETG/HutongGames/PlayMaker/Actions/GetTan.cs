using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the Tangent. You can use degrees, simply check on the DegToRad conversion")]
  [ActionCategory(ActionCategory.Trigonometry)]
  public class GetTan : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
    [RequiredField]
    public FsmFloat angle;
    [HutongGames.PlayMaker.Tooltip("Check on if the angle is expressed in degrees.")]
    public FsmBool DegToRad;
    [HutongGames.PlayMaker.Tooltip("The angle tan")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat result;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.angle = (FsmFloat) null;
      this.DegToRad = (FsmBool) true;
      this.everyFrame = false;
      this.result = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      this.DoTan();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoTan();

    private void DoTan()
    {
      float f = this.angle.Value;
      if (this.DegToRad.Value)
        f *= (float) Math.PI / 180f;
      this.result.Value = Mathf.Tan(f);
    }
  }
}
