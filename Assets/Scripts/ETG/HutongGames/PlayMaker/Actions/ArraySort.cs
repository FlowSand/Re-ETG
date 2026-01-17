// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArraySort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sort items in an Array.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArraySort : FsmStateAction
  {
    [Tooltip("The Array to sort.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;

    public override void Reset() => this.array = (FsmArray) null;

    public override void OnEnter()
    {
      List<object> objectList = new List<object>((IEnumerable<object>) this.array.Values);
      objectList.Sort();
      this.array.Values = objectList.ToArray();
      this.Finish();
    }
  }
}
