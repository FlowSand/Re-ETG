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

