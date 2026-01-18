using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Data Binding/Key Binding")]
[Serializable]
public class dfControlKeyBinding : MonoBehaviour, IDataBindingComponent
  {
    [SerializeField]
    protected dfControl control;
    [SerializeField]
    protected KeyCode keyCode;
    [SerializeField]
    protected bool shiftPressed;
    [SerializeField]
    protected bool altPressed;
    [SerializeField]
    protected bool controlPressed;
    [SerializeField]
    protected dfComponentMemberInfo target;
    private bool isBound;

    public dfControl Control
    {
      get => this.control;
      set
      {
        if (this.isBound)
          this.Unbind();
        this.control = value;
      }
    }

    public KeyCode KeyCode
    {
      get => this.keyCode;
      set => this.keyCode = value;
    }

    public bool AltPressed
    {
      get => this.altPressed;
      set => this.altPressed = value;
    }

    public bool ControlPressed
    {
      get => this.controlPressed;
      set => this.controlPressed = value;
    }

    public bool ShiftPressed
    {
      get => this.shiftPressed;
      set => this.shiftPressed = value;
    }

    public dfComponentMemberInfo Target
    {
      get => this.target;
      set
      {
        if (this.isBound)
          this.Unbind();
        this.target = value;
      }
    }

    public bool IsBound => this.isBound;

    public void Awake()
    {
    }

    public void OnEnable()
    {
    }

    public void Start()
    {
      if (!((UnityEngine.Object) this.control != (UnityEngine.Object) null) || !this.target.IsValid)
        return;
      this.Bind();
    }

    public void Bind()
    {
      if (this.isBound)
        this.Unbind();
      if ((UnityEngine.Object) this.control != (UnityEngine.Object) null)
        this.control.KeyDown += new KeyPressHandler(this.eventSource_KeyDown);
      this.isBound = true;
    }

    public void Unbind()
    {
      if ((UnityEngine.Object) this.control != (UnityEngine.Object) null)
        this.control.KeyDown -= new KeyPressHandler(this.eventSource_KeyDown);
      this.isBound = false;
    }

    private void eventSource_KeyDown(dfControl sourceControl, dfKeyEventArgs args)
    {
      if (args.KeyCode != this.keyCode || args.Shift != this.shiftPressed || args.Control != this.controlPressed || args.Alt != this.altPressed)
        return;
      this.target.GetMethod().Invoke((object) this.target.Component, (object[]) null);
    }
  }

