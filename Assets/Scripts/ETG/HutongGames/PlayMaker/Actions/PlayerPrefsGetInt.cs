using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
    [ActionCategory("PlayerPrefs")]
    public class PlayerPrefsGetInt : FsmStateAction
    {
        [Tooltip("Case sensitive key.")]
        [CompoundArray("Count", "Key", "Variable")]
        public FsmString[] keys;
        [UIHint(UIHint.Variable)]
        public FsmInt[] variables;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.variables = new FsmInt[1];
        }

        public override void OnEnter()
        {
            for (int index = 0; index < this.keys.Length; ++index)
            {
                if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
                    this.variables[index].Value = PlayerPrefs.GetInt(this.keys[index].Value, !this.variables[index].IsNone ? this.variables[index].Value : 0);
            }
            this.Finish();
        }
    }
}
