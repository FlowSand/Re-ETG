// Decompiled with JetBrains decompiler
// Type: TimedObjectKiller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TimedObjectKiller : MonoBehaviour
    {
      public float lifeTime = 1f;
      private static int m_lightCullingMaskID = -1;
      public TimedObjectKiller.PoolType m_poolType;
      private Light m_light;
      private Renderer m_renderer;

      public void Start()
      {
        if (TimedObjectKiller.m_lightCullingMaskID == -1)
          TimedObjectKiller.m_lightCullingMaskID = 1 << LayerMask.NameToLayer("BG_Critical") | 1 << LayerMask.NameToLayer("BG_Nonsense");
        if (SpawnManager.IsPooled(this.gameObject))
        {
          this.m_poolType = TimedObjectKiller.PoolType.Pooled;
        }
        else
        {
          this.m_poolType = TimedObjectKiller.PoolType.NonPooled;
          for (Transform parent = this.transform.parent; (bool) (Object) parent; parent = parent.parent)
          {
            if (SpawnManager.IsPooled(parent.gameObject))
            {
              this.m_poolType = TimedObjectKiller.PoolType.SonOfPooled;
              break;
            }
          }
        }
        if (this.m_poolType == TimedObjectKiller.PoolType.SonOfPooled)
        {
          this.m_light = this.GetComponent<Light>();
          if ((Object) this.m_light != (Object) null)
            this.m_light.cullingMask = TimedObjectKiller.m_lightCullingMaskID;
          this.m_renderer = this.GetComponent<Renderer>();
        }
        this.Init();
      }

      private void Init() => this.StartCoroutine(this.HandleDeath());

      [DebuggerHidden]
      private IEnumerator HandleDeath()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TimedObjectKiller.\u003CHandleDeath\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void OnSpawned()
      {
        if (!this.enabled)
          return;
        if ((bool) (Object) this.m_light)
          this.m_light.enabled = true;
        if ((bool) (Object) this.m_renderer)
          this.m_renderer.enabled = true;
        this.Start();
      }

      public void OnDespawned() => this.StopAllCoroutines();

      public enum PoolType
      {
        Pooled,
        SonOfPooled,
        NonPooled,
      }
    }

}
