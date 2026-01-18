#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Select a random Color from an array of Colors.")]
  [ActionCategory(ActionCategory.Color)]
  public class SelectRandomColor : FsmStateAction
  {
    [CompoundArray("Colors", "Color", "Weight")]
    public FsmColor[] colors;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat[] weights;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmColor storeColor;

    public override void Reset()
    {
      this.colors = new FsmColor[3];
      this.weights = new FsmFloat[3]
      {
        (FsmFloat) 1f,
        (FsmFloat) 1f,
        (FsmFloat) 1f
      };
      this.storeColor = (FsmColor) null;
    }

    public override void OnEnter()
    {
      this.DoSelectRandomColor();
      this.Finish();
    }

    private void DoSelectRandomColor()
    {
      if (this.colors == null || this.colors.Length == 0 || this.storeColor == null)
        return;
      int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
      if (randomWeightedIndex == -1)
        return;
      this.storeColor.Value = this.colors[randomWeightedIndex].Value;
    }
  }
}
