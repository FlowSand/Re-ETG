using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds a value to Vector2 Variable.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2Add : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The vector2 target")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector2 vector2Variable;
    [HutongGames.PlayMaker.Tooltip("The vector2 to add")]
    [RequiredField]
    public FsmVector2 addVector;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
    public bool everyFrame;
    [HutongGames.PlayMaker.Tooltip("Add the value on a per second bases.")]
    public bool perSecond;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      FsmVector2 fsmVector2 = new FsmVector2();
      fsmVector2.UseVariable = true;
      this.addVector = fsmVector2;
      this.everyFrame = false;
      this.perSecond = false;
    }

    public override void OnEnter()
    {
      this.DoVector2Add();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector2Add();

    private void DoVector2Add()
    {
      if (this.perSecond)
        this.vector2Variable.Value += this.addVector.Value * Time.deltaTime;
      else
        this.vector2Variable.Value += this.addVector.Value;
    }
  }
}
