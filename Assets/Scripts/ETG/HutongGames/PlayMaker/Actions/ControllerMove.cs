// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ControllerMove
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Moves a Game Object with a Character Controller. See also Controller Simple Move. NOTE: It is recommended that you make only one call to Move or SimpleMove per frame.")]
  [ActionCategory(ActionCategory.Character)]
  public class ControllerMove : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to move.")]
    [CheckForComponent(typeof (CharacterController))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The movement vector.")]
    [RequiredField]
    public FsmVector3 moveVector;
    [HutongGames.PlayMaker.Tooltip("Move in local or word space.")]
    public Space space;
    [HutongGames.PlayMaker.Tooltip("Movement vector is defined in units per second. Makes movement frame rate independent.")]
    public FsmBool perSecond;
    private GameObject previousGo;
    private CharacterController controller;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.moveVector = fsmVector3;
      this.space = Space.World;
      this.perSecond = (FsmBool) true;
    }

    public override void OnUpdate()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.previousGo)
      {
        this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
        this.previousGo = ownerDefaultTarget;
      }
      if (!((Object) this.controller != (Object) null))
        return;
      Vector3 motion = this.space != Space.World ? ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value) : this.moveVector.Value;
      if (this.perSecond.Value)
      {
        int num1 = (int) this.controller.Move(motion * Time.deltaTime);
      }
      else
      {
        int num2 = (int) this.controller.Move(motion);
      }
    }
  }
}
