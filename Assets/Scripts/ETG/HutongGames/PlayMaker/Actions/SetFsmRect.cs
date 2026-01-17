// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetFsmRect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the value of a Rect Variable in another FSM.")]
  [ActionCategory(ActionCategory.StateMachine)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class SetFsmRect : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
    [RequiredField]
    [UIHint(UIHint.FsmRect)]
    public FsmString variableName;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
    public FsmRect setValue;
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
      this.setValue = (FsmRect) null;
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
        FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
        if (fsmRect != null)
          fsmRect.Value = this.setValue.Value;
        else
          this.LogWarning("Could not find variable: " + this.variableName.Value);
      }
    }

    public override void OnUpdate() => this.DoSetFsmBool();
  }
}
