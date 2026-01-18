#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets a flag on the player's save data.")]
    [ActionCategory(".Brave")]
    public class SetCharacterSpecificSaveFlag : FsmStateAction
    {
        [Tooltip("The flag.")]
        public CharacterSpecificGungeonFlags targetFlag;
        [Tooltip("The value to set the flag to.")]
        public FsmBool value;

        public override string ErrorCheck()
        {
            string empty = string.Empty;
            if (this.targetFlag == CharacterSpecificGungeonFlags.NONE)
                empty += "Target flag is NONE. This is a mistake.";
            return empty;
        }

        public override void Reset()
        {
            this.targetFlag = CharacterSpecificGungeonFlags.NONE;
            this.value = (FsmBool) false;
        }

        public override void OnEnter()
        {
            GameStatsManager.Instance.SetCharacterSpecificFlag(this.targetFlag, this.value.Value);
            this.Finish();
        }
    }
}
