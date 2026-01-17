// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetRectFields
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Get the individual fields of a Rect Variable and store them in Float Variables.")]
  [ActionCategory(ActionCategory.Rect)]
  public class GetRectFields : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmRect rectVariable;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeX;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeY;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeWidth;
    [UIHint(UIHint.Variable)]
    public FsmFloat storeHeight;
    public bool everyFrame;

    public override void Reset()
    {
      this.rectVariable = (FsmRect) null;
      this.storeX = (FsmFloat) null;
      this.storeY = (FsmFloat) null;
      this.storeWidth = (FsmFloat) null;
      this.storeHeight = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetRectFields();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetRectFields();

    private void DoGetRectFields()
    {
      if (this.rectVariable.IsNone)
        return;
      this.storeX.Value = this.rectVariable.Value.x;
      this.storeY.Value = this.rectVariable.Value.y;
      this.storeWidth.Value = this.rectVariable.Value.width;
      this.storeHeight.Value = this.rectVariable.Value.height;
    }
  }
}
