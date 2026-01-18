using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Multiplies a Vector2 variable by Time.deltaTime. Useful for frame rate independent motion.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2PerSecond : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Vector2")]
    [UIHint(UIHint.Variable)]
    public FsmVector2 vector2Variable;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.vector2Variable.Value *= Time.deltaTime;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector2Variable.Value *= Time.deltaTime;
  }
}
