using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class MysteryMimicManController : MonoBehaviour
    {
        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MysteryMimicManController__Startc__Iterator0()
            {
                _this = this
            };
        }
    }

