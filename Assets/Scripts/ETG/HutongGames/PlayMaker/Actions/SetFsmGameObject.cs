// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetFsmGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SetFsmGameObject : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    public FsmString fsmName;
    [UIHint(UIHint.FsmGameObject)]
    [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
    [RequiredField]
    public FsmString variableName;
    [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
    public FsmGameObject setValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    private string fsmNameLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.setValue = (FsmGameObject) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetFsmGameObject();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmGameObject()
    {
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
        return;
      FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
      if (fsmGameObject != null)
        fsmGameObject.Value = this.setValue != null ? this.setValue.Value : (GameObject) null;
      else
        this.LogWarning("Could not find variable: " + this.variableName.Value);
    }

    public override void OnUpdate() => this.DoSetFsmGameObject();
  }
}
