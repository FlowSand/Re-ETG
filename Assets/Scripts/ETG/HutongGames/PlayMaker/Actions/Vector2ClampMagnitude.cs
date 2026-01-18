using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Clamps the Magnitude of Vector2 Variable.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2ClampMagnitude : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Vector2")]
    [UIHint(UIHint.Variable)]
    public FsmVector2 vector2Variable;
    [HutongGames.PlayMaker.Tooltip("The maximum Magnitude")]
    [RequiredField]
    public FsmFloat maxLength;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      this.maxLength = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoVector2ClampMagnitude();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector2ClampMagnitude();

    private void DoVector2ClampMagnitude()
    {
      this.vector2Variable.Value = Vector2.ClampMagnitude(this.vector2Variable.Value, this.maxLength.Value);
    }
  }
}
