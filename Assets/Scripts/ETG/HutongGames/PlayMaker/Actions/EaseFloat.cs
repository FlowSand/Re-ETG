// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.EaseFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.AnimateVariables)]
  [Tooltip("Easing Animation - Float")]
  public class EaseFloat : EaseFsmAction
  {
    [RequiredField]
    public FsmFloat fromValue;
    [RequiredField]
    public FsmFloat toValue;
    [UIHint(UIHint.Variable)]
    public FsmFloat floatVariable;
    private bool finishInNextStep;

    public override void Reset()
    {
      base.Reset();
      this.floatVariable = (FsmFloat) null;
      this.fromValue = (FsmFloat) null;
      this.toValue = (FsmFloat) null;
      this.finishInNextStep = false;
    }

    public override void OnEnter()
    {
      base.OnEnter();
      this.fromFloats = new float[1];
      this.fromFloats[0] = this.fromValue.Value;
      this.toFloats = new float[1];
      this.toFloats[0] = this.toValue.Value;
      this.resultFloats = new float[1];
      this.finishInNextStep = false;
      this.floatVariable.Value = this.fromValue.Value;
    }

    public override void OnExit() => base.OnExit();

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (!this.floatVariable.IsNone && this.isRunning)
        this.floatVariable.Value = this.resultFloats[0];
      if (this.finishInNextStep)
      {
        this.Finish();
        if (this.finishEvent != null)
          this.Fsm.Event(this.finishEvent);
      }
      if (!this.finishAction || this.finishInNextStep)
        return;
      if (!this.floatVariable.IsNone)
        this.floatVariable.Value = !this.reverse.IsNone ? (!this.reverse.Value ? this.toValue.Value : this.fromValue.Value) : this.toValue.Value;
      this.finishInNextStep = true;
    }
  }
}
