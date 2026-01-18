using InControl;
using UnityEngine;

#nullable disable
namespace InterfaceMovement
{
  public class ButtonManager : MonoBehaviour
  {
    public Button focusedButton;
    private TwoAxisInputControl filteredDirection;

    private void Awake()
    {
      this.filteredDirection = new TwoAxisInputControl();
      this.filteredDirection.StateThreshold = 0.5f;
    }

    private void Update()
    {
      this.filteredDirection.Filter(InputManager.ActiveDevice.Direction, Time.deltaTime);
      if (this.filteredDirection.Left.WasRepeated)
        Debug.Log((object) "!!!");
      if (this.filteredDirection.Up.WasPressed)
        this.MoveFocusTo(this.focusedButton.up);
      if (this.filteredDirection.Down.WasPressed)
        this.MoveFocusTo(this.focusedButton.down);
      if (this.filteredDirection.Left.WasPressed)
        this.MoveFocusTo(this.focusedButton.left);
      if (!this.filteredDirection.Right.WasPressed)
        return;
      this.MoveFocusTo(this.focusedButton.right);
    }

    private void MoveFocusTo(Button newFocusedButton)
    {
      if (!((Object) newFocusedButton != (Object) null))
        return;
      this.focusedButton = newFocusedButton;
    }
  }
}
