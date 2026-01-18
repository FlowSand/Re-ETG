using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("PlayerPrefs")]
    [Tooltip("Sets the value of the preference identified by key.")]
    public class PlayerPrefsSetFloat : FsmStateAction
    {
        [CompoundArray("Count", "Key", "Value")]
        [Tooltip("Case sensitive key.")]
        public FsmString[] keys;
        public FsmFloat[] values;

        public override void Reset()
        {
            this.keys = new FsmString[1];
            this.values = new FsmFloat[1];
        }

        public override void OnEnter()
        {
            for (int index = 0; index < this.keys.Length; ++index)
            {
                if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
                    PlayerPrefs.SetFloat(this.keys[index].Value, !this.values[index].IsNone ? this.values[index].Value : 0.0f);
            }
            this.Finish();
        }
    }
}
