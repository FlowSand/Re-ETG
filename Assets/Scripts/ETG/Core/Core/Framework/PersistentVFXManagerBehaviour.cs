// Decompiled with JetBrains decompiler
// Type: PersistentVFXManagerBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PersistentVFXManagerBehaviour : BraveBehaviour
    {
      protected List<GameObject> attachedPersistentVFX;
      protected List<GameObject> attachedDestructibleVFX;
      private bool m_pvmbDestroyed;

      public void AttachPersistentVFX(GameObject vfx)
      {
        if (this.attachedPersistentVFX == null)
          this.attachedPersistentVFX = new List<GameObject>();
        this.attachedPersistentVFX.Add(vfx);
        vfx.transform.parent = this.transform;
      }

      public void AttachDestructibleVFX(GameObject vfx)
      {
        if (this.m_pvmbDestroyed)
        {
          Object.Destroy((Object) vfx);
        }
        else
        {
          if (this.attachedDestructibleVFX == null)
            this.attachedDestructibleVFX = new List<GameObject>();
          this.attachedDestructibleVFX.Add(vfx);
          vfx.transform.parent = this.transform;
        }
      }

      public void TriggerPersistentVFXClear()
      {
        this.TriggerPersistentVFXClear(Vector3.right, 180f, 0.5f, 0.3f, 0.7f);
      }

      public void TriggerPersistentVFXClear(
        Vector3 startingForce,
        float forceVarianceAngle,
        float forceVarianceMagnitude,
        float startingHeightMin,
        float startingHeightMax)
      {
        if (this.attachedPersistentVFX != null)
        {
          for (int index = 0; index < this.attachedPersistentVFX.Count; ++index)
          {
            Vector3 startingForce1 = Quaternion.Euler(0.0f, 0.0f, Random.Range(-forceVarianceAngle, forceVarianceAngle)) * startingForce * (1f + Random.Range(-forceVarianceMagnitude, forceVarianceMagnitude));
            float startingHeight = Random.Range(startingHeightMin, startingHeightMax);
            if ((bool) (Object) this.attachedPersistentVFX[index])
            {
              this.attachedPersistentVFX[index].transform.parent = (Transform) null;
              this.attachedPersistentVFX[index].GetComponent<PersistentVFXBehaviour>().BecomeDebris(startingForce1, startingHeight);
            }
          }
          this.attachedPersistentVFX.Clear();
        }
        if (this.attachedDestructibleVFX == null)
          return;
        this.TriggerDestructibleVFXClear();
      }

      public void TriggerTemporaryDestructibleVFXClear()
      {
        if (this.attachedDestructibleVFX == null)
          return;
        for (int index = 0; index < this.attachedDestructibleVFX.Count; ++index)
          Object.Destroy((Object) this.attachedDestructibleVFX[index]);
        this.attachedDestructibleVFX.Clear();
      }

      public void TriggerDestructibleVFXClear()
      {
        this.m_pvmbDestroyed = true;
        if (this.attachedDestructibleVFX == null)
          return;
        for (int index = 0; index < this.attachedDestructibleVFX.Count; ++index)
          Object.Destroy((Object) this.attachedDestructibleVFX[index]);
        this.attachedDestructibleVFX.Clear();
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
