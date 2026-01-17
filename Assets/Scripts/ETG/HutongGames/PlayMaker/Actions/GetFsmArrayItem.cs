// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmArrayItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [Tooltip("Gets an item in an Array Variable in another FSM.")]
  public class GetFsmArrayItem : BaseFsmVariableIndexAction
  {
    [Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [Tooltip("Optional name of FSM on Game Object.")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [RequiredField]
    [Tooltip("The name of the FSM variable.")]
    [UIHint(UIHint.FsmArray)]
    public FsmString variableName;
    [Tooltip("The index into the array.")]
    public FsmInt index;
    [UIHint(UIHint.Variable)]
    [Tooltip("Get the value of the array at the specified index.")]
    [RequiredField]
    public FsmVar storeValue;
    [Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeValue = (FsmVar) null;
    }

    public override void OnEnter()
    {
      this.DoGetFsmArray();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoGetFsmArray()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject), this.fsmName.Value))
        return;
      FsmArray fsmArray = this.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
      if (fsmArray != null)
      {
        if (this.index.Value < 0 || this.index.Value >= fsmArray.Length)
        {
          this.Fsm.Event(this.indexOutOfRange);
          this.Finish();
        }
        else if (fsmArray.ElementType == this.storeValue.NamedVar.VariableType)
          this.storeValue.SetValue(fsmArray.Get(this.index.Value));
        else
          this.LogWarning("Incompatible variable type: " + this.variableName.Value);
      }
      else
        this.DoVariableNotFound(this.variableName.Value);
    }

    public override void OnUpdate() => this.DoGetFsmArray();
  }
}
