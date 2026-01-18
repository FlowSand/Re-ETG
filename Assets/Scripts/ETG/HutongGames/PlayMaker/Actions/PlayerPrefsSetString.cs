using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets the value of the preference identified by key.")]
    [ActionCategory("PlayerPrefs")]
    public class PlayerPrefsSetString : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        public FsmString[] values;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.values = new FsmString[1];
        }

        public override void OnEnter()
        {
            for (int index = 0; index < this.keys.Length; ++index)
            {
                if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
                    PlayerPrefs.SetString(this.keys[index].Value, !this.values[index].IsNone ? this.values[index].Value : string.Empty);
            }
            this.Finish();
        }
    }
}
