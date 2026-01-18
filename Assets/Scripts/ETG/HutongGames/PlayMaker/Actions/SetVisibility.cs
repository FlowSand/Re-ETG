using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the visibility of a GameObject. Note: this action sets the GameObject Renderer's enabled state.")]
  [ActionCategory(ActionCategory.Material)]
  public class SetVisibility : ComponentAction<Renderer>
  {
    [RequiredField]
    [CheckForComponent(typeof (Renderer))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Should the object visibility be toggled?\nHas priority over the 'visible' setting")]
    public FsmBool toggle;
    [HutongGames.PlayMaker.Tooltip("Should the object be set to visible or invisible?")]
    public FsmBool visible;
    [HutongGames.PlayMaker.Tooltip("Resets to the initial visibility when it leaves the state")]
    public bool resetOnExit;
    private bool initialVisibility;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.toggle = (FsmBool) false;
      this.visible = (FsmBool) false;
      this.resetOnExit = true;
      this.initialVisibility = false;
    }

    public override void OnEnter()
    {
      this.DoSetVisibility(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
      this.Finish();
    }

    private void DoSetVisibility(GameObject go)
    {
      if (!this.UpdateCache(go))
        return;
      this.initialVisibility = this.renderer.enabled;
      if (!this.toggle.Value)
        this.renderer.enabled = this.visible.Value;
      else
        this.renderer.enabled = !this.renderer.enabled;
    }

    public override void OnExit()
    {
      if (!this.resetOnExit)
        return;
      this.ResetVisibility();
    }

    private void ResetVisibility()
    {
      if (!((Object) this.renderer != (Object) null))
        return;
      this.renderer.enabled = this.initialVisibility;
    }
  }
}
