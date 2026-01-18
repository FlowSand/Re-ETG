using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("GUI base action - don't use!")]
  public abstract class GUIAction : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmRect screenRect;
    public FsmFloat left;
    public FsmFloat top;
    public FsmFloat width;
    public FsmFloat height;
    [RequiredField]
    public FsmBool normalized;
    internal Rect rect;

    public override void Reset()
    {
      this.screenRect = (FsmRect) null;
      this.left = (FsmFloat) 0.0f;
      this.top = (FsmFloat) 0.0f;
      this.width = (FsmFloat) 1f;
      this.height = (FsmFloat) 1f;
      this.normalized = (FsmBool) true;
    }

    public override void OnGUI()
    {
      this.rect = this.screenRect.IsNone ? new Rect() : this.screenRect.Value;
      if (!this.left.IsNone)
        this.rect.x = this.left.Value;
      if (!this.top.IsNone)
        this.rect.y = this.top.Value;
      if (!this.width.IsNone)
        this.rect.width = this.width.Value;
      if (!this.height.IsNone)
        this.rect.height = this.height.Value;
      if (!this.normalized.Value)
        return;
      this.rect.x *= (float) Screen.width;
      this.rect.width *= (float) Screen.width;
      this.rect.y *= (float) Screen.height;
      this.rect.height *= (float) Screen.height;
    }
  }
}
