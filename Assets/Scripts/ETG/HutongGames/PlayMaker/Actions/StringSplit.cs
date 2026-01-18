#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.String)]
  [Tooltip("Splits a string into substrings using separator characters.")]
  public class StringSplit : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [Tooltip("String to split.")]
    public FsmString stringToSplit;
    [Tooltip("Characters used to split the string.\nUse '\\n' for newline\nUse '\\t' for tab")]
    public FsmString separators;
    [Tooltip("Remove all leading and trailing white-space characters from each seperated string.")]
    public FsmBool trimStrings;
    [Tooltip("Optional characters used to trim each seperated string.")]
    public FsmString trimChars;
    [UIHint(UIHint.Variable)]
    [ArrayEditor(VariableType.String, "", 0, 0, 65536 /*0x010000*/)]
    [Tooltip("Store the split strings in a String Array.")]
    public FsmArray stringArray;

    public override void Reset()
    {
      this.stringToSplit = (FsmString) null;
      this.separators = (FsmString) null;
      this.trimStrings = (FsmBool) false;
      this.trimChars = (FsmString) null;
      this.stringArray = (FsmArray) null;
    }

    public override void OnEnter()
    {
      char[] charArray = this.trimChars.Value.ToCharArray();
      if (!this.stringToSplit.IsNone && !this.stringArray.IsNone)
      {
        this.stringArray.Values = (object[]) this.stringToSplit.Value.Split(this.separators.Value.ToCharArray());
        if (this.trimStrings.Value)
        {
          for (int index = 0; index < this.stringArray.Values.Length; ++index)
          {
            if (this.stringArray.Values[index] is string str)
            {
              if (!this.trimChars.IsNone && charArray.Length > 0)
                this.stringArray.Set(index, (object) str.Trim(charArray));
              else
                this.stringArray.Set(index, (object) str.Trim());
            }
          }
        }
        this.stringArray.SaveChanges();
      }
      this.Finish();
    }
  }
}
