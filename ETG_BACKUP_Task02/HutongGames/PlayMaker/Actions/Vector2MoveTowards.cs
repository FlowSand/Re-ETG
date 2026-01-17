// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector2MoveTowards
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Moves a Vector2 towards a Target. Optionally sends an event when successful.")]
[ActionCategory(ActionCategory.Vector2)]
public class Vector2MoveTowards : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Vector2 to Move")]
  [RequiredField]
  public FsmVector2 source;
  [HutongGames.PlayMaker.Tooltip("A target Vector2 to move towards.")]
  public FsmVector2 target;
  [HasFloatSlider(0.0f, 20f)]
  [HutongGames.PlayMaker.Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
  public FsmFloat maxSpeed;
  [HutongGames.PlayMaker.Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
  [HasFloatSlider(0.0f, 5f)]
  public FsmFloat finishDistance;
  [HutongGames.PlayMaker.Tooltip("Event to send when the Finish Distance is reached.")]
  public FsmEvent finishEvent;

  public override void Reset()
  {
    this.source = (FsmVector2) null;
    this.target = (FsmVector2) null;
    this.maxSpeed = (FsmFloat) 10f;
    this.finishDistance = (FsmFloat) 1f;
    this.finishEvent = (FsmEvent) null;
  }

  public override void OnUpdate() => this.DoMoveTowards();

  private void DoMoveTowards()
  {
    this.source.Value = Vector2.MoveTowards(this.source.Value, this.target.Value, this.maxSpeed.Value * Time.deltaTime);
    if ((double) (this.source.Value - this.target.Value).magnitude >= (double) this.finishDistance.Value)
      return;
    this.Fsm.Event(this.finishEvent);
    this.Finish();
  }
}
