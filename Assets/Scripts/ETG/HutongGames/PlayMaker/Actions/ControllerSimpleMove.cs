// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ControllerSimpleMove
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
  [ActionCategory(ActionCategory.Character)]
  public class ControllerSimpleMove : FsmStateAction
  {
    [CheckForComponent(typeof (CharacterController))]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject to move.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The movement vector.")]
    [RequiredField]
    public FsmVector3 moveVector;
    [HutongGames.PlayMaker.Tooltip("Multiply the movement vector by a speed factor.")]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("Move in local or word space.")]
    public Space space;
    private GameObject previousGo;
    private CharacterController controller;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.moveVector = fsmVector3;
      this.speed = (FsmFloat) 1f;
      this.space = Space.World;
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
      this.controller.SimpleMove((this.space != Space.World ? ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value) : this.moveVector.Value) * this.speed.Value);
    }
  }
}
