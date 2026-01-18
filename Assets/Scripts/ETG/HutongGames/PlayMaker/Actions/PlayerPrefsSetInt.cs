using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("PlayerPrefs")]
  [Tooltip("Sets the value of the preference identified by key.")]
  public class PlayerPrefsSetInt : FsmStateAction
  {
    [Tooltip("Case sensitive key.")]
    [CompoundArray("Count", "Key", "Value")]
    public FsmString[] keys;
    public FsmInt[] values;

    public override void Reset()
    {
      this.keys = new FsmString[1];
      this.values = new FsmInt[1];
    }

    public override void OnEnter()
    {
      for (int index = 0; index < this.keys.Length; ++index)
      {
        if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
          PlayerPrefs.SetInt(this.keys[index].Value, !this.values[index].IsNone ? this.values[index].Value : 0);
      }
      this.Finish();
    }
  }
}
