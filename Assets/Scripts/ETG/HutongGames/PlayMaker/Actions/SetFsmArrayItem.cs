#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Set an item in an Array Variable in another FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class SetFsmArrayItem : BaseFsmVariableIndexAction
  {
    [Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [Tooltip("Optional name of FSM on Game Object.")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [UIHint(UIHint.FsmArray)]
    [Tooltip("The name of the FSM variable.")]
    [RequiredField]
    public FsmString variableName;
    [Tooltip("The index into the array.")]
    public FsmInt index;
    [RequiredField]
    [Tooltip("Set the value of the array at the specified index.")]
    public FsmVar value;
    [Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.value = (FsmVar) null;
    }

    public override void OnEnter()
    {
      this.DoSetFsmArray();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmArray()
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
        else if (fsmArray.ElementType == this.value.NamedVar.VariableType)
        {
          this.value.UpdateValue();
          fsmArray.Set(this.index.Value, this.value.GetValue());
        }
        else
          this.LogWarning("Incompatible variable type: " + this.variableName.Value);
      }
      else
        this.DoVariableNotFound(this.variableName.Value);
    }

    public override void OnUpdate() => this.DoSetFsmArray();
  }
}
