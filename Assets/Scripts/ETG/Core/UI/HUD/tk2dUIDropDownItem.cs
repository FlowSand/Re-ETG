using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIDropDownItem")]
public class tk2dUIDropDownItem : tk2dUIBaseItemControl
  {
    public tk2dTextMesh label;
    public float height;
    public tk2dUIUpDownHoverButton upDownHoverBtn;
    private int index;

    public int Index
    {
      get => this.index;
      set => this.index = value;
    }

    public event Action<tk2dUIDropDownItem> OnItemSelected;

    public string LabelText
    {
      get => this.label.text;
      set
      {
        this.label.text = value;
        this.label.Commit();
      }
    }

    private void OnEnable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnClick += new System.Action(this.ItemSelected);
    }

    private void OnDisable()
    {
      if (!(bool) (UnityEngine.Object) this.uiItem)
        return;
      this.uiItem.OnClick -= new System.Action(this.ItemSelected);
    }

    private void ItemSelected()
    {
      if (this.OnItemSelected == null)
        return;
      this.OnItemSelected(this);
    }
  }

