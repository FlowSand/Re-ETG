// Decompiled with JetBrains decompiler
// Type: SpellInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/Examples/Actionbar/Spell Inventory")]
public class SpellInventory : MonoBehaviour
{
  [SerializeField]
  protected string spellName = string.Empty;
  private bool needRefresh = true;

  public string Spell
  {
    get => this.spellName;
    set
    {
      this.spellName = value;
      this.refresh();
    }
  }

  private void OnEnable()
  {
    this.refresh();
    this.gameObject.GetComponent<dfControl>().SizeChanged += (PropertyChangedEventHandler<Vector2>) ((source, value) => this.needRefresh = true);
  }

  private void LateUpdate()
  {
    if (!this.needRefresh)
      return;
    this.needRefresh = false;
    this.refresh();
  }

  public void OnResolutionChanged() => this.needRefresh = true;

  private void refresh()
  {
    dfControl component = this.gameObject.GetComponent<dfControl>();
    dfScrollPanel parent = component.Parent as dfScrollPanel;
    if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
      component.Width = parent.Width - (float) parent.ScrollPadding.horizontal;
    SpellSlot componentInChildren = component.GetComponentInChildren<SpellSlot>();
    dfLabel dfLabel1 = component.Find<dfLabel>("lblCosts");
    dfLabel dfLabel2 = component.Find<dfLabel>("lblName");
    dfLabel dfLabel3 = component.Find<dfLabel>("lblDescription");
    if ((UnityEngine.Object) dfLabel1 == (UnityEngine.Object) null)
      throw new Exception("Not found: lblCosts");
    if ((UnityEngine.Object) dfLabel2 == (UnityEngine.Object) null)
      throw new Exception("Not found: lblName");
    if ((UnityEngine.Object) dfLabel3 == (UnityEngine.Object) null)
      throw new Exception("Not found: lblDescription");
    SpellDefinition byName = SpellDefinition.FindByName(this.Spell);
    if (byName == null)
    {
      componentInChildren.Spell = string.Empty;
      dfLabel1.Text = string.Empty;
      dfLabel2.Text = string.Empty;
      dfLabel3.Text = string.Empty;
    }
    else
    {
      componentInChildren.Spell = this.spellName;
      dfLabel2.Text = byName.Name;
      dfLabel1.Text = $"{byName.Cost}/{byName.Recharge}/{byName.Delay}";
      dfLabel3.Text = byName.Description;
      float a = dfLabel3.RelativePosition.y + dfLabel3.Size.y;
      float b = dfLabel1.RelativePosition.y + dfLabel1.Size.y;
      component.Height = Mathf.Max(a, b) + 5f;
    }
  }
}
