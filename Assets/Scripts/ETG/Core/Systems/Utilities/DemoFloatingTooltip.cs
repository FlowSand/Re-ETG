using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Tooltip/Floating Tooltip")]
public class DemoFloatingTooltip : MonoBehaviour
  {
    public float tooltipDelay = 1f;
    private dfLabel _tooltip;
    private dfControl lastControl;
    private float tooltipDelayStart;

    public void Start()
    {
      this._tooltip = this.GetComponent<dfLabel>();
      this._tooltip.IsInteractive = false;
      this._tooltip.IsEnabled = false;
    }

    public void Update()
    {
      dfControl controlUnderMouse = dfInputManager.ControlUnderMouse;
      if ((Object) controlUnderMouse == (Object) null)
        this._tooltip.Hide();
      else if ((Object) controlUnderMouse != (Object) this.lastControl)
      {
        this.tooltipDelayStart = UnityEngine.Time.realtimeSinceStartup;
        if (string.IsNullOrEmpty(controlUnderMouse.Tooltip))
          this._tooltip.Hide();
        else
          this._tooltip.Text = controlUnderMouse.Tooltip;
      }
      else if ((Object) this.lastControl != (Object) null && !string.IsNullOrEmpty(this.lastControl.Tooltip) && (double) UnityEngine.Time.realtimeSinceStartup - (double) this.tooltipDelayStart > (double) this.tooltipDelay)
      {
        this._tooltip.Show();
        this._tooltip.BringToFront();
      }
      if (this._tooltip.IsVisible)
        this.setPosition((Vector2) Input.mousePosition);
      this.lastControl = controlUnderMouse;
    }

    private void setPosition(Vector2 position)
    {
      Vector2 vector2 = new Vector2(0.0f, this._tooltip.Height + 25f);
      dfGUIManager manager = this._tooltip.GetManager();
      position = manager.ScreenToGui(position) - vector2;
      if ((double) position.y < 0.0)
        position.y = 0.0f;
      if ((double) position.x + (double) this._tooltip.Width > (double) manager.GetScreenSize().x)
        position.x = manager.GetScreenSize().x - this._tooltip.Width;
      this._tooltip.RelativePosition = (Vector3) position;
    }
  }

