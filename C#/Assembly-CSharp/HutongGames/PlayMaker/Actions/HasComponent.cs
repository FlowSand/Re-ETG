// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.HasComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[HutongGames.PlayMaker.Tooltip("Checks if an Object has a Component. Optionally remove the Component on exiting the state.")]
public class HasComponent : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.ScriptComponent)]
  [RequiredField]
  public FsmString component;
  public FsmBool removeOnExit;
  public FsmEvent trueEvent;
  public FsmEvent falseEvent;
  [UIHint(UIHint.Variable)]
  public FsmBool store;
  public bool everyFrame;
  private Component aComponent;

  public override void Reset()
  {
    this.aComponent = (Component) null;
    this.gameObject = (FsmOwnerDefault) null;
    this.trueEvent = (FsmEvent) null;
    this.falseEvent = (FsmEvent) null;
    this.component = (FsmString) null;
    this.store = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoHasComponent(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate()
  {
    this.DoHasComponent(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
  }

  public override void OnExit()
  {
    if (!this.removeOnExit.Value || !((Object) this.aComponent != (Object) null))
      return;
    Object.Destroy((Object) this.aComponent);
  }

  private void DoHasComponent(GameObject go)
  {
    if ((Object) go == (Object) null)
    {
      if (!this.store.IsNone)
        this.store.Value = false;
      this.Fsm.Event(this.falseEvent);
    }
    else
    {
      this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
      if (!this.store.IsNone)
        this.store.Value = (Object) this.aComponent != (Object) null;
      this.Fsm.Event(!((Object) this.aComponent != (Object) null) ? this.falseEvent : this.trueEvent);
    }
  }
}
