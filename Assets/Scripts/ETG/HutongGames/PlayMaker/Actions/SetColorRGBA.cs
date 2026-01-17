// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetColorRGBA
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
  [ActionCategory(ActionCategory.Color)]
  public class SetColorRGBA : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmColor colorVariable;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat red;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat green;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat blue;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat alpha;
    public bool everyFrame;

    public override void Reset()
    {
      this.colorVariable = (FsmColor) null;
      this.red = (FsmFloat) 0.0f;
      this.green = (FsmFloat) 0.0f;
      this.blue = (FsmFloat) 0.0f;
      this.alpha = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetColorRGBA();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetColorRGBA();

    private void DoSetColorRGBA()
    {
      if (this.colorVariable == null)
        return;
      Color color = this.colorVariable.Value;
      if (!this.red.IsNone)
        color.r = this.red.Value;
      if (!this.green.IsNone)
        color.g = this.green.Value;
      if (!this.blue.IsNone)
        color.b = this.blue.Value;
      if (!this.alpha.IsNone)
        color.a = this.alpha.Value;
      this.colorVariable.Value = color;
    }
  }
}
