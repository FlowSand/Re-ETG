// Decompiled with JetBrains decompiler
// Type: InControl.PlayerTwoAxisAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace InControl;

public class PlayerTwoAxisAction : TwoAxisInputControl
{
  private PlayerAction negativeXAction;
  private PlayerAction positiveXAction;
  private PlayerAction negativeYAction;
  private PlayerAction positiveYAction;
  public BindingSourceType LastInputType;

  internal PlayerTwoAxisAction(
    PlayerAction negativeXAction,
    PlayerAction positiveXAction,
    PlayerAction negativeYAction,
    PlayerAction positiveYAction)
  {
    this.negativeXAction = negativeXAction;
    this.positiveXAction = positiveXAction;
    this.negativeYAction = negativeYAction;
    this.positiveYAction = positiveYAction;
    this.InvertXAxis = false;
    this.InvertYAxis = false;
    this.Raw = true;
  }

  public bool InvertXAxis { get; set; }

  public bool InvertYAxis { get; set; }

  public event Action<BindingSourceType> OnLastInputTypeChanged;

  public object UserData { get; set; }

  internal void Update(ulong updateTick, float deltaTime)
  {
    this.ProcessActionUpdate(this.negativeXAction);
    this.ProcessActionUpdate(this.positiveXAction);
    this.ProcessActionUpdate(this.negativeYAction);
    this.ProcessActionUpdate(this.positiveYAction);
    this.UpdateWithAxes(Utility.ValueFromSides((float) (OneAxisInputControl) this.negativeXAction, (float) (OneAxisInputControl) this.positiveXAction, this.InvertXAxis), Utility.ValueFromSides((float) (OneAxisInputControl) this.negativeYAction, (float) (OneAxisInputControl) this.positiveYAction, InputManager.InvertYAxis || this.InvertYAxis), updateTick, deltaTime);
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
