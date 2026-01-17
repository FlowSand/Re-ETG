// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetTriggerInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics)]
[HutongGames.PlayMaker.Tooltip("Gets info on the last Trigger event and store in variables.")]
public class GetTriggerInfo : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  public FsmGameObject gameObjectHit;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Useful for triggering different effects. Audio, particles...")]
  public FsmString physicsMaterialName;

  public override void Reset()
  {
    this.gameObjectHit = (FsmGameObject) null;
    this.physicsMaterialName = (FsmString) null;
  }

  private void StoreTriggerInfo()
  {
    if ((Object) this.Fsm.TriggerCollider == (Object) null)
      return;
    this.gameObjectHit.Value = this.Fsm.TriggerCollider.gameObject;
    this.physicsMaterialName.Value = this.Fsm.TriggerCollider.material.name;
  }

  public override void OnEnter()
  {
    this.StoreTriggerInfo();
    this.Finish();
  }
}
