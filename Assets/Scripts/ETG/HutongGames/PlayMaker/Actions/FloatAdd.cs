// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds a value to a Float Variable.")]
  [ActionCategory(ActionCategory.Math)]
  public class FloatAdd : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Float variable to add to.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat floatVariable;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Amount to add.")]
    public FsmFloat add;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;
    [HutongGames.PlayMaker.Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
    public bool perSecond;

    public override void Reset()
    {
      this.floatVariable = (FsmFloat) null;
      this.add = (FsmFloat) null;
      this.everyFrame = false;
      this.perSecond = false;
    }

    public override void OnEnter()
    {
      this.DoFloatAdd();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoFloatAdd();

    private void DoFloatAdd()
    {
      if (!this.perSecond)
        this.floatVariable.Value += this.add.Value;
      else
        this.floatVariable.Value += this.add.Value * Time.deltaTime;
    }
  }
}
