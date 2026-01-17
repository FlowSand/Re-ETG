// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IsKinematic2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigid Body 2D is Kinematic.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class IsKinematic2d : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("the GameObject with a Rigidbody2D attached")]
    [CheckForComponent(typeof (Rigidbody2D))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Event Sent if Kinematic")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event sent if not Kinematic")]
    public FsmEvent falseEvent;
    [HutongGames.PlayMaker.Tooltip("Store the Kinematic state")]
    [UIHint(UIHint.Variable)]
    public FsmBool store;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
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
      bool isKinematic = this.rigidbody2d.isKinematic;
      this.store.Value = isKinematic;
      this.Fsm.Event(!isKinematic ? this.falseEvent : this.trueEvent);
    }
  }
}
