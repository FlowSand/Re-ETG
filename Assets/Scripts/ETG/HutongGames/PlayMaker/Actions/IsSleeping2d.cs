using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigidbody 2D is sleeping.")]
  public class IsSleeping2d : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
    [CheckForComponent(typeof (Rigidbody2D))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Event sent if sleeping")]
    public FsmEvent trueEvent;
    [HutongGames.PlayMaker.Tooltip("Event sent if not sleeping")]
    public FsmEvent falseEvent;
    [HutongGames.PlayMaker.Tooltip("Store the value in a Boolean variable")]
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
      bool flag = this.rigidbody2d.IsSleeping();
      this.store.Value = flag;
      this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
    }
  }
}
