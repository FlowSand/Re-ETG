// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetFsmVector2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Set the value of a Vector2 Variable in another FSM.")]
[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[ActionCategory(ActionCategory.StateMachine)]
public class SetFsmVector2 : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
  [UIHint(UIHint.FsmName)]
  public FsmString fsmName;
  [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
  [UIHint(UIHint.FsmVector2)]
  [RequiredField]
  public FsmString variableName;
  [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
  [RequiredField]
  public FsmVector2 setValue;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
  public bool everyFrame;
  private GameObject goLastFrame;
  private string fsmNameLastFrame;
  private PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.fsmName = (FsmString) string.Empty;
    this.setValue = (FsmVector2) null;
  }

  public override void OnEnter()
  {
    this.DoSetFsmVector2();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoSetFsmVector2()
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
      FsmVector2 fsmVector2 = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
      if (fsmVector2 != null)
        fsmVector2.Value = this.setValue.Value;
      else
        this.LogWarning("Could not find variable: " + this.variableName.Value);
    }
  }

  public override void OnUpdate() => this.DoSetFsmVector2();
}
