using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Animate Popup")]
public class AnimatePopup : MonoBehaviour
    {
        private const float ANIMATION_LENGTH = 0.15f;
        private dfListbox target;

        private void OnDropdownOpen(dfDropdown dropdown, dfListbox popup)
        {
            if ((Object) this.target != (Object) null)
            {
                this.StopCoroutine("animateOpen");
                this.StopCoroutine("animateClose");
                Object.Destroy((Object) this.target.gameObject);
            }
            this.target = popup;
            this.StartCoroutine(this.animateOpen(popup));
        }

        private void OnDropdownClose(dfDropdown dropdown, dfListbox popup)
        {
            this.StartCoroutine(this.animateClose(popup));
        }

        [DebuggerHidden]
        private IEnumerator animateOpen(dfListbox popup)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new AnimatePopup__animateOpenc__Iterator0()
            {
                popup = popup,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator animateClose(dfListbox popup)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new AnimatePopup__animateClosec__Iterator1()
            {
                popup = popup,
                _this = this
            };
        }
    }

