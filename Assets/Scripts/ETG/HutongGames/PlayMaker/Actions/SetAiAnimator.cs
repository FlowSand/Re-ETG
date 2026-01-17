// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAiAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [HutongGames.PlayMaker.Tooltip("Handles updating an AIAnimator.")]
  public class SetAiAnimator : FsmStateAction
  {
    public FsmOwnerDefault GameObject;
    public SetAiAnimator.Mode mode;
    [HutongGames.PlayMaker.Tooltip("Name of the new default animation state (Directional Animations only).  Leave blank to return to the default (idle/base).")]
    public FsmString baseAnimName;

    public override void Reset()
    {
      this.GameObject = (FsmOwnerDefault) null;
      this.mode = SetAiAnimator.Mode.SetBaseAnim;
      this.baseAnimName = (FsmString) string.Empty;
    }

    public override string ErrorCheck()
    {
      string str = string.Empty;
      UnityEngine.GameObject gameObject = this.GameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.GameObject.GameObject.Value : this.Owner;
      if ((bool) (Object) gameObject)
      {
        AIAnimator component = gameObject.GetComponent<AIAnimator>();
        if (!(bool) (Object) component)
          return "Requires an AI Animator.\n";
        if (this.mode == SetAiAnimator.Mode.SetBaseAnim && this.baseAnimName.Value != string.Empty && !component.HasDirectionalAnimation(this.baseAnimName.Value))
          str = $"{str}Unknown animation {this.baseAnimName.Value}.\n";
      }
      else if (!this.GameObject.GameObject.UseVariable)
        return "No object specified";
      return str;
    }

    public override void OnEnter()
    {
      AIAnimator component = this.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<AIAnimator>();
      if (this.mode == SetAiAnimator.Mode.SetBaseAnim)
      {
        if (this.baseAnimName.Value == string.Empty)
          component.ClearBaseAnim();
        else
          component.SetBaseAnim(this.baseAnimName.Value);
      }
      this.Finish();
    }

    public enum Mode
    {
      SetBaseAnim,
    }
  }
}
