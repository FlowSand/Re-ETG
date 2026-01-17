// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetACosine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the Arc Cosine. You can get the result in degrees, simply check on the RadToDeg conversion")]
  [ActionCategory(ActionCategory.Trigonometry)]
  public class GetACosine : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The value of the cosine")]
    [RequiredField]
    public FsmFloat Value;
    [HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat angle;
    [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
    public FsmBool RadToDeg;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.angle = (FsmFloat) null;
      this.RadToDeg = (FsmBool) true;
      this.everyFrame = false;
      this.Value = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      this.DoACosine();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoACosine();

    private void DoACosine()
    {
      float num = Mathf.Acos(this.Value.Value);
      if (this.RadToDeg.Value)
        num *= 57.29578f;
      this.angle.Value = num;
    }
  }
}
