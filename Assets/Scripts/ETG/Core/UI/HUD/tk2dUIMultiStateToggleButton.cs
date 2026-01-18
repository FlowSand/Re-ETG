using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIMultiStateToggleButton")]
public class tk2dUIMultiStateToggleButton : tk2dUIBaseItemControl
  {
    public GameObject[] states;
    public bool activateOnPress;
    private int index;
    public string SendMessageOnStateToggleMethodName = string.Empty;

    public event Action<tk2dUIMultiStateToggleButton> OnStateToggle;

    public int Index
    {
      get => this.index;
      set
      {
        if (value >= this.states.Length)
          value = this.states.Length;
        if (value < 0)
          value = 0;
        if (this.index == value)
          return;
        this.index = value;
        this.SetState();
        if (this.OnStateToggle != null)
          this.OnStateToggle(this);
        this.DoSendMessage(this.SendMessageOnStateToggleMethodName, (object) this);
      }
    }

    private void Start() => this.SetState();

    private void OnEnable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnClick += new System.Action(this.ButtonClick);
      this.uiItem.OnDown += new System.Action(this.ButtonDown);
    }

    private void OnDisable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnClick -= new System.Action(this.ButtonClick);
      this.uiItem.OnDown -= new System.Action(this.ButtonDown);
    }

    private void ButtonClick()
    {
      if (this.activateOnPress)
        return;
      this.ButtonToggle();
    }

    private void ButtonDown()
    {
      if (!this.activateOnPress)
        return;
      this.ButtonToggle();
    }

    private void ButtonToggle()
    {
      if (this.Index + 1 >= this.states.Length)
        this.Index = 0;
      else
        ++this.Index;
    }

    private void SetState()
    {
      for (int index = 0; index < this.states.Length; ++index)
      {
        if ((UnityEngine.Object) this.states[index] != (UnityEngine.Object) null)
        {
          if (index != this.index)
          {
            if (this.states[index].activeInHierarchy)
              this.states[index].SetActive(false);
          }
          else if (!this.states[index].activeInHierarchy)
            this.states[index].SetActive(true);
        }
      }
    }
  }

