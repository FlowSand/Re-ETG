// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorLayerCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Returns the Animator controller layer count")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorLayerCount : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  [ActionSection("Results")]
  [HutongGames.PlayMaker.Tooltip("The Animator controller layer count")]
  [UIHint(UIHint.Variable)]
  public FsmInt layerCount;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.layerCount = (FsmInt) null;
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
        this.DoGetLayerCount();
        this.Finish();
      }
    }
  }

  private void DoGetLayerCount()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this.layerCount.Value = this._animator.layerCount;
  }
}
