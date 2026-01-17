// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorBody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Gets the avatar body mass center position and rotation. Optionally accepts a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
public class GetAnimatorBody : FsmStateActionAnimatorBase
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
  [UIHint(UIHint.Variable)]
  [ActionSection("Results")]
  public FsmVector3 bodyPosition;
  [HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
  [UIHint(UIHint.Variable)]
  public FsmQuaternion bodyRotation;
  [HutongGames.PlayMaker.Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
  public FsmGameObject bodyGameObject;
  private Animator _animator;
  private Transform _transform;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.bodyPosition = (FsmVector3) null;
    this.bodyRotation = (FsmQuaternion) null;
    this.bodyGameObject = (FsmGameObject) null;
    this.everyFrame = false;
  }

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
        GameObject gameObject = this.bodyGameObject.Value;
        if ((Object) gameObject != (Object) null)
          this._transform = gameObject.transform;
        this.DoGetBodyPosition();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.DoGetBodyPosition();

  private void DoGetBodyPosition()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this.bodyPosition.Value = this._animator.bodyPosition;
    this.bodyRotation.Value = this._animator.bodyRotation;
    if (!((Object) this._transform != (Object) null))
      return;
    this._transform.position = this._animator.bodyPosition;
    this._transform.rotation = this._animator.bodyRotation;
  }
}
