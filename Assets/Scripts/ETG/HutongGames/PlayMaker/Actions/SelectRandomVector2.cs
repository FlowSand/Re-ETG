#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Select a Random Vector2 from a Vector2 array.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class SelectRandomVector2 : FsmStateAction
  {
    [CompoundArray("Vectors", "Vector", "Weight")]
    [Tooltip("The array of Vectors and respective weights")]
    public FsmVector2[] vector2Array;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat[] weights;
    [RequiredField]
    [Tooltip("The picked vector2")]
    [UIHint(UIHint.Variable)]
    public FsmVector2 storeVector2;

    public override void Reset()
    {
      this.vector2Array = new FsmVector2[3];
      this.weights = new FsmFloat[3]
      {
        (FsmFloat) 1f,
        (FsmFloat) 1f,
        (FsmFloat) 1f
      };
      this.storeVector2 = (FsmVector2) null;
    }

    public override void OnEnter()
    {
      this.DoSelectRandomColor();
      this.Finish();
    }

    private void DoSelectRandomColor()
    {
      if (this.vector2Array == null || this.vector2Array.Length == 0 || this.storeVector2 == null)
        return;
      int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
      if (randomWeightedIndex == -1)
        return;
      this.storeVector2.Value = this.vector2Array[randomWeightedIndex].Value;
    }
  }
}
