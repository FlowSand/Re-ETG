// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorLayerName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Returns the name of a layer from its index")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorLayerName : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The layer index")]
    [RequiredField]
    public FsmInt layerIndex;
    [ActionSection("Results")]
    [HutongGames.PlayMaker.Tooltip("The layer name")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString layerName;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.layerIndex = (FsmInt) null;
      this.layerName = (FsmString) null;
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
          this.DoGetLayerName();
          this.Finish();
        }
      }
    }

    private void DoGetLayerName()
    {
      if ((Object) this._animator == (Object) null)
        return;
      this.layerName.Value = this._animator.GetLayerName(this.layerIndex.Value);
    }
  }
}
