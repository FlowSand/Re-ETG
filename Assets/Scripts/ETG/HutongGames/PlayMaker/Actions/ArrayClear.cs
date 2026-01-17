// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayClear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Array)]
  [Tooltip("Sets all items in an Array to their default value: 0, empty string, false, or null depending on their type. Optionally defines a reset value to use.")]
  public class ArrayClear : FsmStateAction
  {
    [Tooltip("The Array Variable to clear.")]
    [UIHint(UIHint.Variable)]
    public FsmArray array;
    [Tooltip("Optional reset value. Leave as None for default value.")]
    [MatchElementType("array")]
    public FsmVar resetValue;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.resetValue = new FsmVar() { useVariable = true };
    }

    public override void OnEnter()
    {
      int length = this.array.Length;
      this.array.Reset();
      this.array.Resize(length);
      if (!this.resetValue.IsNone)
      {
        this.resetValue.UpdateValue();
        object obj = this.resetValue.GetValue();
        for (int index = 0; index < length; ++index)
          this.array.Set(index, obj);
      }
      this.Finish();
    }
  }
}
