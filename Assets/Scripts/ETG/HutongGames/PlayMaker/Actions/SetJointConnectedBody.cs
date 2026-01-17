// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetJointConnectedBody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Connect a joint to a game object.")]
  [ActionCategory(ActionCategory.Physics)]
  public class SetJointConnectedBody : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The joint to connect. Requires a Joint component.")]
    [CheckForComponent(typeof (Joint))]
    [RequiredField]
    public FsmOwnerDefault joint;
    [HutongGames.PlayMaker.Tooltip("The game object to connect to the Joint. Set to none to connect the Joint to the world.")]
    [CheckForComponent(typeof (Rigidbody))]
    public FsmGameObject rigidBody;

    public override void Reset()
    {
      this.joint = (FsmOwnerDefault) null;
      this.rigidBody = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.joint);
      if ((Object) ownerDefaultTarget != (Object) null)
      {
        Joint component = ownerDefaultTarget.GetComponent<Joint>();
        if ((Object) component != (Object) null)
          component.connectedBody = !((Object) this.rigidBody.Value == (Object) null) ? this.rigidBody.Value.GetComponent<Rigidbody>() : (Rigidbody) null;
      }
      this.Finish();
    }
  }
}
