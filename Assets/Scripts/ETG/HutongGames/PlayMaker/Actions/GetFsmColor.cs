// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the value of a Color Variable from another FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetFsmColor : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [UIHint(UIHint.FsmColor)]
    [RequiredField]
    public FsmString variableName;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmColor storeValue;
    public bool everyFrame;
    private GameObject goLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeValue = (FsmColor) null;
    }

    public override void OnEnter()
    {
      this.DoGetFsmColor();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmColor();

    private void DoGetFsmColor()
    {
      if (this.storeValue == null)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.goLastFrame)
      {
        this.goLastFrame = ownerDefaultTarget;
        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
      }
      if ((Object) this.fsm == (Object) null)
        return;
      FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
      if (fsmColor == null)
        return;
      this.storeValue.Value = fsmColor.Value;
    }
  }
}
