using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector2)]
  [HutongGames.PlayMaker.Tooltip("Use a high pass filter to isolate sudden changes in a Vector2 Variable.")]
  public class Vector2HighPassFilter : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input.")]
    public FsmVector2 vector2Variable;
    [HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have.")]
    public FsmFloat filteringFactor;
    private Vector2 filteredVector;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      this.filteringFactor = (FsmFloat) 0.1f;
    }

    public override void OnEnter()
    {
      this.filteredVector = new Vector2(this.vector2Variable.Value.x, this.vector2Variable.Value.y);
    }

    public override void OnUpdate()
    {
      this.filteredVector.x = this.vector2Variable.Value.x - (float) ((double) this.vector2Variable.Value.x * (double) this.filteringFactor.Value + (double) this.filteredVector.x * (1.0 - (double) this.filteringFactor.Value));
      this.filteredVector.y = this.vector2Variable.Value.y - (float) ((double) this.vector2Variable.Value.y * (double) this.filteringFactor.Value + (double) this.filteredVector.y * (1.0 - (double) this.filteringFactor.Value));
      this.vector2Variable.Value = new Vector2(this.filteredVector.x, this.filteredVector.y);
    }
  }
}
