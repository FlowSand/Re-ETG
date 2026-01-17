// Decompiled with JetBrains decompiler
// Type: DraGunThirdEyeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class DraGunThirdEyeController : MonoBehaviour
    {
      public GameObject IntroDummy;
      public GameObject IntroAttachPoint;
      public GameObject RoarDummy;
      public GameObject RoarAttachPoint;
      public GameObject AttachPoint;
      public List<ParticleSystem> particleSystems;
      private List<Renderer> m_particleRenderers;

      public void Awake()
      {
        this.m_particleRenderers = new List<Renderer>(this.particleSystems.Count);
        for (int index = 0; index < this.particleSystems.Count; ++index)
          this.m_particleRenderers.Add(this.particleSystems[index].GetComponent<Renderer>());
      }

      public void LateUpdate()
      {
        GameObject gameObject = this.AttachPoint;
        if (this.IntroDummy.activeSelf)
          gameObject = this.IntroAttachPoint;
        else if (this.RoarDummy.activeSelf)
          gameObject = this.RoarAttachPoint;
        for (int index = 0; index < this.particleSystems.Count; ++index)
        {
          this.m_particleRenderers[index].enabled = true;
          if (gameObject.activeSelf)
          {
            this.particleSystems[index].enableEmission = true;
            this.transform.position = gameObject.transform.position;
          }
          else
            this.particleSystems[index].enableEmission = false;
          if (GameManager.IsBossIntro)
            this.particleSystems[index].Simulate(GameManager.INVARIANT_DELTA_TIME, true, false);
          else if (this.particleSystems[index].isPaused)
            this.particleSystems[index].Play();
        }
      }
    }

}
