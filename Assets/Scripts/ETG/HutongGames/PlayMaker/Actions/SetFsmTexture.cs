using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  [HutongGames.PlayMaker.Tooltip("Set the value of a Texture Variable in another FSM.")]
  public class SetFsmTexture : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
    [UIHint(UIHint.FsmTexture)]
    [RequiredField]
    public FsmString variableName;
    [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
    public FsmTexture setValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    private string fsmNameLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.variableName = (FsmString) string.Empty;
      this.setValue = (FsmTexture) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetFsmBool();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmBool()
    {
      if (this.setValue == null)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.goLastFrame || this.fsmName.Value != this.fsmNameLastFrame)
      {
        this.goLastFrame = ownerDefaultTarget;
        this.fsmNameLastFrame = this.fsmName.Value;
        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
      }
      if ((Object) this.fsm == (Object) null)
      {
        this.LogWarning("Could not find FSM: " + this.fsmName.Value);
      }
      else
      {
        FsmTexture fsmTexture = this.fsm.FsmVariables.FindFsmTexture(this.variableName.Value);
        if (fsmTexture != null)
          fsmTexture.Value = this.setValue.Value;
        else
          this.LogWarning("Could not find variable: " + this.variableName.Value);
      }
    }

    public override void OnUpdate() => this.DoSetFsmBool();
  }
}
