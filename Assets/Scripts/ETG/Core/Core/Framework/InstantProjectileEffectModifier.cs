using System.Collections;
using System.Diagnostics;

#nullable disable

public class InstantProjectileEffectModifier : BraveBehaviour
    {
        public bool DoesWhiteFlash;
        public float RoomDamageRadius = 10f;
        public VFXPool AdditionalVFX;
        public bool DoesAdditionalScreenShake;
        [ShowInInspectorIf("DoesAdditionalScreenShake", false)]
        public ScreenShakeSettings AdditionalScreenShake;
        public bool DoesRadialProjectileModule;
        [ShowInInspectorIf("DoesRadialProjectileModule", false)]
        public RadialBurstInterface RadialModule;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InstantProjectileEffectModifier__Startc__Iterator0()
            {
                _this = this
            };
        }
    }

