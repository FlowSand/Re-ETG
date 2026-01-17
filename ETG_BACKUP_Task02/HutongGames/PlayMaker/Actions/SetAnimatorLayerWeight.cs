// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorLayerWeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Sets the layer's current weight")]
public class SetAnimatorLayerWeight : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The layer's index")]
  [RequiredField]
  public FsmInt layerIndex;
  [HutongGames.PlayMaker.Tooltip("Sets the layer's current weight")]
  [RequiredField]
  public FsmFloat layerWeight;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
  public bool everyFrame;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.layerIndex = (FsmInt) null;
    this.layerWeight = (FsmFloat) null;
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
        this.DoLayerWeight();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnUpdate() => this.DoLayerWeight();

  private void DoLayerWeight()
  {
    if ((Object) this._animator == (Object) null)
      return;
    this._animator.SetLayerWeight(this.layerIndex.Value, this.layerWeight.Value);
  }
}
