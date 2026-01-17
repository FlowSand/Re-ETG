// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorRoot : FsmStateActionAnimatorBase
  {
    [RequiredField]
    [CheckForComponent(typeof (Animator))]
    [HutongGames.PlayMaker.Tooltip("The target.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
    [ActionSection("Results")]
    public FsmVector3 rootPosition;
    [HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
    [UIHint(UIHint.Variable)]
    public FsmQuaternion rootRotation;
    [HutongGames.PlayMaker.Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
    public FsmGameObject bodyGameObject;
    private Animator _animator;
    private Transform _transform;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.rootPosition = (FsmVector3) null;
      this.rootRotation = (FsmQuaternion) null;
      this.bodyGameObject = (FsmGameObject) null;
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
      this.rootPosition.Value = this._animator.rootPosition;
      this.rootRotation.Value = this._animator.rootRotation;
      if (!((Object) this._transform != (Object) null))
        return;
      this._transform.position = this._animator.rootPosition;
      this._transform.rotation = this._animator.rootRotation;
    }
  }
}
