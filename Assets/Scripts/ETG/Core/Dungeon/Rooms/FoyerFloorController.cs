using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class FoyerFloorController : MonoBehaviour
    {
        public string FinalFormSpriteName;
        public string IntermediateFormSpriteName;
        public string BaseSpriteName;
        public tk2dSprite PitSprite;
        public string FinalPitName;
        public string IntermediatePitName;
        public string BasePitName;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new FoyerFloorController__Startc__Iterator0()
            {
                _this = this
            };
        }
    }

