// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetVector2XY
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Get the XY channels of a Vector2 Variable and store them in Float Variables.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class GetVector2XY : FsmStateAction
  {
    [Tooltip("The vector2 source")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector2 vector2Variable;
    [Tooltip("The x component")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeX;
    [UIHint(UIHint.Variable)]
    [Tooltip("The y component")]
    public FsmFloat storeY;
    [Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      this.storeX = (FsmFloat) null;
      this.storeY = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetVector2XYZ();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetVector2XYZ();

    private void DoGetVector2XYZ()
    {
      if (this.vector2Variable == null)
        return;
      if (this.storeX != null)
        this.storeX.Value = this.vector2Variable.Value.x;
      if (this.storeY == null)
        return;
      this.storeY.Value = this.vector2Variable.Value.y;
    }
  }
}
