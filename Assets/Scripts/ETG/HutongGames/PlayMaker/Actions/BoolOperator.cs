// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BoolOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Performs boolean operations on 2 Bool Variables.")]
  [ActionCategory(ActionCategory.Math)]
  public class BoolOperator : FsmStateAction
  {
    [Tooltip("The first Bool variable.")]
    [RequiredField]
    public FsmBool bool1;
    [Tooltip("The second Bool variable.")]
    [RequiredField]
    public FsmBool bool2;
    [Tooltip("Boolean Operation.")]
    public BoolOperator.Operation operation;
    [Tooltip("Store the result in a Bool Variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool storeResult;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.bool1 = (FsmBool) false;
      this.bool2 = (FsmBool) false;
      this.operation = BoolOperator.Operation.AND;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoBoolOperator();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoBoolOperator();

    private void DoBoolOperator()
    {
      bool flag1 = this.bool1.Value;
      bool flag2 = this.bool2.Value;
      switch (this.operation)
      {
        case BoolOperator.Operation.AND:
          this.storeResult.Value = flag1 && flag2;
          break;
        case BoolOperator.Operation.NAND:
          this.storeResult.Value = (!flag1 ? 0 : (flag2 ? 1 : 0)) == 0;
          break;
        case BoolOperator.Operation.OR:
          this.storeResult.Value = flag1 || flag2;
          break;
        case BoolOperator.Operation.XOR:
          this.storeResult.Value = flag1 ^ flag2;
          break;
      }
    }

    public enum Operation
    {
      AND,
      NAND,
      OR,
      XOR,
    }
  }
}
