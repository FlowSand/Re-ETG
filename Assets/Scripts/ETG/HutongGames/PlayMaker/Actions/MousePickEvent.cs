// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MousePickEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Use Ray Distance to set how close the camera must be to pick the object.\n\nNOTE: Picking uses the Main Camera.")]
[ActionTarget(typeof (UnityEngine.GameObject), "GameObject", false)]
[ActionCategory(ActionCategory.Input)]
public class MousePickEvent : FsmStateAction
{
  [CheckForComponent(typeof (Collider))]
  public FsmOwnerDefault GameObject;
  [HutongGames.PlayMaker.Tooltip("Length of the ray to cast from the camera.")]
  public FsmFloat rayDistance = (FsmFloat) 100f;
  [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is over the GameObject.")]
  public FsmEvent mouseOver;
  [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
  public FsmEvent mouseDown;
  [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is released while over the GameObject.")]
  public FsmEvent mouseUp;
  [HutongGames.PlayMaker.Tooltip("Event to send when the mouse moves off the GameObject.")]
  public FsmEvent mouseOff;
  [UIHint(UIHint.Layer)]
  [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
  public FsmInt[] layerMask;
  [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
  public FsmBool invertMask;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.GameObject = (FsmOwnerDefault) null;
    this.rayDistance = (FsmFloat) 100f;
    this.mouseOver = (FsmEvent) null;
    this.mouseDown = (FsmEvent) null;
    this.mouseUp = (FsmEvent) null;
    this.mouseOff = (FsmEvent) null;
    this.layerMask = new FsmInt[0];
    this.invertMask = (FsmBool) false;
    this.everyFrame = true;
  }

  public override void OnEnter()
  {
    this.DoMousePickEvent();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoMousePickEvent();

  private void DoMousePickEvent()
  {
    bool flag = this.DoRaycast();
    this.Fsm.RaycastHitInfo = ActionHelpers.mousePickInfo;
    if (flag)
    {
      if (this.mouseDown != null && Input.GetMouseButtonDown(0))
        this.Fsm.Event(this.mouseDown);
      if (this.mouseOver != null)
        this.Fsm.Event(this.mouseOver);
      if (this.mouseUp == null || !Input.GetMouseButtonUp(0))
        return;
      this.Fsm.Event(this.mouseUp);
    }
    else
    {
      if (this.mouseOff == null)
        return;
      this.Fsm.Event(this.mouseOff);
    }
  }

  private bool DoRaycast()
  {
    return ActionHelpers.IsMouseOver(this.GameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.GameObject.GameObject.Value : this.Owner, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
  }

  public override string ErrorCheck()
  {
    return string.Empty + ActionHelpers.CheckRayDistance(this.rayDistance.Value) + ActionHelpers.CheckPhysicsSetup(this.GameObject);
  }
}
