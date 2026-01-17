// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Add an item to the end of an Array.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayAdd : FsmStateAction
  {
    [Tooltip("The Array Variable to use.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmArray array;
    [Tooltip("Item to add.")]
    [RequiredField]
    [MatchElementType("array")]
    public FsmVar value;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.value = (FsmVar) null;
    }

    public override void OnEnter()
    {
      this.DoAddValue();
      this.Finish();
    }

    private void DoAddValue()
    {
      this.array.Resize(this.array.Length + 1);
      this.value.UpdateValue();
      this.array.Set(this.array.Length - 1, this.value.GetValue());
    }
  }
}
