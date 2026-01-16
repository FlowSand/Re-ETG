// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ControllerIsGrounded
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
[ActionCategory(ActionCategory.Character)]
public class ControllerIsGrounded : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
  [CheckForComponent(typeof (CharacterController))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Event to send if touching the ground.")]
  public FsmEvent trueEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if not touching the ground.")]
  public FsmEvent falseEvent;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
  public FsmBool storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;
  private GameObject previousGo;
  private CharacterController controller;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.trueEvent = (FsmEvent) null;
    this.falseEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoControllerIsGrounded();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoControllerIsGrounded();

  private void DoControllerIsGrounded()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    if ((Object) ownerDefaultTarget != (Object) this.previousGo)
    {
      this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
      this.previousGo = ownerDefaultTarget;
    }
    if ((Object) this.controller == (Object) null)
      return;
    bool isGrounded = this.controller.isGrounded;
    this.storeResult.Value = isGrounded;
    this.Fsm.Event(!isGrounded ? this.falseEvent : this.trueEvent);
  }
}
