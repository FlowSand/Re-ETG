// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.UnityObject)]
[HutongGames.PlayMaker.Tooltip("Gets a Component attached to a GameObject and stores it in an Object variable. NOTE: Set the Object variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
public class GetComponent : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the component.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmObject storeComponent;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.storeComponent = (FsmObject) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetComponent();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetComponent();

  private void DoGetComponent()
  {
    if (this.storeComponent == null)
      return;
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null || this.storeComponent.IsNone)
      return;
    this.storeComponent.Value = (Object) ownerDefaultTarget.GetComponent(this.storeComponent.ObjectType);
  }
}
