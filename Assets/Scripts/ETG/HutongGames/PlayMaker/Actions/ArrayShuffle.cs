// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayShuffle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Shuffle values in an array. Optionally set a start index and range to shuffle only part of the array.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayShuffle : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Array to shuffle.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmArray array;
    [HutongGames.PlayMaker.Tooltip("Optional start Index for the shuffling. Leave it to none or 0 for no effect")]
    public FsmInt startIndex;
    [HutongGames.PlayMaker.Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to none or 0 for no effect, it will shuffle the whole array")]
    public FsmInt shufflingRange;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      FsmInt fsmInt1 = new FsmInt();
      fsmInt1.UseVariable = true;
      this.startIndex = fsmInt1;
      FsmInt fsmInt2 = new FsmInt();
      fsmInt2.UseVariable = true;
      this.shufflingRange = fsmInt2;
    }

    public override void OnEnter()
    {
      List<object> objectList = new List<object>((IEnumerable<object>) this.array.Values);
      int min = 0;
      int b = objectList.Count - 1;
      if (this.startIndex.Value > 0)
        min = Mathf.Min(this.startIndex.Value, b);
      if (this.shufflingRange.Value > 0)
        b = Mathf.Min(objectList.Count - 1, min + this.shufflingRange.Value);
      for (int index1 = b; index1 > min; --index1)
      {
        int index2 = Random.Range(min, index1 + 1);
        object obj = objectList[index1];
        objectList[index1] = objectList[index2];
        objectList[index2] = obj;
      }
      this.array.Values = objectList.ToArray();
      this.Finish();
    }
  }
}
