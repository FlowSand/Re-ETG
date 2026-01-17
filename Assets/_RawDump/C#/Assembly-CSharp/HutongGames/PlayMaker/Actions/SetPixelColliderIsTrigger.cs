// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetPixelColliderIsTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the IsTriger property of a PixelCollider.")]
[ActionCategory(".Brave")]
public class SetPixelColliderIsTrigger : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("If null, use self.")]
  public FsmGameObject targetObject;
  [HutongGames.PlayMaker.Tooltip("PixelCollider index to set (0 indexed).")]
  public FsmInt colliderIndex;
  [HutongGames.PlayMaker.Tooltip("The new value of the IsTrigger flag on the PixelCollider.")]
  public FsmBool isTriggerValue;

  public override void Reset()
  {
    this.colliderIndex = (FsmInt) 0;
    this.isTriggerValue = (FsmBool) false;
  }

  public override void OnEnter()
  {
    if ((Object) this.targetObject.Value == (Object) null)
      this.Owner.GetComponent<TalkDoerLite>().specRigidbody.PixelColliders[this.colliderIndex.Value].IsTrigger = this.isTriggerValue.Value;
    else
      this.targetObject.Value.GetComponent<TalkDoerLite>().specRigidbody.PixelColliders[this.colliderIndex.Value].IsTrigger = this.isTriggerValue.Value;
    this.Finish();
  }
}
