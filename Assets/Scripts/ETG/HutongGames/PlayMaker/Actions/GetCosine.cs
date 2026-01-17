// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetCosine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Trigonometry)]
  [HutongGames.PlayMaker.Tooltip("Get the cosine. You can use degrees, simply check on the DegToRad conversion")]
  public class GetCosine : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
    [RequiredField]
    public FsmFloat angle;
    [HutongGames.PlayMaker.Tooltip("Check on if the angle is expressed in degrees.")]
    public FsmBool DegToRad;
    [HutongGames.PlayMaker.Tooltip("The angle cosinus")]
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
      this.DoCosine();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoCosine();

    private void DoCosine()
    {
      float f = this.angle.Value;
      if (this.DegToRad.Value)
        f *= (float) Math.PI / 180f;
      this.result.Value = Mathf.Cos(f);
    }
  }
}
