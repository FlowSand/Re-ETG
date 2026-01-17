// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmArray
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Copy an Array Variable from another FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class GetFsmArray : BaseFsmVariableAction
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
    [UIHint(UIHint.Variable)]
    [Tooltip("Get the content of the array variable.")]
    [RequiredField]
    public FsmArray storeValue;
    [Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
    public bool copyValues;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.variableName = (FsmString) null;
      this.storeValue = (FsmArray) null;
      this.copyValues = true;
    }

    public override void OnEnter()
    {
      this.DoSetFsmArrayCopy();
      this.Finish();
    }

    private void DoSetFsmArrayCopy()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject), this.fsmName.Value))
        return;
      FsmArray fsmArray = this.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
      if (fsmArray != null)
      {
        if (fsmArray.ElementType != this.storeValue.ElementType)
        {
          this.LogError($"Can only copy arrays with the same elements type. Found <{(object) fsmArray.ElementType}> and <{(object) this.storeValue.ElementType}>");
        }
        else
        {
          this.storeValue.Resize(0);
          if (this.copyValues)
            this.storeValue.Values = fsmArray.Values.Clone() as object[];
          else
            this.storeValue.Values = fsmArray.Values;
        }
      }
      else
        this.DoVariableNotFound(this.variableName.Value);
    }
  }
}
