#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Each time this action is called it gets the next item from a Array. \nThis lets you quickly loop through all the items of an array to perform actions on them.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayGetNext : FsmStateAction
  {
    [Tooltip("The Array Variable to use.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;
    [Tooltip("From where to start iteration, leave as 0 to start from the beginning")]
    public FsmInt startIndex;
    [Tooltip("When to end iteration, leave as 0 to iterate until the end")]
    public FsmInt endIndex;
    [Tooltip("Event to send to get the next item.")]
    public FsmEvent loopEvent;
    [Tooltip("Event to send when there are no more items.")]
    public FsmEvent finishedEvent;
    [MatchElementType("array")]
    [ActionSection("Result")]
    public FsmVar result;
    [UIHint(UIHint.Variable)]
    public FsmInt currentIndex;
    private int nextItemIndex;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.startIndex = (FsmInt) null;
      this.endIndex = (FsmInt) null;
      this.currentIndex = (FsmInt) null;
      this.loopEvent = (FsmEvent) null;
      this.finishedEvent = (FsmEvent) null;
      this.result = (FsmVar) null;
    }

    public override void OnEnter()
    {
      if (this.nextItemIndex == 0 && this.startIndex.Value > 0)
        this.nextItemIndex = this.startIndex.Value;
      this.DoGetNextItem();
      this.Finish();
    }

    private void DoGetNextItem()
    {
      if (this.nextItemIndex >= this.array.Length)
      {
        this.nextItemIndex = 0;
        this.currentIndex.Value = this.array.Length - 1;
        this.Fsm.Event(this.finishedEvent);
      }
      else
      {
        this.result.SetValue(this.array.Get(this.nextItemIndex));
        if (this.nextItemIndex >= this.array.Length)
        {
          this.nextItemIndex = 0;
          this.currentIndex.Value = this.array.Length - 1;
          this.Fsm.Event(this.finishedEvent);
        }
        else if (this.endIndex.Value > 0 && this.nextItemIndex >= this.endIndex.Value)
        {
          this.nextItemIndex = 0;
          this.currentIndex.Value = this.endIndex.Value;
          this.Fsm.Event(this.finishedEvent);
        }
        else
        {
          ++this.nextItemIndex;
          this.currentIndex.Value = this.nextItemIndex - 1;
          if (this.loopEvent == null)
            return;
          this.Fsm.Event(this.loopEvent);
        }
      }
    }
  }
}
