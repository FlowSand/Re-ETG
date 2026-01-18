using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Actionbar/Spell Slot")]
[ExecuteInEditMode]
public class SpellSlot : MonoBehaviour
    {
        [SerializeField]
        protected string spellName = string.Empty;
        [SerializeField]
        protected int slotNumber;
        [SerializeField]
        protected bool isActionSlot;
        private bool isSpellActive;

        public bool IsActionSlot
        {
            get => this.isActionSlot;
            set
            {
                this.isActionSlot = value;
                this.refresh();
            }
        }

        public string Spell
        {
            get => this.spellName;
            set
            {
                this.spellName = value;
                this.refresh();
            }
        }

        public int SlotNumber
        {
            get => this.slotNumber;
            set
            {
                this.slotNumber = value;
                this.refresh();
            }
        }

        private void OnEnable() => this.refresh();

        private void Start() => this.refresh();

        private void Update()
        {
            if (!this.IsActionSlot || string.IsNullOrEmpty(this.Spell) || !Input.GetKeyDown((KeyCode) (this.slotNumber + 48)))
                return;
            this.castSpell();
        }

        public void onSpellActivated(SpellDefinition spell)
        {
            if (spell.Name != this.Spell)
                return;
            this.StartCoroutine(this.showCooldown());
        }

        private void OnDoubleClick()
        {
            if (this.isSpellActive || string.IsNullOrEmpty(this.Spell))
                return;
            this.castSpell();
        }

        private void OnDragStart(dfControl source, dfDragEventArgs args)
        {
            if (!this.allowDrag(args))
                return;
            if (string.IsNullOrEmpty(this.Spell))
            {
                args.State = dfDragDropState.Denied;
            }
            else
            {
                dfSprite sprite = this.GetComponent<dfControl>().Find("Icon") as dfSprite;
                Ray ray = sprite.GetCamera().ScreenPointToRay(Input.mousePosition);
                Vector2 position = Vector2.zero;
                if (!sprite.GetHitPosition(ray, out position))
                    return;
                ActionbarsDragCursor.Show(sprite, (Vector2) Input.mousePosition, position);
                if (this.IsActionSlot)
                    sprite.SpriteName = string.Empty;
                args.State = dfDragDropState.Dragging;
                args.Data = (object) this;
            }
            args.Use();
        }

        private void OnDragEnd(dfControl source, dfDragEventArgs args)
        {
            ActionbarsDragCursor.Hide();
            if (!this.isActionSlot)
                return;
            if (args.State == dfDragDropState.CancelledNoTarget)
                this.Spell = string.Empty;
            this.refresh();
        }

        private void OnDragDrop(dfControl source, dfDragEventArgs args)
        {
            if (this.allowDrop(args))
            {
                args.State = dfDragDropState.Dropped;
                SpellSlot data = args.Data as SpellSlot;
                string spellName = this.spellName;
                this.Spell = data.Spell;
                if (data.IsActionSlot)
                    data.Spell = spellName;
            }
            else
                args.State = dfDragDropState.Denied;
            args.Use();
        }

        private bool allowDrag(dfDragEventArgs args)
        {
            return !this.isSpellActive && !string.IsNullOrEmpty(this.spellName);
        }

        private bool allowDrop(dfDragEventArgs args)
        {
            return !this.isSpellActive && (Object) (args.Data as SpellSlot) != (Object) null && this.IsActionSlot;
        }

        [DebuggerHidden]
        private IEnumerator showCooldown()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new SpellSlot__showCooldownc__Iterator0()
            {
                _this = this
            };
        }

        private void castSpell()
        {
            ActionBarViewModel actionBarViewModel = ((IEnumerable<Object>) Object.FindObjectsOfType(typeof (ActionBarViewModel))).FirstOrDefault<Object>() as ActionBarViewModel;
            if (!((Object) actionBarViewModel != (Object) null))
                return;
            actionBarViewModel.CastSpell(this.Spell);
        }

        private void refresh()
        {
            SpellDefinition byName = SpellDefinition.FindByName(this.Spell);
            this.GetComponent<dfControl>().Find<dfSprite>("Icon").SpriteName = byName == null ? string.Empty : byName.Icon;
            dfButton componentInChildren = this.GetComponentInChildren<dfButton>();
            componentInChildren.IsVisible = this.IsActionSlot;
            componentInChildren.Text = this.slotNumber.ToString();
        }
    }

