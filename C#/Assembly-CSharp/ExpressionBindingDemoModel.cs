// Decompiled with JetBrains decompiler
// Type: ExpressionBindingDemoModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Data Binding/Expression Binding Model")]
public class ExpressionBindingDemoModel : MonoBehaviour
{
  private dfListbox list;

  public List<string> SpellsLearned { get; set; }

  public SpellDefinition SelectedSpell
  {
    get => SpellDefinition.FindByName(this.SpellsLearned[this.list.SelectedIndex]);
  }

  private void Awake()
  {
    this.list = this.GetComponentInChildren<dfListbox>();
    this.list.SelectedIndex = 0;
    this.SpellsLearned = ((IEnumerable<SpellDefinition>) SpellDefinition.AllSpells).OrderBy<SpellDefinition, string>((Func<SpellDefinition, string>) (x => x.Name)).Select<SpellDefinition, string>((Func<SpellDefinition, string>) (x => x.Name)).ToList<string>();
  }
}
