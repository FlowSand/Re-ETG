// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GameObjectIsVisible
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Tests if a Game Object is visible.")]
[ActionTarget(typeof (GameObject), "gameObject", false)]
[ActionCategory(ActionCategory.Logic)]
public class GameObjectIsVisible : ComponentAction<Renderer>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
  [CheckForComponent(typeof (Renderer))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is visible.")]
  public FsmEvent trueEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if the GameObject is NOT visible.")]
  public FsmEvent falseEvent;
  [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.trueEvent = (FsmEvent) null;
    this.falseEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoIsVisible();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoIsVisible();

  private void DoIsVisible()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    bool isVisible = this.renderer.isVisible;
    this.storeResult.Value = isVisible;
    this.Fsm.Event(!isVisible ? this.falseEvent : this.trueEvent);
  }
}
