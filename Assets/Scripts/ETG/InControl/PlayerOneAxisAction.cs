using System;

#nullable disable
namespace InControl
{
  public class PlayerOneAxisAction : OneAxisInputControl
  {
    private PlayerAction negativeAction;
    private PlayerAction positiveAction;
    public BindingSourceType LastInputType;

    internal PlayerOneAxisAction(PlayerAction negativeAction, PlayerAction positiveAction)
    {
      this.negativeAction = negativeAction;
      this.positiveAction = positiveAction;
      this.Raw = true;
    }

    public event Action<BindingSourceType> OnLastInputTypeChanged;

    public object UserData { get; set; }

    internal void Update(ulong updateTick, float deltaTime)
    {
      this.ProcessActionUpdate(this.negativeAction);
      this.ProcessActionUpdate(this.positiveAction);
      this.CommitWithValue(Utility.ValueFromSides((float) (OneAxisInputControl) this.negativeAction, (float) (OneAxisInputControl) this.positiveAction), updateTick, deltaTime);
    }

    private void ProcessActionUpdate(PlayerAction action)
    {
      BindingSourceType lastInputType = this.LastInputType;
      if (action.UpdateTick > this.UpdateTick)
      {
        this.UpdateTick = action.UpdateTick;
        lastInputType = action.LastInputType;
      }
      if (this.LastInputType == lastInputType)
        return;
      this.LastInputType = lastInputType;
      if (this.OnLastInputTypeChanged == null)
        return;
      this.OnLastInputTypeChanged(lastInputType);
    }

    [Obsolete("Please set this property on device controls directly. It does nothing here.")]
    public new float LowerDeadZone
    {
      get => 0.0f;
      set
      {
      }
    }

    [Obsolete("Please set this property on device controls directly. It does nothing here.")]
    public new float UpperDeadZone
    {
      get => 0.0f;
      set
      {
      }
    }
  }
}
