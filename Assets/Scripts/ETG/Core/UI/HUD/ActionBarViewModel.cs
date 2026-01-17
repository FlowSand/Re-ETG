// Decompiled with JetBrains decompiler
// Type: ActionBarViewModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("Daikon Forge/Examples/Actionbar/View Model")]
    public class ActionBarViewModel : MonoBehaviour
    {
      [SerializeField]
      private float _health;
      [SerializeField]
      private int _maxHealth = 100;
      [SerializeField]
      private float _healthRegenRate = 0.5f;
      [SerializeField]
      private float _energy;
      [SerializeField]
      private int _maxEnergy = 100;
      [SerializeField]
      private float _energyRegenRate = 1f;
      private List<ActionBarViewModel.SpellCastInfo> activeSpells = new List<ActionBarViewModel.SpellCastInfo>();

      public event ActionBarViewModel.SpellEventHandler SpellActivated;

      public event ActionBarViewModel.SpellEventHandler SpellDeactivated;

      public int MaxHealth => this._maxHealth;

      public int MaxEnergy => this._maxEnergy;

      public int Health
      {
        get => (int) this._health;
        private set => this._health = (float) Mathf.Max(0, Mathf.Min(this._maxHealth, value));
      }

      public int Energy
      {
        get => (int) this._energy;
        private set => this._energy = (float) Mathf.Max(0, Mathf.Min(this._maxEnergy, value));
      }

      private void OnEnable()
      {
      }

      private void Start()
      {
        this._health = 35f;
        this._energy = 50f;
      }

      private void Update()
      {
        this._health = Mathf.Min((float) this._maxHealth, this._health + BraveTime.DeltaTime * this._healthRegenRate);
        this._energy = Mathf.Min((float) this._maxEnergy, this._energy + BraveTime.DeltaTime * this._energyRegenRate);
        for (int index = this.activeSpells.Count - 1; index >= 0; --index)
        {
          ActionBarViewModel.SpellCastInfo activeSpell = this.activeSpells[index];
          float num = UnityEngine.Time.realtimeSinceStartup - activeSpell.whenCast;
          if ((double) activeSpell.spell.Recharge <= (double) num)
          {
            this.activeSpells.RemoveAt(index);
            if (this.SpellDeactivated != null)
              this.SpellDeactivated(activeSpell.spell);
          }
        }
      }

      public void CastSpell(string spellName)
      {
        SpellDefinition spell = SpellDefinition.FindByName(spellName);
        if (spell == null)
          throw new InvalidCastException();
        if (this.activeSpells.Any<ActionBarViewModel.SpellCastInfo>((Func<ActionBarViewModel.SpellCastInfo, bool>) (activeSpell => activeSpell.spell == spell)) || this.Energy < spell.Cost)
          return;
        this.Energy -= spell.Cost;
        this.activeSpells.Add(new ActionBarViewModel.SpellCastInfo()
        {
          spell = spell,
          whenCast = UnityEngine.Time.realtimeSinceStartup
        });
        if (this.SpellActivated == null)
          return;
        this.SpellActivated(spell);
      }

      public delegate void SpellEventHandler(SpellDefinition spell);

      private class SpellCastInfo
      {
        public SpellDefinition spell;
        public float whenCast;
      }
    }

}
