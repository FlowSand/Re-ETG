// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetFsmColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [HutongGames.PlayMaker.Tooltip("Set the value of a Color Variable in another FSM.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SetFsmColor : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
    [RequiredField]
    [UIHint(UIHint.FsmColor)]
    public FsmString variableName;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
    public FsmColor setValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    private string fsmNameLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.setValue = (FsmColor) null;
    }

    public override void OnEnter()
    {
      this.DoSetFsmColor();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmColor()
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
        FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
        if (fsmColor != null)
          fsmColor.Value = this.setValue.Value;
        else
          this.LogWarning("Could not find variable: " + this.variableName.Value);
      }
    }

    public override void OnUpdate() => this.DoSetFsmColor();
  }
}
