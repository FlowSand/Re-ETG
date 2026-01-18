using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets the value of a String Variable, based upon GungeonFlags.")]
    [ActionCategory(ActionCategory.String)]
    public class CompileStringFromSaveFlags : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString stringVariable;
        public FsmString[] stringComponents;
        public GungeonFlags[] flagComponents;
        public FsmBool[] valueComponents;

        public override void Reset()
        {
            this.stringVariable = (FsmString) null;
            this.stringComponents = new FsmString[0];
            this.flagComponents = new GungeonFlags[0];
            this.valueComponents = new FsmBool[0];
        }

        public override void OnEnter()
        {
            this.DoSetStringValue();
            this.Finish();
        }

        private void DoSetStringValue()
        {
            if (this.stringVariable == null)
                return;
            List<string> stringList = new List<string>();
            for (int index = 0; index < this.stringComponents.Length; ++index)
            {
                if (this.flagComponents[index] == GungeonFlags.NONE || GameStatsManager.Instance.GetFlag(this.flagComponents[index]) == this.valueComponents[index].Value)
                    stringList.Add(StringTableManager.GetString(this.stringComponents[index].Value));
            }
            string str1 = string.Empty;
            if (stringList.Count > 0)
            {
                char[] charArray = (str1 + stringList[0]).ToCharArray();
                charArray[0] = char.ToUpper(charArray[0]);
                string str2 = new string(charArray);
                for (int index = 1; index < stringList.Count; ++index)
                {
                    if (stringList.Count == 2)
                        str2 = $"{str2} {StringTableManager.GetString("#AND")} {stringList[index]}";
                    else if (index == stringList.Count - 1)
                        str2 = $"{str2}, {StringTableManager.GetString("#AND")} {stringList[index]}";
                    else
                        str2 = $"{str2}, {stringList[index]}";
                }
                str1 = str2 + ".";
            }
            this.stringVariable.Value = str1;
        }
    }
}
