using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Actionbar/Show Spell Window")]
public class ShowSpellWindow : MonoBehaviour
    {
        private bool busy;
        private bool isVisible;

        private void OnEnable()
        {
            GameObject.Find("Spell Window").GetComponent<dfControl>().IsVisible = false;
        }

        private void OnClick()
        {
            if (this.busy)
                return;
            this.StopAllCoroutines();
            dfControl component = GameObject.Find("Spell Window").GetComponent<dfControl>();
            if (!this.isVisible)
                this.StartCoroutine(this.showWindow(component));
            else
                this.StartCoroutine(this.hideWindow(component));
        }

        [DebuggerHidden]
        private IEnumerator hideWindow(dfControl window)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ShowSpellWindow__hideWindowc__Iterator0()
            {
                window = window,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator showWindow(dfControl window)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ShowSpellWindow__showWindowc__Iterator1()
            {
                window = window,
                _this = this
            };
        }
    }

