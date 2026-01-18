using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class VfxController : BraveBehaviour
    {
        public List<AIAnimator.NamedVFXPool> OtherVfx;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int index = 0; index < this.OtherVfx.Count; ++index)
                this.OtherVfx[index].vfxPool.DestroyAll();
        }

        public VFXPool GetVfx(string name)
        {
            return this.OtherVfx.Find((Predicate<AIAnimator.NamedVFXPool>) (n => n.name == name))?.vfxPool;
        }

        public void PlayVfx(string name, Vector2? sourceNormal = null, Vector2? sourceVelocity = null)
        {
            for (int index = 0; index < this.OtherVfx.Count; ++index)
            {
                AIAnimator.NamedVFXPool namedVfxPool = this.OtherVfx[index];
                if (namedVfxPool.name == name)
                {
                    if ((bool) (UnityEngine.Object) namedVfxPool.anchorTransform)
                        namedVfxPool.vfxPool.SpawnAtLocalPosition(Vector3.zero, 0.0f, namedVfxPool.anchorTransform, sourceNormal, sourceVelocity, true);
                    else
                        namedVfxPool.vfxPool.SpawnAtPosition((Vector3) this.specRigidbody.UnitCenter, parent: this.transform, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, keepReferences: true);
                }
            }
        }

        public void StopVfx(string name)
        {
            for (int index = 0; index < this.OtherVfx.Count; ++index)
            {
                AIAnimator.NamedVFXPool namedVfxPool = this.OtherVfx[index];
                if (namedVfxPool.name == name)
                    namedVfxPool.vfxPool.DestroyAll();
            }
        }
    }

