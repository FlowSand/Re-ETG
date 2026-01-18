using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Checkbox")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_checkbox.html")]
[dfCategory("Basic Controls")]
[ExecuteInEditMode]
[dfTooltip("Implements a standard checkbox (or toggle) control")]
[Serializable]
public class dfCheckbox : dfControl
  {
    [SerializeField]
    protected bool isChecked;
    [SerializeField]
    protected dfControl checkIcon;
    [SerializeField]
    protected dfLabel label;
    [SerializeField]
    protected dfControl group;
    [SerializeField]
    protected bool clickWhenSpacePressed = true;

    public event PropertyChangedEventHandler<bool> CheckChanged;

    public bool ClickWhenSpacePressed
    {
      get => this.clickWhenSpacePressed;
      set => this.clickWhenSpacePressed = value;
    }

    public bool IsChecked
    {
      get => this.isChecked;
      set
      {
        if (value == this.isChecked)
          return;
        this.isChecked = value;
        this.OnCheckChanged();
        if (!value || !((UnityEngine.Object) this.group != (UnityEngine.Object) null))
          return;
        this.handleGroupedCheckboxChecked();
      }
    }

    public dfControl CheckIcon
    {
      get => this.checkIcon;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.checkIcon))
          return;
        this.checkIcon = value;
        this.Invalidate();
      }
    }

    public dfLabel Label
    {
      get => this.label;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.label))
          return;
        this.label = value;
        this.Invalidate();
      }
    }

    public dfControl GroupContainer
    {
      get => this.group;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.group))
          return;
        this.group = value;
        this.Invalidate();
      }
    }

    public string Text
    {
      get => (UnityEngine.Object) this.label != (UnityEngine.Object) null ? this.label.Text : "[LABEL NOT SET]";
      set
      {
        if (!((UnityEngine.Object) this.label != (UnityEngine.Object) null))
          return;
        this.label.Text = value;
      }
    }

    public override bool CanFocus => this.IsEnabled && this.IsVisible;

    public override void Start()
    {
      base.Start();
      if (!((UnityEngine.Object) this.checkIcon != (UnityEngine.Object) null))
        return;
      this.checkIcon.BringToFront();
      this.checkIcon.IsVisible = this.IsChecked;
    }

    protected internal override void OnKeyPress(dfKeyEventArgs args)
    {
      if (this.ClickWhenSpacePressed && this.IsInteractive && args.KeyCode == KeyCode.Space)
        this.OnClick(new dfMouseEventArgs((dfControl) this, dfMouseButtons.Left, 1, new Ray(), Vector2.zero, 0.0f));
      else
        base.OnKeyPress(args);
    }

    protected internal override void OnClick(dfMouseEventArgs args)
    {
      base.OnClick(args);
      if (!this.IsInteractive)
        return;
      if ((UnityEngine.Object) this.group == (UnityEngine.Object) null)
        this.IsChecked = !this.IsChecked;
      else
        this.handleGroupedCheckboxChecked();
      args.Use();
    }

    protected internal void OnCheckChanged()
    {
      this.SignalHierarchy(nameof (OnCheckChanged), (object) this, (object) this.isChecked);
      if (this.CheckChanged != null)
        this.CheckChanged((dfControl) this, this.isChecked);
      if (!((UnityEngine.Object) this.checkIcon != (UnityEngine.Object) null))
        return;
      if (this.IsChecked)
        this.checkIcon.BringToFront();
      this.checkIcon.IsVisible = this.IsChecked;
    }

    private void handleGroupedCheckboxChecked()
    {
      if ((UnityEngine.Object) this.group == (UnityEngine.Object) null)
        return;
      foreach (dfCheckbox componentsInChild in this.group.transform.GetComponentsInChildren<dfCheckbox>())
      {
        if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) this && (UnityEngine.Object) componentsInChild.GroupContainer == (UnityEngine.Object) this.GroupContainer && componentsInChild.IsChecked)
          componentsInChild.IsChecked = false;
      }
      this.IsChecked = true;
    }
  }

