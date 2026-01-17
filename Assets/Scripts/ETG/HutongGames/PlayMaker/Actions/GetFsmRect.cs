// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmRect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the value of a Rect Variable from another FSM.")]
  [ActionCategory(ActionCategory.StateMachine)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class GetFsmRect : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [RequiredField]
    [UIHint(UIHint.FsmRect)]
    public FsmString variableName;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmRect storeValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    protected PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.variableName = (FsmString) string.Empty;
      this.storeValue = (FsmRect) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetFsmVariable();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmVariable();

    private void DoGetFsmVariable()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.goLastFrame)
      {
        this.goLastFrame = ownerDefaultTarget;
        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
      }
      if ((Object) this.fsm == (Object) null || this.storeValue == null)
        return;
      FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
      if (fsmRect == null)
        return;
      this.storeValue.Value = fsmRect.Value;
    }
  }
}
