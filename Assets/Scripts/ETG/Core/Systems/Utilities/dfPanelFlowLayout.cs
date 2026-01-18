using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Panel Addon/Flow Layout")]
[ExecuteInEditMode]
public class dfPanelFlowLayout : MonoBehaviour
  {
    [SerializeField]
    protected RectOffset borderPadding = new RectOffset();
    [SerializeField]
    protected Vector2 itemSpacing = new Vector2();
    [SerializeField]
    protected dfControlOrientation flowDirection;
    [SerializeField]
    protected bool hideClippedControls;
    [SerializeField]
    protected int maxLayoutSize;
    [SerializeField]
    protected List<dfControl> excludedControls = new List<dfControl>();
    private dfPanel panel;

    public dfControlOrientation Direction
    {
      get => this.flowDirection;
      set
      {
        if (value == this.flowDirection)
          return;
        this.flowDirection = value;
        this.PerformLayout();
      }
    }

    public Vector2 ItemSpacing
    {
      get => this.itemSpacing;
      set
      {
        value = Vector2.Max(value, Vector2.zero);
        if (object.Equals((object) value, (object) this.itemSpacing))
          return;
        this.itemSpacing = value;
        this.PerformLayout();
      }
    }

    public RectOffset BorderPadding
    {
      get
      {
        if (this.borderPadding == null)
          this.borderPadding = new RectOffset();
        return this.borderPadding;
      }
      set
      {
        value = value.ConstrainPadding();
        if (object.Equals((object) value, (object) this.borderPadding))
          return;
        this.borderPadding = value;
        this.PerformLayout();
      }
    }

    public bool HideClippedControls
    {
      get => this.hideClippedControls;
      set
      {
        if (value == this.hideClippedControls)
          return;
        this.hideClippedControls = value;
        this.PerformLayout();
      }
    }

    public int MaxLayoutSize
    {
      get => this.maxLayoutSize;
      set
      {
        if (value == this.maxLayoutSize)
          return;
        this.maxLayoutSize = value;
        this.PerformLayout();
      }
    }

    public List<dfControl> ExcludedControls => this.excludedControls;

    public void OnEnable()
    {
      this.panel = this.GetComponent<dfPanel>();
      if ((Object) this.panel == (Object) null)
      {
        Debug.LogError((object) $"The {this.GetType().Name} component requires a dfPanel component.", (Object) this.gameObject);
        this.enabled = false;
      }
      else
        this.panel.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.OnSizeChanged);
    }

    public void OnDisable()
    {
      if (!((Object) this.panel != (Object) null))
        return;
      this.panel.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.OnSizeChanged);
      this.panel = (dfPanel) null;
    }

    public void OnControlAdded(dfControl container, dfControl child)
    {
      child.ZOrderChanged += new PropertyChangedEventHandler<int>(this.child_ZOrderChanged);
      child.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.child_SizeChanged);
      this.PerformLayout();
    }

    public void OnControlRemoved(dfControl container, dfControl child)
    {
      child.ZOrderChanged -= new PropertyChangedEventHandler<int>(this.child_ZOrderChanged);
      child.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.child_SizeChanged);
      this.PerformLayout();
    }

    public void OnSizeChanged(dfControl control, Vector2 value) => this.PerformLayout();

    private void child_SizeChanged(dfControl control, Vector2 value) => this.PerformLayout();

    private void child_ZOrderChanged(dfControl control, int value) => this.PerformLayout();

    public void PerformLayout()
    {
      if ((Object) this.panel == (Object) null)
        this.panel = this.GetComponent<dfPanel>();
      Vector3 vector3 = new Vector3((float) this.borderPadding.left, (float) this.borderPadding.top);
      bool flag1 = true;
      float num1 = this.flowDirection != dfControlOrientation.Horizontal || this.maxLayoutSize <= 0 ? this.panel.Width - (float) this.borderPadding.right : (float) this.maxLayoutSize;
      float num2 = this.flowDirection != dfControlOrientation.Vertical || this.maxLayoutSize <= 0 ? this.panel.Height - (float) this.borderPadding.bottom : (float) this.maxLayoutSize;
      int b = 0;
      dfList<dfControl> controls = this.panel.Controls;
      int index = 0;
      while (index < controls.Count)
      {
        dfControl control = controls[index];
        if (control.enabled && control.gameObject.activeSelf && !this.excludedControls.Contains(control))
        {
          if (!flag1)
          {
            if (this.flowDirection == dfControlOrientation.Horizontal)
              vector3.x += this.itemSpacing.x;
            else
              vector3.y += this.itemSpacing.y;
          }
          bool flag2;
          if (this.flowDirection == dfControlOrientation.Horizontal)
          {
            if (!flag1 && (double) vector3.x + (double) control.Width > (double) num1 + 1.4012984643248171E-45)
            {
              vector3.x = (float) this.borderPadding.left;
              vector3.y += (float) b;
              b = 0;
              flag2 = true;
            }
          }
          else if (!flag1 && (double) vector3.y + (double) control.Height > (double) num2 + 1.4012984643248171E-45)
          {
            vector3.y = (float) this.borderPadding.top;
            vector3.x += (float) b;
            b = 0;
            flag2 = true;
          }
          control.RelativePosition = vector3;
          if (this.flowDirection == dfControlOrientation.Horizontal)
          {
            vector3.x += control.Width;
            b = Mathf.Max(Mathf.CeilToInt(control.Height + this.itemSpacing.y), b);
          }
          else
          {
            vector3.y += control.Height;
            b = Mathf.Max(Mathf.CeilToInt(control.Width + this.itemSpacing.x), b);
          }
          control.IsVisible = this.canShowControlUnclipped(control);
        }
        ++index;
        flag1 = false;
      }
    }

    private bool canShowControlUnclipped(dfControl control)
    {
      if (!this.hideClippedControls)
        return true;
      Vector3 relativePosition = control.RelativePosition;
      return (double) relativePosition.x + (double) control.Width < (double) this.panel.Width - (double) this.borderPadding.right && (double) relativePosition.y + (double) control.Height < (double) this.panel.Height - (double) this.borderPadding.bottom;
    }
  }

