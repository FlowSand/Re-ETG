using System.Collections;
using System.Diagnostics;

using FullInspector;

#nullable disable

[InspectorDropdownName("ManfredsRival/Magic2")]
public class ManfredsRivalMagic2 : ManfredsRivalMagic1
    {
        private const int NumTimes = 3;
        private const int NumBulletsMainWave = 16 /*0x10*/;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ManfredsRivalMagic2__Topc__Iterator0()
            {
                _this = this
            };
        }
    }

