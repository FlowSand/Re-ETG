using System;
using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Transfer a value from one array to another, basically a copy/cut paste action on steroids.")]
  [NoActionTargets]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayTransferValue : FsmStateAction
  {
    [RequiredField]
    [Tooltip("The Array Variable source.")]
    [UIHint(UIHint.Variable)]
    public FsmArray arraySource;
    [Tooltip("The Array Variable target.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray arrayTarget;
    [Tooltip("The index to transfer.")]
    [MatchFieldType("array")]
    public FsmInt indexToTransfer;
    [ObjectType(typeof (ArrayTransferValue.ArrayTransferType))]
    [ActionSection("Transfer Options")]
    public FsmEnum copyType;
    [ObjectType(typeof (ArrayTransferValue.ArrayPasteType))]
    public FsmEnum pasteType;
    [ActionSection("Result")]
    [Tooltip("Event sent if this array source does not contains that element (described below)")]
    public FsmEvent indexOutOfRange;

    public override void Reset()
    {
      this.arraySource = (FsmArray) null;
      this.arrayTarget = (FsmArray) null;
      this.indexToTransfer = (FsmInt) null;
      this.copyType = (FsmEnum) (Enum) ArrayTransferValue.ArrayTransferType.Copy;
      this.pasteType = (FsmEnum) (Enum) ArrayTransferValue.ArrayPasteType.AsLastItem;
    }

    public override void OnEnter()
    {
      this.DoTransferValue();
      this.Finish();
    }

    private void DoTransferValue()
    {
      if (this.arraySource.IsNone || this.arrayTarget.IsNone)
        return;
      int index = this.indexToTransfer.Value;
      if (index < 0 || index >= this.arraySource.Length)
      {
        this.Fsm.Event(this.indexOutOfRange);
      }
      else
      {
        object obj = this.arraySource.Values[index];
        if ((ArrayTransferValue.ArrayTransferType) this.copyType.Value == ArrayTransferValue.ArrayTransferType.Cut)
        {
          List<object> objectList = new List<object>((IEnumerable<object>) this.arraySource.Values);
          objectList.RemoveAt(index);
          this.arraySource.Values = objectList.ToArray();
        }
        else if ((ArrayTransferValue.ArrayTransferType) this.copyType.Value == ArrayTransferValue.ArrayTransferType.nullify)
          this.arraySource.Values.SetValue((object) null, index);
        if ((ArrayTransferValue.ArrayPasteType) this.pasteType.Value == ArrayTransferValue.ArrayPasteType.AsFirstItem)
        {
          List<object> objectList = new List<object>((IEnumerable<object>) this.arrayTarget.Values);
          objectList.Insert(0, obj);
          this.arrayTarget.Values = objectList.ToArray();
        }
        else if ((ArrayTransferValue.ArrayPasteType) this.pasteType.Value == ArrayTransferValue.ArrayPasteType.AsLastItem)
        {
          this.arrayTarget.Resize(this.arrayTarget.Length + 1);
          this.arrayTarget.Set(this.arrayTarget.Length - 1, obj);
        }
        else if ((ArrayTransferValue.ArrayPasteType) this.pasteType.Value == ArrayTransferValue.ArrayPasteType.InsertAtSameIndex)
        {
          if (index >= this.arrayTarget.Length)
            this.Fsm.Event(this.indexOutOfRange);
          List<object> objectList = new List<object>((IEnumerable<object>) this.arrayTarget.Values);
          objectList.Insert(index, obj);
          this.arrayTarget.Values = objectList.ToArray();
        }
        else
        {
          if ((ArrayTransferValue.ArrayPasteType) this.pasteType.Value != ArrayTransferValue.ArrayPasteType.ReplaceAtSameIndex)
            return;
          if (index >= this.arrayTarget.Length)
            this.Fsm.Event(this.indexOutOfRange);
          else
            this.arrayTarget.Set(index, obj);
        }
      }
    }

    public enum ArrayTransferType
    {
      Copy,
      Cut,
      nullify,
    }

    public enum ArrayPasteType
    {
      AsFirstItem,
      AsLastItem,
      InsertAtSameIndex,
      ReplaceAtSameIndex,
    }
  }
}
