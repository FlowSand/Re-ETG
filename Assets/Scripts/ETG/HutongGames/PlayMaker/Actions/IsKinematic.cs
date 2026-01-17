// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IsKinematic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigid Body is Kinematic.")]
  public class IsKinematic : ComponentAction<Rigidbody>
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
      this.DoIsKinematic();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIsKinematic();

    private void DoIsKinematic()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      bool isKinematic = this.rigidbody.isKinematic;
      this.store.Value = isKinematic;
      this.Fsm.Event(!isKinematic ? this.falseEvent : this.trueEvent);
    }
  }
}
