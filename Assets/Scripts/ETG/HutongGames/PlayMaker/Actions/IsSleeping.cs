// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IsSleeping
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigid Body is sleeping.")]
  [ActionCategory(ActionCategory.Physics)]
  public class IsSleeping : ComponentAction<Rigidbody>
  {
    [RequiredField]
    [CheckForComponent(typeof (Rigidbody))]
    public FsmOwnerDefault gameObject;
    public FsmEvent trueEvent;
    public FsmEvent falseEvent;
    [UIHint(UIHint.Variable)]
    public FsmBool store;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.trueEvent = (FsmEvent) null;
      this.falseEvent = (FsmEvent) null;
      this.store = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoIsSleeping();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIsSleeping();

    private void DoIsSleeping()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      bool flag = this.rigidbody.IsSleeping();
      this.store.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
