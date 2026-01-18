using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
    [ActionCategory("PlayerPrefs")]
    public class PlayerPrefsGetString : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Variable")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        [UIHint(UIHint.Variable)]
        public FsmString[] variables;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.variables = new FsmString[1];
        }

        public override void OnEnter()
        {
            for (int index = 0; index < this.keys.Length; ++index)
            {
                if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
                    this.variables[index].Value = PlayerPrefs.GetString(this.keys[index].Value, !this.variables[index].IsNone ? this.variables[index].Value : string.Empty);
            }
            this.Finish();
        }
    }
}
