#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Select a Random String from an array of Strings.")]
  [ActionCategory(ActionCategory.String)]
  public class SelectRandomString : FsmStateAction
  {
    [CompoundArray("Strings", "String", "Weight")]
    public FsmString[] strings;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat[] weights;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString storeString;

    public override void Reset()
    {
      this.strings = new FsmString[3];
      this.weights = new FsmFloat[3]
      {
        (FsmFloat) 1f,
        (FsmFloat) 1f,
        (FsmFloat) 1f
      };
      this.storeString = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoSelectRandomString();
      this.Finish();
    }

    private void DoSelectRandomString()
    {
      if (this.strings == null || this.strings.Length == 0 || this.storeString == null)
        return;
      int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
      if (randomWeightedIndex == -1)
        return;
      this.storeString.Value = this.strings[randomWeightedIndex].Value;
    }
  }
}
