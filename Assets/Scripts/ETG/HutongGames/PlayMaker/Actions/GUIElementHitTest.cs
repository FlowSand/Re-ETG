using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Performs a Hit Test on a Game Object with a GUITexture or GUIText component.")]
  [ActionCategory(ActionCategory.GUIElement)]
  public class GUIElementHitTest : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that has a GUITexture or GUIText component.")]
    [CheckForComponent(typeof (GUIElement))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Specify camera or use MainCamera as default.")]
    public Camera camera;
    [HutongGames.PlayMaker.Tooltip("A vector position on screen. Usually stored by actions like GetTouchInfo, or World To Screen Point.")]
    public FsmVector3 screenPoint;
    [HutongGames.PlayMaker.Tooltip("Specify screen X coordinate.")]
    public FsmFloat screenX;
    [HutongGames.PlayMaker.Tooltip("Specify screen Y coordinate.")]
    public FsmFloat screenY;
    [HutongGames.PlayMaker.Tooltip("Whether the specified screen coordinates are normalized (0-1).")]
    public FsmBool normalized;
    [HutongGames.PlayMaker.Tooltip("Event to send if the Hit Test is true.")]
    public FsmEvent hitEvent;
    [HutongGames.PlayMaker.Tooltip("Store the result of the Hit Test in a bool variable (true/false).")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you want to wait for the hit test to return true.")]
    public FsmBool everyFrame;
    private GUIElement guiElement;
    private GameObject gameObjectCached;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.camera = (Camera) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.screenPoint = fsmVector3;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.screenX = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.screenY = fsmFloat2;
      this.normalized = (FsmBool) true;
      this.hitEvent = (FsmEvent) null;
      this.everyFrame = (FsmBool) true;
    }

    public override void OnEnter()
    {
      this.DoHitTest();
      if (this.everyFrame.Value)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoHitTest();

    private void DoHitTest()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.gameObjectCached)
      {
        this.guiElement = (GUIElement) ownerDefaultTarget.GetComponent<GUITexture>() ?? (GUIElement) ownerDefaultTarget.GetComponent<GUIText>();
        this.gameObjectCached = ownerDefaultTarget;
      }
      if ((Object) this.guiElement == (Object) null)
      {
        this.Finish();
      }
      else
      {
        Vector3 screenPosition = !this.screenPoint.IsNone ? this.screenPoint.Value : new Vector3(0.0f, 0.0f);
        if (!this.screenX.IsNone)
          screenPosition.x = this.screenX.Value;
        if (!this.screenY.IsNone)
          screenPosition.y = this.screenY.Value;
        if (this.normalized.Value)
        {
          screenPosition.x *= (float) Screen.width;
          screenPosition.y *= (float) Screen.height;
        }
        if (this.guiElement.HitTest(screenPosition, this.camera))
        {
          this.storeResult.Value = true;
          this.Fsm.Event(this.hitEvent);
        }
        else
          this.storeResult.Value = false;
      }
    }
  }
}
