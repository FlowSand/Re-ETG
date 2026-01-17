// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorBody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
public class SetAnimatorBody : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The gameObject target of the ik goal")]
  public FsmGameObject target;
  [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
  public FsmVector3 position;
  [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
  public FsmQuaternion rotation;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;
  private Animator _animator;
  private Transform _transform;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.target = (FsmGameObject) null;
    FsmVector3 fsmVector3 = new FsmVector3();
    fsmVector3.UseVariable = true;
    this.position = fsmVector3;
    FsmQuaternion fsmQuaternion = new FsmQuaternion();
    fsmQuaternion.UseVariable = true;
    this.rotation = fsmQuaternion;
    this.everyFrame = false;
  }

  public override void OnPreprocess() => this.Fsm.HandleAnimatorIK = true;

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
    {
      this.Finish();
    }
    else
    {
      this._animator = ownerDefaultTarget.GetComponent<Animator>();
      if ((Object) this._animator == (Object) null)
      {
        this.Finish();
      }
      else
      {
        GameObject gameObject = this.target.Value;
        if (!((Object) gameObject != (Object) null))
          return;
        this._transform = gameObject.transform;
      }
    }
  }

  public override void DoAnimatorIK(int layerIndex)
  {
    this.DoSetBody();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoSetBody()
  {
    if ((Object) this._animator == (Object) null)
      return;
    if ((Object) this._transform != (Object) null)
    {
      this._animator.bodyPosition = !this.position.IsNone ? this._transform.position + this.position.Value : this._transform.position;
      if (this.rotation.IsNone)
        this._animator.bodyRotation = this._transform.rotation;
      else
        this._animator.bodyRotation = this._transform.rotation * this.rotation.Value;
    }
    else
    {
      if (!this.position.IsNone)
        this._animator.bodyPosition = this.position.Value;
      if (this.rotation.IsNone)
        return;
      this._animator.bodyRotation = this.rotation.Value;
    }
  }
}
