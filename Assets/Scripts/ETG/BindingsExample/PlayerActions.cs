using InControl;
using System;
using UnityEngine;

#nullable disable
namespace BindingsExample
{
  public class PlayerActions : PlayerActionSet
  {
    public PlayerAction Fire;
    public PlayerAction Jump;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Move;

    public PlayerActions()
    {
      this.Fire = this.CreatePlayerAction(nameof (Fire));
      this.Jump = this.CreatePlayerAction(nameof (Jump));
      this.Left = this.CreatePlayerAction("Move Left");
      this.Right = this.CreatePlayerAction("Move Right");
      this.Up = this.CreatePlayerAction("Move Up");
      this.Down = this.CreatePlayerAction("Move Down");
      this.Move = this.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
    }

    public static PlayerActions CreateWithDefaultBindings()
    {
      PlayerActions withDefaultBindings = new PlayerActions();
      withDefaultBindings.Fire.AddDefaultBinding(Key.A);
      withDefaultBindings.Fire.AddDefaultBinding(InputControlType.Action1);
      withDefaultBindings.Fire.AddDefaultBinding(Mouse.LeftButton);
      withDefaultBindings.Jump.AddDefaultBinding(Key.Space);
      withDefaultBindings.Jump.AddDefaultBinding(InputControlType.Action3);
      withDefaultBindings.Jump.AddDefaultBinding(InputControlType.Back);
      withDefaultBindings.Up.AddDefaultBinding(Key.UpArrow);
      withDefaultBindings.Down.AddDefaultBinding(Key.DownArrow);
      withDefaultBindings.Left.AddDefaultBinding(Key.LeftArrow);
      withDefaultBindings.Right.AddDefaultBinding(Key.RightArrow);
      withDefaultBindings.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
      withDefaultBindings.Right.AddDefaultBinding(InputControlType.LeftStickRight);
      withDefaultBindings.Up.AddDefaultBinding(InputControlType.LeftStickUp);
      withDefaultBindings.Down.AddDefaultBinding(InputControlType.LeftStickDown);
      withDefaultBindings.Left.AddDefaultBinding(InputControlType.DPadLeft);
      withDefaultBindings.Right.AddDefaultBinding(InputControlType.DPadRight);
      withDefaultBindings.Up.AddDefaultBinding(InputControlType.DPadUp);
      withDefaultBindings.Down.AddDefaultBinding(InputControlType.DPadDown);
      withDefaultBindings.Up.AddDefaultBinding(Mouse.PositiveY);
      withDefaultBindings.Down.AddDefaultBinding(Mouse.NegativeY);
      withDefaultBindings.Left.AddDefaultBinding(Mouse.NegativeX);
      withDefaultBindings.Right.AddDefaultBinding(Mouse.PositiveX);
      withDefaultBindings.ListenOptions.IncludeUnknownControllers = true;
      withDefaultBindings.ListenOptions.MaxAllowedBindings = 4U;
      withDefaultBindings.ListenOptions.UnsetDuplicateBindingsOnSet = true;
      withDefaultBindings.ListenOptions.OnBindingFound = (Func<PlayerAction, BindingSource, bool>) ((action, binding) =>
      {
        if (!(binding == (BindingSource) new KeyBindingSource(new Key[1]
        {
          Key.Escape
        })))
          return true;
        action.StopListeningForBinding();
        return false;
      });
      withDefaultBindings.ListenOptions.OnBindingAdded += (Action<PlayerAction, BindingSource>) ((action, binding) => Debug.Log((object) $"Binding added... {binding.DeviceName}: {binding.Name}"));
      withDefaultBindings.ListenOptions.OnBindingRejected += (Action<PlayerAction, BindingSource, BindingSourceRejectionType>) ((action, binding, reason) => Debug.Log((object) ("Binding rejected... " + (object) reason)));
      return withDefaultBindings;
    }
  }
}
